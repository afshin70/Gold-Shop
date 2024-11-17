namespace Gold.ApplicationService.Contract.DTOs.Models.DocumentModels
{
    public class InstallmentPymentsDetailModel
    {
        public long InstallmentId { get; set; }
        public int DocumentNumber { get; set; }
        public DateTime InstallmentDate { get; set; }
        public int InstallmentNumber { get; set; }
        public int InstallmentCount { get; set; }
        public long InstallmentAmount { get; set; }
        public long SumOfRemainAmount { get; set; }
        public IEnumerable<PaymentModel> Payments { get; set; } =new List<PaymentModel>();
    }
}
