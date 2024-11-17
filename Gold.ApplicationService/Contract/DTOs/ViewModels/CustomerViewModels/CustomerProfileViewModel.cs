using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels
{

	public class CustomerProfileViewModel
	{
        [Display(ResourceType = typeof(Captions), Name = "City")]
        public int? CityId { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "Province")]
        public int? ProvinceId { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "PostalCode")]
        [MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MinLength(10, ErrorMessageResourceName = "MinLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Numberic(null, ErrorMessageResourceName = "Numberic", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? PostalCode { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "Address")]
        [MaxLength(500, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? Address { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "SanaCode")]
        [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? SanaCode { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "JobTitle")]
        [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? JobTitle { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "BirthDate")]
        [MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [PersianDate("/", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? BirthDate { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "ProfileImage")]
        [FileChecker(".png|.jpg|.jpeg", "image/png|image/jpeg", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        [FileSizeChecker(0, 5125, ErrorMessageResourceName = "FileSizeChecker", ErrorMessageResourceType = typeof(ValidationMessages))]
        public IFormFile? ProfileImage { get; set; }
    }
}
