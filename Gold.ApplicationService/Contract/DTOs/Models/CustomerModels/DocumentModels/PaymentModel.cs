namespace Gold.ApplicationService.Contract.DTOs.Models.DocumentModels
{
    public class PaymentModel
    {
        public long PaymentId { get; set; }
        public long InstallmentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime RegisterDate { get; set; }
        public long Amount { get; set; }
        public string? ImageName { get; set; }
        public string? ImageUrl { get; set; }

    }
}
