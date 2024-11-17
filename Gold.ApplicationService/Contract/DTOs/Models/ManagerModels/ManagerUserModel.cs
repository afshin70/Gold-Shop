using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.Models.ManagerModels
{
    public class ManagerUserModel
    {
        public int ManagerUserId { get; set; }
        public string FullName { get; set; } = string.Empty;
		public string Mobile { get; set; } = string.Empty;
		public string UserName { get; set; } = string.Empty;
		public string AccessLevelTitle { get; set; } = string.Empty;
		public bool IsActive { get; set; }
    } 
}
