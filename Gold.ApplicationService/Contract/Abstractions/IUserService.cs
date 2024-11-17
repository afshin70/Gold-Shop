using Gold.ApplicationService.Contract.DTOs.Models.AccessLevelModels;
using Gold.ApplicationService.Contract.DTOs.Models.UserModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.AccessLevelViewModels;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gold.ApplicationService.Contract.Abstractions
{
    public interface IUserService
    {
        CommandResult<IQueryable<AccessLevelModel>> GetAccessLevelListAsQuerable();
        Task<CommandResult> ChangePasswordAsync(string username, string newPassword, CancellationToken cancellationToken);
  
        Task<CommandResult<string>> GetUserNameByIdAsync(int userId, CancellationToken cancellationToken);
        Task<CommandResult> VerifyResetPasswordCode(string username, string code);
        Task<CommandResult<List<int>>> GetManagerPermissionsByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<CommandResult<CreateOrEditAccessLevelViewModel>> GetAccessLevelForEditAsync(int permissionId, CancellationToken cancellationToken);
        Task<CommandResult<CreateOrEditAccessLevelViewModel>> CreateOrEditAccessLevelAsync(CreateOrEditAccessLevelViewModel model, CancellationToken cancellationToken);
        Task<CommandResult<string>> RemoveAccessLevelAsync(byte id, CancellationToken cancellationToken);
        Task<CommandResult<List<SelectListItem>>> GetAccessLevelAsSelectListItemAsync(int selectedId=0, CancellationToken cancellationToken=default);
        CommandResult<IQueryable<EditInformationRequestModel>> EditInformationRequestListAsQuerable(bool? isActive);
        Task<CommandResult> RemoveEditInformationRequestAsync(int id, CancellationToken cancellationToken);
        Task<CommandResult> ArchiveEditInformationRequestAsync(int id, CancellationToken cancellationToken);
        Task<CommandResult<List<SelectListItem>>> GetUsersAsSelectListByUserTypeAsync(int selected = 0, UserType userType = default, CancellationToken cancellationToken = default);
        Task<CommandResult<List<SelectListItem>>> GetAdminMenusAsSelectListByUserTypeAsync(byte selected = 0, CancellationToken cancellationToken = default);
        Task<CommandResult> LogUserActivityAsync(LogUserActivityModel model, CancellationToken cancellationToken = default,bool saveLog=true);
        CommandResult<IQueryable<AccessLevelReportModel>> GetAccessLevelReportListAsQuerable();
        Task<CommandResult<bool>> IsExistUserByUserNameAsync(string? nationalCode, CancellationToken cancellationToken);
        Task<CommandResult<List<SelectListItem>>> GetUsersFullNameAsSelectListByUserTypeAsync(int selected = 0, UserType userType = 0, CancellationToken cancellationToken = default);
    }
}