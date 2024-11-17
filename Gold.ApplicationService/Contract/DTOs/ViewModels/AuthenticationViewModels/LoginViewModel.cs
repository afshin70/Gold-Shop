using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using Gold.SharedKernel.ExtentionMethods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.AuthenticationViewModels
{
    public class LoginViewModel
    {
        [Display(Name = nameof(Captions.UserName), ResourceType = typeof(Captions))]
        [MaxLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxLength))]
        [MinLength(3, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MinLength))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Required))]
        public string UserName { get; set; } = string.Empty;
		[Display(Name = nameof(Captions.Password), ResourceType = typeof(Captions))]
        [MaxLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxLength))]
        [MinLength(6, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MinLength))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Required))]
        public string Password { get; set; } = string.Empty;
		//public string Password { get
		//    {
		//        return Password.ToEnNumber();
		//    } 
		//    set { }
		//}
		[Display(Name = nameof(Captions.RememberMe), ResourceType = typeof(Captions))]
        public bool RememberMe { get; set; }

        [Display(Name = nameof(Captions.SecurityCode), ResourceType = typeof(Captions))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Required))]
        [MaxLength(6, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxLength))]
        [MinLength(6, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MinLength))]
        [Numberic(null, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Numberic))]
        public string Captcha { get; set; } = string.Empty;
	}
}
