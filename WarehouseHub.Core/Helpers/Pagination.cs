using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseHub.Core.Helpers
{
    public class Pagination<T> where T: class
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public IReadOnlyList<T> Data { get; set; }
        public Pagination(int index, int size, int total, IReadOnlyList<T> data)
        {
            PageIndex = index;
            PageSize = size;
            Total = total;
            Data = data;
        }
    }
}
