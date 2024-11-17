using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.SellerViewModels
{
    public class CreateOrEditSellerViewModel
    {
        public int SellerId { get; set; }
        [Display(ResourceType = typeof(Captions), Name = "FullName")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string FullName { get; set; } = string.Empty;


		[Display(ResourceType = typeof(Captions), Name = "Gallery")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public int? GalleryId { get; set; }


        [Display(ResourceType = typeof(Captions), Name = "Mobile")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(11, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [PersianMobileNumber(ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string Mobile { get; set; } = string.Empty;


		[Display(ResourceType = typeof(Captions), Name = "HasAccessToRegisterLoan")]
        public bool HasAccessToRegisterLoan { get; set; }



        [Display(ResourceType = typeof(Captions), Name = "Status")]
        public bool IsActive { get; set; }



        [Display(ResourceType = typeof(Captions), Name = "UserName")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string UserName { get; set; } = string.Empty;


        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Captions), Name = "Password")]
        [MinLength(6, ErrorMessageResourceName = "MinLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? Password { get; set; }


        [Display(ResourceType = typeof(Captions), Name = "ConfirmPassword")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessageResourceName = "NotMatch", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? ConfirmPassword { get; set; }


        [Display(ResourceType = typeof(Captions), Name = "ProductRegisterPerHourCount")]
        [Range(1, byte.MaxValue, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Numberic(null, ErrorMessageResourceName = "Numberic", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public byte? ProductRegisterPerHourCount { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "Image")]
        [FileChecker(".png|.jpg|.gif|.jpeg","image/png|image/jpeg|image/gif", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        [FileSizeChecker(0, 500, ErrorMessageResourceName = "FileSizeChecker", ErrorMessageResourceType = typeof(ValidationMessages))]
        public IFormFile? Image { get; set; }
        //[Display(ResourceType = typeof(Captions), Name = "ImageName")]
        //[MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? ImageNameUrl { get; set; } = string.Empty;
        public string? ImageName { get; set; }

        public List<SelectListItem> Galleries { get; set; } = new List<SelectListItem>();

        public bool IsDeleteImage { get; set; } = false;

        /// <summary>
        /// امکان افزودن محصول توسط فروشنده
        /// </summary>
         [Display(ResourceType = typeof(Captions), Name = "HasAccessToRegisterProduct")]
        public bool HasAccessToRegisterProduct { get; set; } = false;
    }
}
