namespace Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels
{
    public class EditCustomerSummaryInfoWithEssentialTelAndCardNoViewModel
    {
        public EditCustomerSummaryInfoViewModel EditCustomerSummaryInfo { get; set; }=new();
        public EssentialTelViewModel EssentialTel { get; set; } = new();
        public CardNumberViewModel CardNumber { get; set; } = new();
    }
}
