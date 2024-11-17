namespace Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.CustomerPaymentsViewModel
{
    public class CustomerPaymentInfoViewModel
    {
        public int DocumentNumber { get; set; }
        public DateTime InstallmentDate { get; set; }
        public DateTime? CustomerPayedTime { get; set; }
        public DateTime CustomerPaymentRedgisterDate { get; set; }
        public int InstallmentNumber { get; set; }
        public byte InstallmentCount { get; set; }
        public long InstallmentAmount { get; set; }
        /// <summary>
        /// وضعیت حساب تا این قسط
        /// </summary>
        public long SumOfRemainAmount { get; set; }
		public string? CustomerDescription { get; set; }


	}
}
