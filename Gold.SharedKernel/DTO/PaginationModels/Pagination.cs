using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.SharedKernel.DTO.PaginationModels
{
    public class Pagination<TData>
    {
        public int TotalItems { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }
        public List<TData> Data { get; set; }

        public Pagination(int totalItems, int page, int pageSize=10)
        {
            int totalPages = (int)Math.Ceiling((decimal)totalItems/(decimal)pageSize);
            if (page>totalPages)
                page = totalPages;
            if (page<=0)
                page = 1;

            int currentPage = page;
            int startPage = currentPage - 2;
            int endPage = currentPage + 3;
            if (startPage<=0)
            {
                endPage = endPage - (startPage - 1);
                startPage = 1;
            }
            if (endPage>totalPages)
            {
                endPage = totalPages;
                if (endPage>10)
                {
                    startPage = endPage - 4;
                }
            }

            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
        }
    }
}
