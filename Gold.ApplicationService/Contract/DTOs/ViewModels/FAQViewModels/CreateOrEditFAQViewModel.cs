using Gold.Resources;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.FAQViewModels
{
    public class CreateOrEditFAQViewModel
    {
        public int? Id { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "Question")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? Question { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "Answer")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? Answer { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "Category")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public int? FaqCategoryId { get; set; }

        public List<SelectListItem>?  Categories { get; set; }
    }
}
