using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseHub.Domain.Entities;

namespace WarehouseHub.Core.Contracts
{
    public interface ITokenServices
    {
        Task<string> CreateTokenAsync(AppUser user, UserManager<AppUser> admin);
    }
}
