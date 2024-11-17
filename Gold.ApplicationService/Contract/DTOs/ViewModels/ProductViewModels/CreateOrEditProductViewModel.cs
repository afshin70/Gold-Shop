using Gold.Domain.Enums;
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

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.ProductViewModels
{
    public class CreateOrEditProductViewModel
    {
        public long? ProductId { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "Title")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
       // [RegularExpression("/^[\\u0600-\\u06FF\\s]+$/", ErrorMessageResourceName = nameof(ValidationMessages.OnlyInputPersianChars), ErrorMessageResourceType = typeof(ValidationMessages))]
        public string?  ProductTitle { get; set; }

        /// <summary>
        /// وزن به گرم
        /// </summary>
        [Display(ResourceType = typeof(Captions), Name = "Weight")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [FloatNumberic(".",3, ErrorMessageResourceName = "Numberic", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(6, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        //[RegularExpression()]
       public string? Weight { get; set; }
      
        /// <summary>
        /// عیار طلا بین 1 تا 24
        /// </summary>
        [Display(ResourceType = typeof(Captions), Name = "Karat")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        //[Numberic(null, ErrorMessageResourceName = "Numberic", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Range(1,24, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(ValidationMessages))]
        public byte? Karat { get; set; }
        
        /// <summary>
        /// اجرت بین 1 تا 500
        /// </summary>
        [Display(ResourceType = typeof(Captions), Name = "Wage")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        //[Numberic(null, ErrorMessageResourceName = "Numberic", ErrorMessageResourceType = typeof(ValidationMessages))]
        //[MaxLength(3, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Range(1.00, 500.00, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(ValidationMessages))]
        public decimal? Wage { get; set; }
        
        /// <summary>
        /// سود گالری عدد صحیح بین صفر تا حداکثر تعیین شده در مدیریت
        /// </summary>
        [Display(ResourceType = typeof(Captions), Name = "GalleryProfit")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [FloatNumberic(".", 3, ErrorMessageResourceName = "Numberic", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(6, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Range(0.0, 500, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? GalleryProfit { get; set; }
       
        /// <summary>
        /// مالیات بین 1 تا 100
        /// </summary>
        //[Display(ResourceType = typeof(Captions), Name = "Tax")]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        //[Numberic(null, ErrorMessageResourceName = "Numberic", ErrorMessageResourceType = typeof(ValidationMessages))]
        //[MaxLength(3, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        //public string? Tax { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "Status")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public ProductStatus? Status { get; set; }

        public string StatusStr { get { return Status.ToString(); } }

        [Display(ResourceType = typeof(Captions), Name = "Description")]
       // [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? Description { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "StoneValue")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Numberic(",", ErrorMessageResourceName = "Numberic", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(13, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? StonPrice { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "Gallery")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public int? GalleryId { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "Category")]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public List<int>? CategoryIds { get; set; }

        public int? RegistrarUserId { get; set; }
        public UserType? UserType { get; set; }
        public string? GalleryName { get; set; }
    }
}
