using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.GoldPriceViewModels
{
    public class GoldPriceViewModel
    {
        [Display(Name = "Shekel", ResourceType = typeof(Captions))]
        [MaxLength(12, ErrorMessageResourceName = "MaxLengthNumber", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MonyFormat(",", ErrorMessageResourceName = "MonyFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? Shekel { get; set; }
        [Display(Name = "GoldAnas", ResourceType = typeof(Captions))]
        //[MonyFormat(",",true, ErrorMessageResourceName = "MonyFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        //[MaxLength(9,ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Range(0.00, 999999.99, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(ValidationMessages))]
        public double? Anas { get; set; }
        [Display(Name = "OneGram18KaratGold", ResourceType = typeof(Captions))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MonyFormat(",", ErrorMessageResourceName = "MonyFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(12, ErrorMessageResourceName = "MaxLengthNumber", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Numberic(".", ErrorMessageResourceName = "Numberic", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string Karat18 { get; set; }
        [Display(Name = "BankCoin", ResourceType = typeof(Captions))]
        [MaxLength(12, ErrorMessageResourceName = "MaxLengthNumber", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MonyFormat(",", ErrorMessageResourceName = "MonyFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? Coin { get; set; }
        [Display(Name = "OldCoin", ResourceType = typeof(Captions))]
        [MaxLength(13, ErrorMessageResourceName = "MaxLengthNumber", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MonyFormat(",", ErrorMessageResourceName = "MonyFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? OldCoin { get; set; }
        [Display(Name = "BankHalfCoin", ResourceType = typeof(Captions))]
        [MaxLength(12, ErrorMessageResourceName = "MaxLengthNumber", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MonyFormat(",", ErrorMessageResourceName = "MonyFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? HalfCoin { get; set; }
        [Display(Name = "BankQuarterCoin", ResourceType = typeof(Captions))]
        [MaxLength(12, ErrorMessageResourceName = "MaxLengthNumber", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MonyFormat(",", ErrorMessageResourceName = "MonyFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? QuarterCoin { get; set; }
        [Display(Name = "GramCoin", ResourceType = typeof(Captions))]
        [MaxLength(12, ErrorMessageResourceName = "MaxLengthNumber", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MonyFormat(",", ErrorMessageResourceName = "MonyFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? GramCoin { get; set; }
        public int? UserId { get; set; }
    }
}
