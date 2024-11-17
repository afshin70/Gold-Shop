using Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels;
using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using Gold.SharedKernel.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Gold.ApplicationService.Contract.DTOs.Models.CustomerModels
{
    public class CreateOrEditCustomerViewModel
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Captions), Name = "FullName")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string FullName { get; set; } = string.Empty;

		[Display(ResourceType = typeof(Captions), Name = "FatherName")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? FatherName { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "Nationality")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public NationalityType? Nationality { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "Gender")]
        public GenderType? Gender { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "JobTitle")]
		[MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
		public string? JobTitle { get; set; }

		[Display(ResourceType = typeof(Captions), Name = "BirthDate")]
		[MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [PersianDate("/",ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
		public string? BirthDate { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "Status")]
        public bool IsActive { get; set; }


        [Display(ResourceType = typeof(Captions), Name = "City")]
        public int? CityId { get; set; }


        [Display(ResourceType = typeof(Captions), Name = "NationalCode_UserName")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessageResourceName = "OnlyAlphanumeric", ErrorMessageResourceType = typeof(ValidationMessages))]
		[MinLength(3, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MinLength))]
		// [PersianNationalCode(ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(ValidationMessages))]
		public string NationalCode { get; set; } = string.Empty;


		[Display(ResourceType = typeof(Captions), Name = "SanaCode")]
        [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? SanaCode { get; set; }


        [Display(ResourceType = typeof(Captions), Name = "Mobile_Password")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(11, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [PersianMobileNumber(ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string Mobile { get; set; } = string.Empty;


		[Display(ResourceType = typeof(Captions), Name = "PostalCode")]
        [MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MinLength(10, ErrorMessageResourceName = "MinLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Numberic(null, ErrorMessageResourceName = "Numberic", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? PostalCode { get; set; }


        [Display(ResourceType = typeof(Captions), Name = "Address")]
        [MaxLength(500, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? Address { get; set; }

        public EssentialTelViewModel? EssentialTels { get; set; }


		[Display(ResourceType = typeof(Captions), Name = "CardNumberOwner")]
		[MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
		public string? CardNumberOwner { get; set; } = string.Empty;

		[Display(ResourceType = typeof(Captions), Name = "CardNumber")]
		[MaxLength(19, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
		[BankCartNumber('-', ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
		public string? CardNumber { get; set; } = string.Empty;
		// public CardNumberViewModel? CardNumber { get; set; }

		public List<SelectListItem> Proviances { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> Cities { get; set; } = new List<SelectListItem>();
    }
}
