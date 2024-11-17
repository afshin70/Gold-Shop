using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels;
using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using System.ComponentModel.DataAnnotations;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels
{
	public class EssentialTelViewModel
	{
		public long EssentialTelId { get; set; }

		public int CustomerId { get; set; } = 0;
		[Display(ResourceType = typeof(Captions), Name = "EssentialTel")]
		[MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
		[Numberic(null, ErrorMessageResourceName = "Numberic", ErrorMessageResourceType = typeof(ValidationMessages))]
		public string Tel { get; set; } = string.Empty;


		[Display(ResourceType = typeof(Captions), Name = "EssentialTelRatio")]
		[MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
		public string? RelationShip { get; set; }


		public int OrderNo { get; set; } = 0;


		public List<EssentialTelModel> EssentialTels { get; set; } = new();

	}
}
