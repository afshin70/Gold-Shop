using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.DocumentViewModels
{
    public class DocumentBasicInformationViewModel
    {
        [Display(ResourceType = typeof(Captions), Name = "DocumentNumber")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Range(1,int.MaxValue, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(ValidationMessages))]
        public int DocumentNumber { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "Date")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [PersianDate("/",ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string DocumentDate { get; set; } = string.Empty;
		//public DateTime DocumentDate { get; set; }

		[Display(ResourceType = typeof(Captions), Name = "NationalCode")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [PersianNationalCode(ErrorMessageResourceName = "PersianNationalCode", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string NationalCode { get; set; } = string.Empty;



		public string? DocumentWizardDataJson { get; set; }
    }
}
