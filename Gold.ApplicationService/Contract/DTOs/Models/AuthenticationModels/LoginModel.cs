using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.Models.AuthenticationModels
{
    public class LoginModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
		public bool RememberMe { get; set; }
    }
}
