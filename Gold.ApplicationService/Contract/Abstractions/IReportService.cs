using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.DocumentModels;
using Gold.ApplicationService.Contract.DTOs.Models.ReportModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.AuthenticationViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.DocumentViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.ReportViewModels;
using Gold.SharedKernel.DTO.OperationResult;

namespace Gold.ApplicationService.Contract.Abstractions
{
    public interface IReportService
    {
        CommandResult<IQueryable<ManagersOperationReportModel>> GetManagersOperationReport(ManagersOperationReportViewModel model);
        CommandResult<List<AdminMenuModel>> GetManagersOperationReportAdminMenus();
        CommandResult<IQueryable<ConfirmedPaymentReport>> GetPaymentsReport(PaymentReportSearchViewModel model);
        Task<CommandResult<long>> PaymentAmountSumAsync(PaymentReportSearchViewModel model, CancellationToken cancellationToken);
    }
}