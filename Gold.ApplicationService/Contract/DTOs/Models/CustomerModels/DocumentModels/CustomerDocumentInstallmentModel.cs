namespace Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.DocumentModels
{
	public class CustomerDocumentInstallmentModel
	{
        public long InstallmentId { get; set; }
        public string PersianInstallmentDate { get; set; } = string.Empty;
        public string? PersianPaymentDate { get; set; }
        public int?   Delay { get; set; }
        public long PaymentAmount { get; set; }
        //public bool IsPaid { get; set; }
        //public string? Description { get; set; }
        public string CustomerMessage { get; set; } = string.Empty;
        public bool HasDelay { get; set; }
        public string InstallmentStateClass { get; set; }
    }
}
