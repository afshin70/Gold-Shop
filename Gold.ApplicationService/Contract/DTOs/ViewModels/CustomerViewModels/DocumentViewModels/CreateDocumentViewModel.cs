using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using Gold.SharedKernel.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.DocumentViewModels
{
    public class CreateDocumentViewModel
    {
        public long? DocumentId { get; set; }

        [Display(Name = "DocumentNumber", ResourceType = typeof(Captions))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public int DocumentNo { get; set; }
        [Display(Name = "DocumentDate", ResourceType = typeof(Captions))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string DocumentDate { get; set; } = string.Empty;
        [Display(Name = "NationalCode_UserName", ResourceType = typeof(Captions))]
        [Required(ErrorMessageResourceName = nameof(ValidationMessages.Required), ErrorMessageResourceType = typeof(ValidationMessages))]
		[MinLength(3, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MinLength))]
		public string NationalCode { get; set; } = string.Empty;
        [Display(ResourceType = typeof(Captions), Name = "Nationality")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public NationalityType? Nationality { get; set; }

        #region Step 2 Data
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



        [Display(ResourceType = typeof(Captions), Name = "CardNumberOwner")]
        [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? CardNumberOwner { get; set; } = string.Empty;

        [Display(ResourceType = typeof(Captions), Name = "CardNumber")]
        [MaxLength(19, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [BankCartNumber('-', ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? CardNumber { get; set; } = string.Empty;
        #endregion

        #region step 3 Data
        [Display(Name = "Gallery", ResourceType = typeof(Captions))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public int GalleryId { get; set; }

        [Display(Name = "Seller", ResourceType = typeof(Captions))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public int SellerId { get; set; }

        [Display(Name = "Prepayment", ResourceType = typeof(Captions))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MonyFormat(",", ErrorMessageResourceName = "MonyFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? PrepaymentAmount { get; set; }

        [Display(Name = "Remain", ResourceType = typeof(Captions))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MonyFormat(",", ErrorMessageResourceName = "MonyFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? RemainAmount { get; set; }

        [Display(Name = "InstallmentCount", ResourceType = typeof(Captions))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Range(1, byte.MaxValue, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(ValidationMessages))]
        public byte InstallmentCount { get; set; }

        [Display(Name = "InstallmentAmount", ResourceType = typeof(Captions))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MonyFormat(",", ErrorMessageResourceName = "MonyFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? InstallmentAmount { get; set; }

        [Display(Name = "AdminDescription", ResourceType = typeof(Captions))]
        public string? AdminDescription { get; set; }
        [Display(Name = "Collaterals", ResourceType = typeof(Captions))]

        public List<CollateralViewModel> Collaterals { get; set; } = new();
        #endregion
        public bool IsNewCstomer { get; set; }


    }
}
