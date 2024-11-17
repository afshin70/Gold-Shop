using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.DocumentModels
{
	public class CustomerDocumentDetailModel
    {
        public string FullName { get; set; } = string.Empty;
        public string NationalCode { get; set; } = string.Empty;
		/// <summary>
		/// تاریخ روز فروش
		/// </summary>
		public string SaleDate { get; set; } = string.Empty;
		public string Mobile { get; set; } = string.Empty;
		public string? EssentialTell { get; set; }
        public int DocumentNo { get; set; }
        public string DocumentDate { get; set; }
        /// <summary>
        /// مبلغ فاکتور
        /// </summary>
        public long InvoiceAmount { get; set; }
        public long PrePaymentAmount { get; set; }
        public long RemainAmount { get; set; }
        public byte InstallmentCount { get; set; }
        public long InstallmentAmount { get; set; }
        public List<string> CollateralInfo { get; set; } = new();
        public string Gallery { get; set; } = string.Empty;
        public string Seller { get; set; } = string.Empty;

		//public string Message { get; set; } = string.Empty;
		/// <summary>
		/// مبلغ پرداخت شده
		/// </summary>
		public long PaymentAmount { get; set; }
        /// <summary>
        /// تاریخ تسویه
        /// </summary>
        public string PersianSettleDate { get; set; } = string.Empty;
		/// <summary>
		/// تاریخ تحویل
		/// </summary>
		public string PersianDeliveryDate { get; set; } = string.Empty;
        public string DocumentLastCustomerMessage { get; set; }
        public List<CustomerDocumentInstallmentModel> Installments { get; set; } = new();
    }
}
