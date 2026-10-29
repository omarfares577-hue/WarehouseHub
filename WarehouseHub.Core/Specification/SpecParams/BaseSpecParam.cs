using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseHub.Core.Specification.SpecParams
{
    public class BaseSpecParam
    {
        public string? Sort { get; set; }
        private string? _search;
        public string? Search { get => _search; set =>_search = value?.ToLower(); }
        public int PageIndex = 1;
        private int _pagesize = 5;
        private  int _maxpagesize =  50;
        public int PageSize { get => _pagesize;
            set => _pagesize = value > _maxpagesize ? _maxpagesize : value;
        }
    }
}
