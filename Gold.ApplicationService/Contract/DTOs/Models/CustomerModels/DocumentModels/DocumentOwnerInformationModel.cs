using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels;

namespace Gold.ApplicationService.Contract.DTOs.Models.DocumentModels
{
    public class DocumentOwnerInformationModel
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
		public string NationalCode { get; set; } = string.Empty;
		public string ProvinceName { get; set; } = string.Empty;
		public string CityName { get; set; } = string.Empty;
		public int CityId { get; set; }
        public string Address { get; set; } = string.Empty;
		public string EssentialTel { get; set; } = string.Empty;
		public string EssentialTelRatio { get; set; } = string.Empty;

        public List<DocumentSummaryInfo> Documents { get; set; } = new();


       // public string DocumentWizardDataJson { get; set; }
    }
}
