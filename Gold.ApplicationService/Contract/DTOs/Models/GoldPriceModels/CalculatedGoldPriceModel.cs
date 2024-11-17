using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.Models.GoldPriceModels
{
    public class CalculatedGoldPriceModel
    {
        /// <summary>
        /// ارزش طلای خالص
        /// </summary>
        public string PureGoldPrice { get; set; }
        /// <summary>
        /// جمع نهایی فاکتور
        /// </summary>
        public string InvoiceTotalPrice { get; set; }
    }
}
