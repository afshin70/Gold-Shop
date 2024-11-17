namespace Gold.ApplicationService.Contract.DTOs.Models.DocumentModels
{
	public class PaymentDescriptionWithMessageModel
    {
        public string Description { get; set; }
        public string Message { get; set; }
    }

    public class GeneratePaymentInfoModel
    {
        public string Description { get; set; }
        public string Message { get; set; }
        public int DelayDay { get; set; }
    }

    public class GeneratePaymentInfoRequestModel
    {
        public string PaymentAmount { get; set; }
        public long InstallmentId { get; set; }
        public long PaymentId { get; set; }
        public int DelayDay { get; set; }
        public string PaymentDate { get; set; }
        public bool IsCalcWithPaymentDate { get; set; }
    }
}
