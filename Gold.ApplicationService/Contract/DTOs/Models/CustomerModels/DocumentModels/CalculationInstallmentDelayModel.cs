using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.DocumentModels
{
	public class CalculationInstallmentDelayModel
	{
        public long  DelayAmount { get; set; }
        public int CurrentInstallmentDelayDay { get; set; }
        public int TotalDelayDayToCurrentInstallment { get; set; }
    }
}
