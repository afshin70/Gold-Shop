using Gold.Resources;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;
using System.Xml.Linq;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.DocumentViewModels
{
    public class CollateralViewModel
    {
        [Display(Name = "Image", ResourceType = typeof(Captions))]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Captions))]
        public string? Description { get; set; }
        [Display(Name = "CollateralType", ResourceType = typeof(Captions))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public int CollateralTypeId { get; set; }
    }
}
