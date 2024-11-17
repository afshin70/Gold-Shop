using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.Models.CustomerModels
{
    public class RegisterCustomerModel
    {
        public string FullName { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
		public bool IsActive { get; set; }
        public int CityId { get; set; }
        public string PostalCode { get; set; } = string.Empty;
		public string NationalCode { get; set; } = string.Empty;
		public string Mobile { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
        public List<CreateEssentialTelModel> EssentialNumbers { get; set; } = new();
    }
}
