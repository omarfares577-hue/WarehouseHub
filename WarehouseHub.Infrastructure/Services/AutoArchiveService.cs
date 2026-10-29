using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WarehouseHub.Domain.Entities;
using WarehouseHub.Infrastructure.Data;

public class AutoArchiveService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AutoArchiveService> _logger;

    public AutoArchiveService(IServiceScopeFactory scopeFactory, ILogger<AutoArchiveService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<WarehouseContext>(); 

                    var oneYearAgo = DateTime.UtcNow.AddYears(-1);

                    var oldLogs = await context.Set<InventoryLog>()
                        .Where(x => x.Timestamp <= oneYearAgo)
                        .ToListAsync(stoppingToken);

                    if (oldLogs.Any())
                    {
                        var archiveLogs = oldLogs.Select(log => new ArchivedInventoryLog
                        {
                            WarehouseItemId = log.WarehouseItemId,
                            SupplierId = log.SupplierId,
                            QuantityChanged = log.QuantityChanged,
                            OperationType = log.OperationType,
                            Timestamp = log.Timestamp
                        }).ToList();

                        await context.Set<ArchivedInventoryLog>().AddRangeAsync(archiveLogs, stoppingToken);
                        context.Set<InventoryLog>().RemoveRange(oldLogs);

                        await context.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation($"{oldLogs.Count()} Have been Archived Successfully");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UnExpected Error during AutArchive");
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}