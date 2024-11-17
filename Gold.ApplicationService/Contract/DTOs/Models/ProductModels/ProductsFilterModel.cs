using Gold.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.Models.ProductModels
{
    public  class ProductsFilterModel
    {
        public int Page { get; set; }
        public int[]? Categories { get; set; }
        public SortBy? SortBy { get; set; }
        public string? Term { get; set; }
        public int PageSize { get; set; } = 16;
    }
}
