using Gold.ApplicationService.Contract.DTOs.Models.DocumentModels;
using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.DocumentViewModels
{
    public class CreateOrEditCollateralViewModel
    {
        public long CollateralId { get; set; }
        public long DocumentId { get; set; }
        public int? DocumentNo { get; set; }
        [Display(Name = "Image", ResourceType = typeof(Captions))]
        [FileChecker(".png|.jpg|.gif|.jpeg", "image/png|image/jpeg|image/gif", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        [FileSizeChecker(0, 3000, ErrorMessageResourceName = "FileSizeChecker", ErrorMessageResourceType = typeof(ValidationMessages))]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Captions))]
        public string? Description { get; set; }
        [Display(Name = "CollateralType", ResourceType = typeof(Captions))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public int? CollateralTypeId { get; set; }

        public string? CollateralTypeTitle { get; set; }

        public string? ImageUrl { get; set; }
        public string? ImageName { get; set; }
        public bool IsDeleteImage { get; set; }
        public List<SelectListItem> CollateralTypes { get; set; } = new List<SelectListItem>();
        public List<CollateralModel> Collaterals { get; set; } = new();
    }
}
