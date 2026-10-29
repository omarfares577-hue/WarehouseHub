using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseHub.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; } = null!;
        public Address Address { get; set; } = null!;
    }
    public class Address
    {
        public int Id { get; set; }  
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string ZipCode { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;
        public string AppUserId { get; set; } = null!;
    }
}
