using Gold.SharedKernel.Enums;
using Gold.SharedKernel.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.Models.DocumentModels
{
    public class DocumentDetailModel
    {

        public long Id { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string NationalCode { get; set; } = string.Empty;
        public DocumentStatus DocumentStatus { get; set; }
        /// <summary>
        /// تاریخ روز فروش یا تاریخ سند
        /// </summary>
        public DateTime DocumentDate { get; set; }
        public string PersianDocumentDate
        {
            get
            {
                return DateTimeTools.GeorgianToPersian(DocumentDate, ShowMode.OnlyDate);
            }
        }
        public string Mobile { get; set; } = string.Empty;
        public string? EssentialTel { get; set; }
        public int DocumentNumber { get; set; }
        public long InvoiceAmount { get 
            {
                return PrePaymentAmount + RemainAmount;    
            } 
        }
        public long PrePaymentAmount { get; set; }
        public long RemainAmount { get; set; }
        public long InstallmentCount { get; set; }
        public long InstallmentAmount { get; set; }
        public string GalleryName { get; set; }=string.Empty;
        public string SellerName { get; set; } = string.Empty;
		public string? AdminDescription { get; set; }
        //public IEnumerable<CollateralInfoModel> CollateralsInfo { get; set; }
    }
}
