using Gold.ApplicationService.Contract.DTOs.Models.AuthenticationModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.AuthenticationViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.AuthenticationViewModels.RegisterCustomerViewModels;
using Gold.Domain.Entities.AuthEntities;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.Abstractions
{
    public interface IAuthenticationManager
    {
        Task<CommandResult> ChangePasswordByTokenAsync(ChangePasswordByTokenViewModel model, CancellationToken cancellationToken);
        Task<bool> IsValidUserPasswordAsync(string passwordHash, string passwordSaltKey, string password);
        Task<CommandResult<UserType>> LoginUserAsync(HttpContext httpContext, LoginViewModel model, CancellationToken cancellationToken);
        Task<CommandResult> LogOutUserAsync(HttpContext httpContext, CancellationToken cancellationToken);
        Task<CommandResult<VerifyResetPasswordCodeViewModel>> ProcessForgetPasswordAsync(ForgetPasswordViewModel model, CancellationToken cancellationToken);
       Task< CommandResult<VerifyResetPasswordCodeViewModel>> VerifyResetPasswordOTPCodeAsync(VerifyResetPasswordCodeViewModel model, CancellationToken cancellationToken);
        Task<CommandResult<List<AdminMenuGroupModel>>> GetAdminPanelMenuListAsync(CancellationToken cancellationToken = default);
        Task<CommandResult> ChangePasswordAsync(ChangeUserPasswordModel changeUserPasswordModel, CancellationToken cancellationToken);
        Task<CommandResult<VerifyResetPasswordCodeViewModel>> ResendVerifyResetPasswordOTPCodeAsync(string? userName, string? mobile, CancellationToken cancellationToken);
        Task<CommandResult<TimeSpan>> GetResetPasswordSmsTimerAsync(VerifyResetPasswordCodeViewModel model, CancellationToken cancellationToken);
        Task<CommandResult> ChangeCustomerPasswordAsync(ChangeUserPasswordViewModel model, string userName, CancellationToken cancellationToken);
        Task<CommandResult<TimeSpan>> GetRegisterCustomerSmsTimerAsync(RegisterCustomerVerifyCodeViewModel model, CancellationToken cancellationToken);
        Task<CommandResult<RegisterCustomerVerifyCodeViewModel>> VerifyRegisterCustomerOTPCodeAsync(RegisterCustomerVerifyCodeViewModel model, CancellationToken cancellationToken);
        Task<CommandResult<RegisterCustomerVerifyCodeViewModel>> ResendRegisterCustomerVerifyOTPCodeAsync(NationalityType nationalityType,string? userName, string? mobile, CancellationToken cancellationToken);
        Task<CommandResult<RegisterCustomerVerifyCodeViewModel>> ProcessRegisterCustomerOTPCodeAsync(RegisterCustomerBaseInfoViewModel model, CancellationToken cancellationToken);
    }
}
