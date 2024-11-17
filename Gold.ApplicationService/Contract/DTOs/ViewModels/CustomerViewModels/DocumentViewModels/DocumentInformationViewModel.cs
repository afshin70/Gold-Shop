using Gold.ApplicationService.Contract.DTOs.Models.DocumentModels;
using System.ComponentModel.DataAnnotations;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.DocumentViewModels
{
    public class DocumentInformationViewModel
    {
        /// <summary>
        ///مانده
        /// </summary>
        public int RemainAmount { get; set; }
        /// <summary>
        /// پیش پرداخت
        /// </summary>
        public int PrepaymentAmount { get; set; }
        /// <summary>
        /// تعداد اقساط
        /// </summary>
        public int InstallmentCount { get; set; }
        /// <summary>
        /// مبلغ اقساط
        /// </summary>
        public int InstallmentAmount { get; set; }
        /// <summary>
        /// گالری
        /// </summary>
        public int GalleryId { get; set; }
        /// <summary>
        /// فروشنده
        /// </summary>
        public int SellerId { get; set; }
        public string AdminDescription { get; set; }=string.Empty;

        /// <summary>
        ///  ضمانت ها
        /// </summary>
        public List<CollateralInfoModel> Collaterals { get; set; } = new();



		public string DocumentWizardDataJson { get; set; } = string.Empty;

	}
}
