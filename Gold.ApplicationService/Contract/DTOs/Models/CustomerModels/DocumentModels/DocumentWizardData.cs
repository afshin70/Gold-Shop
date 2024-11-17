namespace Gold.ApplicationService.Contract.DTOs.Models.DocumentModels
{
    public class DocumentWizardData
    {
        #region Customer info
        public int? CustomerId { get; set; }
        public string? NationalCode { get; set; }
        public string? FullName { get; set; }
        public string? Mobile { get; set; }
        public string? EssentialTel { get; set; }
        public string? EssentialTelRatio { get; set; }
        #endregion

        #region Document info
        public int? DocumentNumber { get; set; }
        public string? DocumentDate { get; set; }
        #endregion


        #region Loan info
        /// <summary>
        /// مانده
        /// </summary>
        public int? RemainAmount { get; set; }
        /// <summary>
        /// پیش پرداخت
        /// </summary>
        public int? PrePaymentAmount { get; }
        /// <summary>
        /// مبلغ اقساط
        /// </summary>
        public int? InstallmentAmount { get; }
        /// <summary>
        /// گالری
        /// </summary>
        public int? GalleryId { get; set; }
        /// <summary>
        /// فروشنده
        /// </summary>
        public int? SellerId { get; set; }
        /// <summary>
        /// تعداد اقساط
        /// </summary>
        public byte? InstallmentCount { get; set; }
        /// <summary>
        /// توضیحات برای مدیران
        /// </summary>
        public string? AdminDescription { get; set; }
        #endregion

        #region Guarantee info

        #endregion
    }

}
