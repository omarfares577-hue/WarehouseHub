using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseHub.Core.Errors
{
    public class ApiException : ApiResponse
    {
        public string Details { get; set; } = string.Empty;
        public ApiException(int statusCode, string? message = null, string? details = null) : base(statusCode, message)
        {
            Details = details ?? string.Empty;
        }

    }
}
