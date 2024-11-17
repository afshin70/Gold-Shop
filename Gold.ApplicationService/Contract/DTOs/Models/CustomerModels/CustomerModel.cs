using Gold.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.Models.CustomerModels
{
    public class CustomerModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
		public string NationalCode { get; set; } = string.Empty;
		public string Mobile { get; set; } = string.Empty;
		public bool IsActive { get; set; }
        /// <summary>
        /// بد حساب یا خوش حساب
        /// fill by AccountStatusType enum
        /// </summary>
        public string AccountStatus { get; set; } = string.Empty;
        public string AccountStatusClass { get; set; } = string.Empty;

        public bool HasProfileImage { get; set; }

    }
}
