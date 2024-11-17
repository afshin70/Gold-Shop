using Gold.Domain.Contract.DTOs.UserModels;
using Gold.Domain.Contract.IRepositories.Base;
using Gold.Domain.Entities;
using Gold.Domain.Entities.AuthEntities;
using Gold.SharedKernel.DTO.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.Domain.Contract.IRepositories
{
    public interface IUserRepository : IBaseRepository<User,int>
    {
        Task<CommandResult<Customer>> GetCustomerInfoAsync(int customerId, CancellationToken cancellationToken);
        Task<CommandResult<User>> GetUserByUsernameAsync(string username, CancellationToken cancellationToken);
        Task<CommandResult<UserLoginInfoModel>> GetUserLoginInfoByUsernameAsync(string username, CancellationToken cancellationToken);
        Task<CommandResult<UserResetCodeAndEmailInfoModel>> GetUserResetCodeAndEmailInfoAsync(string username);
        Task<CommandResult> IsValidResetPasswordCodeAsync(string username, string code);
        Task<CommandResult> UpdateWrongPasswordCountAsync(string userName, int count, CancellationToken cancellationToken);
        Task<CommandResult<User>> InsertCustomerUserAsync(User user, CancellationToken cancellationToken);
        Task<CommandResult<User>> GetUserByCustomerIdAsync(int id, CancellationToken cancellationToken);
        Task<CommandResult<string>> GetUserNameByIdAsync(int userId, CancellationToken cancellationToken);
        Task<CommandResult> ValidateSecurityStampByUserNameAsync(string userName, Guid securityStamp);
        CommandResult ValidateSecurityStampByUserName(string userName, Guid securityStamp);
        Task<CommandResult<User>> GetManagerUserAsync(int userId, CancellationToken cancellationToken);
        Task<CommandResult<List<User>>> UpdateRange(List<User> entities);
    }
}
