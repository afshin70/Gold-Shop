using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.Models.CustomerModels
{
    public class SendMessageModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Nationality { get; set; }
        public string NationalCode { get; set; }
        public string Mobile { get; set; }
        public string AccountStatus { get; set; }
        public string RegisterDate { get; set; }

    }
}
