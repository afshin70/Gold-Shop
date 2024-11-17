using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.GalleryViewModels
{
    public class CreateOrEditGalleryViewModel
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "Name")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string Name { get; set; } = string.Empty;

        [Display(ResourceType = typeof(Captions), Name = "Address")]
        [MaxLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? Address { get; set; }


        [Display(ResourceType = typeof(Captions), Name = "Tel")]
        [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? Tel { get; set; }
        
        
        [Display(ResourceType = typeof(Captions), Name = "Status")]
        public bool IsActive { get; set; }


        [Display(ResourceType = typeof(Captions), Name = "HasInstallmentSale")]
        public bool HasInstallmentSale { get; set; }


        [Display(ResourceType = typeof(Captions), Name = "PurchaseDescription")]
        [DataType(DataType.Html)]
        public string? PurchaseDescription { get; set; }

    }
}
