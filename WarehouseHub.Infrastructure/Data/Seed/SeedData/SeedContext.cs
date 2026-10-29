using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WarehouseHub.Domain.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WarehouseHub.Infrastructure.Data.Seed.SeedData
{
    public class SeedContext
    {
        public static async Task SeedAsync(WarehouseContext context)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

            // تحديد مسار مجلد التشغيل الحالي بذكاء ليعمل في دوكر ومحلياً
            var basePath = AppContext.BaseDirectory;

            if (!context.Categories.Any())
            {
                var path = Path.Combine(basePath, "Data", "Seed", "category.json");
                if (File.Exists(path))
                {
                    var categoriesData = await File.ReadAllTextAsync(path);
                    var categories = JsonSerializer.Deserialize<List<Category>>(categoriesData, jsonOptions);
                    if (categories != null && categories.Count > 0)
                    {
                        await context.Categories.AddRangeAsync(categories);
                        await context.SaveChangesAsync();
                    }
                }
            }

            if (!context.Suppliers.Any())
            {
                var path = Path.Combine(basePath, "Data", "Seed", "supplier.json");
                if (File.Exists(path))
                {
                    var suppliersData = await File.ReadAllTextAsync(path);
                    var suppliers = JsonSerializer.Deserialize<List<Supplier>>(suppliersData, jsonOptions);
                    if (suppliers != null && suppliers.Count > 0)
                    {
                        await context.Suppliers.AddRangeAsync(suppliers);
                        await context.SaveChangesAsync();
                    }
                }
            }

            if (!context.WarehouseItems.Any())
            {
                var path = Path.Combine(basePath, "Data", "Seed", "items.json");
                if (File.Exists(path))
                {
                    var itemsData = await File.ReadAllTextAsync(path);
                    var items = JsonSerializer.Deserialize<List<WarehouseItem>>(itemsData, jsonOptions);
                    if (items != null && items.Count > 0)
                    {
                        await context.WarehouseItems.AddRangeAsync(items);
                        await context.SaveChangesAsync();
                    }
                }
            }

            if (!context.InventoryLogs.Any())
            {
                var path = Path.Combine(basePath, "Data", "Seed", "logs.json");
                if (File.Exists(path))
                {
                    var logsData = await File.ReadAllTextAsync(path);
                    var logs = JsonSerializer.Deserialize<List<InventoryLog>>(logsData, jsonOptions);
                    if (logs != null && logs.Count > 0)
                    {
                        await context.InventoryLogs.AddRangeAsync(logs);
                        await context.SaveChangesAsync();
                    }
                }
            }
        }
    
    }
    git --version
}

