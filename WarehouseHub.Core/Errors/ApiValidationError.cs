using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseHub.Core.Errors
{
    public class ApiValidationError : ApiResponse
    {
        public IEnumerable<string> ErrorMessages { get; set; } = Enumerable.Empty<string>();
        public ApiValidationError() : base(400)
        {
            ErrorMessages = new List<string>();
        }

    }
}
