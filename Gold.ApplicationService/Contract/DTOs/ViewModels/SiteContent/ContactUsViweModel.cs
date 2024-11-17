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

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.SiteContent
{
    public class ContactUsViweModel
    {
        [Display(ResourceType = typeof(Captions), Name = "FullName")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string FullName { get; set; }

        [Display(Name = nameof(Captions.PhoneNumber), ResourceType = typeof(Captions))]
        [MaxLength(11, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxLength))]
        [MinLength(11, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MinLength))]
        [RegularExpression("^(\\+98|0098|98|0)?9\\d{9}$", ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Invalid))]
        public string? Phone { get; set; } = string.Empty;


        [Display(ResourceType = typeof(Captions), Name = "Message")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(500, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string Message { get; set; }

        [Display(Name = nameof(Captions.SecurityCode), ResourceType = typeof(Captions))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Required))]
        [MaxLength(6, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxLength))]
        [MinLength(6, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MinLength))]
        [Numberic(null, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Numberic))]
        public string Captcha { get; set; }
    }
}
