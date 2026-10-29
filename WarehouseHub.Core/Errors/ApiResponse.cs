using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseHub.Core.Errors
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = null!;
        public ApiResponse(int statusCode, string? message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessage();
        }

        private string GetDefaultMessage()
        {
            return StatusCode switch
            {
                400 => "Bad Request,Data is invalid",
                401 => "Unauthorized,Password or email is incorrect",
                404 => "Not Found",
                500 => "Internal Server Error",
                _ => "An error occurred"
            };
        } 
    
    } 
}
