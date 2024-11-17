using Gold.SharedKernel.Enums;

namespace Gold.ApplicationService.Contract.DTOs.Models.DocumentModels
{
    public class InstallmentPaymentReminderModel
    {
        public GenderType? Gender { get; set; }
        public string FullName { get; set; }
        public int DocumentNumber { get; set; }
        public DateTime DocumentDate { get; set; }
        public int InstallmentCount { get; set; }
        public long InstallmentAmount { get; set; }
        public byte InstallmentDay { get; set; }
        public byte InstallmentNumber { get; set; }
        public int TotalDelayDays { get; set; }
        public long InstallmentId { get; set; }
        public long DocumentId { get; set; }
        public int CustomerId { get; set; }
    }
                   
}
