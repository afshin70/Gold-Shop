using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels;
using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using System.ComponentModel.DataAnnotations;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels
{
	public class CardNumberViewModel
	{
		public long CardNumberId { get; set; }

		public int CustomerId { get; set; } = 0;
		
		[Display(ResourceType = typeof(Captions), Name = "CardNumberOwner")]
		[MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
		public string? Owner { get; set; } = string.Empty;
		
		[Display(ResourceType = typeof(Captions), Name = "CardNumber")]
		[MinLength(19,ErrorMessageResourceName = "BankCardNumberMinLength", ErrorMessageResourceType = typeof(ValidationMessages))]
		[MaxLength(19,ErrorMessageResourceName = "BankCardNumberMaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
		[BankCartNumber('-',ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
		public string? CardNumber { get; set; } = string.Empty;
		
		public int OrderNo { get; set; } = 0;


		public List<CardNumberModel> CardNumbers { get; set; } = new();

	}
}
