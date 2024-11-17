using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.CustomerMessageModels
{
    public class CustomerMessageModel
    {
        public string Content { get; set; } = string.Empty;
		public string PersianDate { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
	}
}
