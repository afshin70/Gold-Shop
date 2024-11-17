using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using System.ComponentModel.DataAnnotations;

namespace Gold.ApplicationService.Contract.DTOs.Models.ProductModels
{
    public class ProductForShowInSiteModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string ImageName { get; set; }
        public bool IsSold { get; set; }
        public bool IsImage { get; set; }
    }

    public class ProductInfoForShowInSiteModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
       
        /// <summary>
        /// وضعیت علاقه مندی کاربر
        /// </summary>
        public bool IsBookmarked { get; set; }

        /// <summary>
        /// تخفیف =سودگالری - سود گالری محصول
        /// </summary>
        public decimal Discount { get; set; }
        /// <summary>
        /// قیمت طلا=قیمت یک گرم طلای 18 عیار تقسیم بر 18 ضربدر وزن محصول
        /// </summary>
        public long GoldPrice { get; set; }
        /// <summary>
        /// قیمت محصول ضربدر تخفیف منهای قیمت محصول
        /// </summary>
        public long DiscountedPrice { get; set; }
        public string Description { get; set; }
        public bool HasInstallmentSale { get; set; }
        /// <summary>
        /// توضیحات خرید قسطی محصول
        /// </summary>
        public string InstallmentPurchaseOfProduct { get; set; }
        /// <summary>
        /// وزن
        /// </summary>
        public string Weight { get; set; }

        /// <summary>
        /// درصد اجرت
        /// </summary>
        public decimal Wage { get; set; }
        /// <summary>
        /// مبلغ اجرت
        /// </summary>
        public long WageAmount { get; set; }

        /// <summary>
        /// دصد سود گالری
        /// </summary>
        public decimal GalleryProfit { get; set; }
        /// <summary>
        /// مبلغ سود گالری
        /// </summary>
        public long GalleryProfitAmount { get; set; }

        /// <summary>
        /// درصد مالیات
        /// </summary>
        public byte Tax { get; set; }
        
        /// <summary>
        /// مبلغ مالیات
        /// </summary>
        public long TaxAmount { get; set; }

        /// <summary>
        /// مبلغ ارزش سنگ
        /// </summary>
        public long StonePrice { get; set; }

        /// <summary>
        /// قیمت نهایی محصول
        /// </summary>
        public long FinalPrice { get; set; }

        /// <summary>
        /// مبلغ پیش پرداخت خرید قسطی
        /// </summary>

        public long DefaultPrePayment { get; set; }


        public bool HasDiscount { get; set; }
        #region تصاویر و ویدیو های محصول
        public List<ProductGalleryForShowInSiteModel> GalleryFiles { get; set; } = new();
        #endregion

        #region اطلاعات گالری محصول
        public string GalleryTitle { get; set;}
        public string GalleryAddress { get; set; }
        public string GalleryPhone { get; set; }
        public string GalleryDescription { get; set;}

        #region اطلاعات فروشندگان گالری
        public List<GallerySellerForShowInSiteModel> GallerySellers { get; set; } = new();
        #endregion

        #endregion
    }

    public class GallerySellerForShowInSiteModel
    {
        public string Name { get; set; }
        public string ProfileImage { get; set; }
        public string Phone { get; set; }
    }

    public class ProductGalleryForShowInSiteModel
    {
        public long ProductId { get; set; }
        public bool IsImage { get; set; }
        public string FileName { get; set; }
    }

    public class InstallmentPurchaseModel
    {
        /// <summary>
        /// مبلغ نهایی فاکتور
        /// </summary>
        public long InvoiceAmount { get; set; }

        /// <summary>
        /// مبلغ قسط-مبلغ نهایی هر قسط=تعداد ماه/ مبلغ مانده*( درصد سود ماهیانه / 100 ) * تعداد ماه + مبلغ مانده -
        /// </summary>
        public long InstallmentAmount { get; set; }

        /// <summary>
        /// مبلغ مانده -
        /// مبلغ نهایی محصول – مبلغ پیش پرداخت
        /// </summary>
        public long RemainAmount { get; set; }
    }

    public class InstallmentPurchaseInputDataModel
    {
        public long ProductId { get; set; }
        /// <summary>
        /// تعداد اقساط
        /// </summary>
        public byte InstallmentCount { get; set; }
        /// <summary>
        /// پیش پرداخت
        /// </summary>
        public long PrePayment { get; set; }
    }
    public class InstallmentPurchaseInputDataViewModel
    {
        public long ProductId { get; set; }

        /// <summary>
        /// تعداد اقساط
        /// </summary>
        [Display(Name = nameof(Captions.InstallmentCount), ResourceType = typeof(Captions))]
        [Range(3,12, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public byte? InstallmentCount { get; set; }

        /// <summary>
        /// پیش پرداخت
        /// </summary>
        [Display(Name = nameof(Captions.Prepayment), ResourceType = typeof(Captions))]
        [MaxLength(12, ErrorMessageResourceName = "MaxLengthNumber", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MonyFormat(",", ErrorMessageResourceName = "MonyFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string PrePayment { get; set; }
    }
}
