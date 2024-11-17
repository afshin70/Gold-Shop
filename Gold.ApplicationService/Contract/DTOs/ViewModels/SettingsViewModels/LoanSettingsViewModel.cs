using Gold.Resources;
using Gold.SharedKernel.DTO.Bootstrap;
using Gold.SharedKernel.Attributes.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.SettingsViewModels
{
    public class LoanSettingsViewModel
    {
        /// <summary>
        /// درصد سود ماهیانه
        /// </summary>
        [Display(ResourceType = typeof(Captions), Name = "MonthlyProfitPercentage")]
        //[Range(0, 100, ErrorMessageResourceName = $"Range",ErrorMessageResourceType =typeof(ValidationMessages))]
        //[Range(typeof(decimal), "0.00", "100.00", ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Numberic(".", ErrorMessageResourceName = "Numberic", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(6, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? MonthlyProfitPercentage { get; set; }
        //public decimal? MonthlyProfitPercentage { get; set; }

        /// <summary>
        /// ضریب جریمه
        /// بین صفر تا یک
        /// </summary>
        [Display(ResourceType = typeof(Captions), Name = "PenaltyFactor")]
        //[Range(0, 1, ErrorMessageResourceName = $"Range", ErrorMessageResourceType = typeof(ValidationMessages))]
        //[Range(typeof(decimal), "0", "1", ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Numberic(".", ErrorMessageResourceName = "Numberic", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(6, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? PenaltyFactor { get; set; }
        //public decimal? PenaltyFactor { get; set; }

        /// <summary>
        /// ساعت ارسال پیام یادآوری به کاربران
        /// </summary>
        //[RegularExpression("((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)")]
        [Display(ResourceType = typeof(Captions), Name = "TimeToSendReminderMessagesToUsers")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        //[TimeChecker(ErrorMessageResourceName = "TimeChecker", ErrorMessageResourceType = typeof(ValidationMessages))]
        public TimeSpan? TimeToSendReminderMessagesToUsers { get; set; }

        /// <summary>
        /// درصد سود گالری
        /// </summary>
        [Display(ResourceType = typeof(Captions), Name = "MaxProfitGallery")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Numberic(".", ErrorMessageResourceName = "Numberic", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(6, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? MaxProfitGallery { get; set; }

        /// <summary>
        /// مالیات
        /// </summary>
        [Display(ResourceType = typeof(Captions), Name = "Tax")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Range(0,100, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(ValidationMessages))]
        //[Numberic(".", ErrorMessageResourceName = "Numberic", ErrorMessageResourceType = typeof(ValidationMessages))]
        //[MaxLength(3, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public byte? Tax { get; set; }
        //public string? Tax { get; set; }

    }
}
