using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using Gold.SharedKernel.Enums;
using System.ComponentModel.DataAnnotations;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.DocumentViewModels
{
    public class CreateDocumentSecondStepViewModel
    {
        [Display(Name = "FullName", ResourceType = typeof(Captions))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? FullName { get; set; }

        [Display(Name = "Gender", ResourceType = typeof(Captions))]
        public GenderType? Gender { get; set; }

        [Display(Name = "FatherName", ResourceType = typeof(Captions))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? FatherName { get; set; }


        [Display(Name = "Mobile", ResourceType = typeof(Captions))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(11, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [PersianMobileNumber(ErrorMessageResourceName = "PersianMobileNumber", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string Mobile { get; set; } = string.Empty;

        [Display(Name = "EssentialTel", ResourceType = typeof(Captions))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? EssentialTel { get; set; }

        [Display(Name = "EssentialTelRatio", ResourceType = typeof(Captions))]
        [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? EssentialTelRatio { get; set; }

        [Display(Name = "BirthDate", ResourceType = typeof(Captions))]
        [PersianDate("/", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? BirthDate { get; set; }

        [Display(Name = "JobTitle", ResourceType = typeof(Captions))]
        [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? JobTitle { get; set; }


        [Display(Name = "PostalCode", ResourceType = typeof(Captions))]
        [MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MinLength(10, ErrorMessageResourceName = "MinLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Numberic(null, ErrorMessageResourceName = "Numberic", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? PostalCode { get; set; }

        [Display(Name = "Address", ResourceType = typeof(Captions))]
        [MaxLength(500, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? Address { get; set; }

        [Display(Name = "City", ResourceType = typeof(Captions))]
        public int? CityId { get; set; }
    }
}
