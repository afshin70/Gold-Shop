using Gold.Resources;
using Gold.SharedKernel.Enums;
using System.ComponentModel.DataAnnotations;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.CustomerPaymentsViewModel
{
	public class RejectionCustomerPaymentViewModel
	{
		public long CustomerPaymentId { get; set; }
		public int DocumentNumber { get; set; }
		//public string InstallmentDate { get; set; }
		//public byte InstallmentCount { get; set; }
		//public int InstallmentNumber { get; set; }
		public long InstallmentAmount { get; set; }
		public string? CustomerDescription { get; set; }
		public string? RegisterPaymentDateTime { get; set; }

		[Display(Name = "Description", ResourceType = typeof(Captions))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
		public string? Description { get; set; }

		public ConfirmStatusType	StatusType { get; set; }
	}
}
