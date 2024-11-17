using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.Models.ReportModels
{
    public class ManagersOperationReportModel
    {
        public long Id { get; set; }
        public string Page { get; set; } = string.Empty;
		public string Operation { get; set; } = string.Empty;
		public string UserName { get; set; } = string.Empty;
		public string PersianDate { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
	}
}
