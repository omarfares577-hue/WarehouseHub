using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseHub.Core.Dtos.Identity
{
    public class UserDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;

    }
}
