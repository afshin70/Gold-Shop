using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.Domain.Contract.DTOs.UserModels.CustomerModels
{
    public class CustomerInfoModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string NationalCode { get; set; }
        public string Mobile { get; set; }
        public bool IsActive { get; set; }
    }
}
