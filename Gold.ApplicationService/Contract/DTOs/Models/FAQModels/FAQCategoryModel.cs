using Gold.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.Models.FAQModels
{
    public  class FAQCategoryModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int OrderNo { get; set; }
    }
}
