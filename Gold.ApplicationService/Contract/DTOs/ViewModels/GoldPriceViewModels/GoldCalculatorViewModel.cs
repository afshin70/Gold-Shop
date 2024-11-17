using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;
using System.Xml.Linq;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.GoldPriceViewModels
{
    public class GoldCalculatorViewModel
    {
        /// <summary>
        /// قیمت هر گرم طلا
        /// </summary>
        [Display(Name = "GramsGold", ResourceType = typeof(Captions))]
        [MaxLength(12, ErrorMessageResourceName = "MaxLengthNumber", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MonyFormat(",", ErrorMessageResourceName = "MonyFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? GramsGoldPrice { get; set; }

        /// <summary>
        /// وزن طلا
        /// </summary>
        [Display(ResourceType = typeof(Captions), Name = "Weight")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [FloatNumberic(".", 3, ErrorMessageResourceName = "Numberic", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(6, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? Weight { get; set; }

        /// <summary>
        /// اجرت ساخت
        /// </summary>
        [Display(Name = "ConstructionWages", ResourceType = typeof(Captions))]
        [MaxLength(12, ErrorMessageResourceName = "MaxLengthNumber", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MonyFormat(",", ErrorMessageResourceName = "MonyFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? Wage { get; set; }

        /// <summary>
        /// قیمت سنگ
        /// </summary>
        [Display(Name = "StoneValue", ResourceType = typeof(Captions))]
        [MaxLength(12, ErrorMessageResourceName = "MaxLengthNumber", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MonyFormat(",", ErrorMessageResourceName = "MonyFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? StonePrice { get; set; }

        /// <summary>
        /// سود گالری
        /// </summary>
        [Display(ResourceType = typeof(Captions), Name = "GalleryProfit")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [FloatNumberic(".", 3, ErrorMessageResourceName = "Numberic", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(6, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? GalleryProfit { get; set; }

        /// <summary>
        /// مالیات بر ارزش افزوده
        /// </summary>
        [Display(ResourceType = typeof(Captions), Name = "TaxValue")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [FloatNumberic(".", 3, ErrorMessageResourceName = "Numberic", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(6, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? Tax { get; set; }
    }
}
