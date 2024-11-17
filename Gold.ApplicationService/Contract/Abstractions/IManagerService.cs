using Gold.ApplicationService.Contract.DTOs.Models.ManagerModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.ManagerViewModels;
using Gold.SharedKernel.DTO.OperationResult;

namespace Gold.ApplicationService.Contract.Abstractions
{
    public interface IManagerService
    {
        Task<CommandResult<CreateOrEditManagerUserViewModel>> AddOrEditManagerUserAsync(CreateOrEditManagerUserViewModel model, CancellationToken cancellationToken);
        CommandResult<IQueryable<ManagerUserModel>> GetManagerListAsQuerable();
        CommandResult<IQueryable<ManagerUserReportModel>> GetManagerReportListAsQuerable();
        Task<CommandResult<CreateOrEditManagerUserViewModel>> GetManagerUserForEditAsync(int id, CancellationToken cancellationToken);
        Task<CommandResult<string>> RemoveManagerAsync(int id, CancellationToken cancellationToken);
    }
}