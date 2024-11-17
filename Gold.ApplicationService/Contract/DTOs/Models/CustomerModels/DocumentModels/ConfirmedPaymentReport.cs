namespace Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.DocumentModels
{
    public class ConfirmedPaymentReport
    {
        public long Id { get; set; }
        public int DocumentNumber { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PersianInstallmentDate { get; set; } = string.Empty;
		public long InstallmentAmount { get; set; }
        public int InstallmentNumber { get; set; }
        public string PersianPaymentDate { get; set; } = string.Empty;
		public long PaymentAmount { get; set; }
        public string ImageName { get; set; } = string.Empty;
	}
}
