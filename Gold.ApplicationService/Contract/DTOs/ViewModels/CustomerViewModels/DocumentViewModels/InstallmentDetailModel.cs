namespace Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.DocumentViewModels
{
    public class InstallmentDetailModel
    {
        public long InstallmentId { get; set; }
        public int DocumentNumber { get; set; }
        public DateTime InstallmentDate { get; set; }
        public int InstallmentNumber { get; set; }
        public int InstallmentCount { get; set; }
        public long InstallmentAmount { get; set; }
        public bool IsPayInstallment { get; set; }
    }
}
