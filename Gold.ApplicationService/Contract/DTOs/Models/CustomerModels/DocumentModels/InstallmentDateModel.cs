using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.Models.DocumentModels
{
    public class InstallmentDateModel
    {
        public int Row { get; set; }
        public string PersianDate { get; set; } = string.Empty;
        public DateTime GorgianDate { get; set; }
    }
}
