using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.Domain.Contract.DTOs.UserModels
{
    public class UserResetCodeAndEmailInfoModel
    {
        public string ResetPsswordCode { get; set; }
        public string UserName { get; set; }
        public DateTime ResetPsswordCodeExpirationDate { get; set; }
        public string UserMail { get; set; }
    }
}
