using Gold.SharedKernel.Enums;

namespace Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.CustomerPymentsModel
{
    public class CustomerPaymentModel
    {
        public string ImageName { get; set; } = string.Empty;
		public string PersianDate { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string? AdminDescription { get; set; }

        public string? PaymentDate { get; set; }
        public long? PaymentAmount { get; set; }
        public ConfirmStatusType ConfirmType { get; set; }
    }


}
