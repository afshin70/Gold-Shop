using Gold.Domain.Contract.DTOs.UserModels;
using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Entities;
using Gold.Domain.Entities.AuthEntities;
using Gold.Domain.Entities.Base;
using Gold.Infrastracture.EFCoreContext;
using Gold.Infrastracture.LogSystem;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gold.Infrastracture.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public UserRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            _logManager = logManager;
        }

        public CommandResult Delete(User entity)
        {
            try
            {
                _context.Users.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<User>.FailureInUpdateData();

            }
        }

        public async Task<CommandResult<User>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
                if (item is null)
                {
                    return CommandResult<User>.Failure(DBOperationMessages.DataWasNotFound, item);
                }
                else
                {
                    return CommandResult<User>.Success(DBOperationMessages.TheDataWasFetchedCorrectly, item);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<User>.FailureInRetrivingData();
            }
        }

        public async Task<CommandResult<Customer>> GetCustomerInfoAsync(int customerId, CancellationToken cancellationToken)
        {
            try
            {
                var customer = await _context.Customers
                    .Where(x => x.Id == customerId)
                    .Include(x => x.User)
                    .Include(x => x.EssentialTels)
                    .FirstOrDefaultAsync();

                if (customer is null)
                {
                    return CommandResult<Customer>.Failure(DBOperationMessages.DataWasNotFound, customer);
                }
                else
                {
                    return CommandResult<Customer>.Success(DBOperationMessages.TheDataWasFetchedCorrectly, customer);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Customer>.FailureInRetrivingData();
            }
        }

        public async Task<CommandResult<User>> GetUserByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName.ToLower() == username.ToLower());
                if (user is null)
                {
                    return CommandResult<User>.Failure(DBOperationMessages.DataWasNotFound, null);
                }
                else
                {
                    return CommandResult<User>.Success(DBOperationMessages.TheDataWasFetchedCorrectly, user);
                }
            }
            catch (Exception ex)
            {
                //throw new Exception(ExceptionErrorMessages.ArgumentNullException, ex);
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<User>.FailureInRetrivingData();
            }

        }


        public Task<CommandResult<UserLoginInfoModel>> GetUserLoginInfoByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<CommandResult<UserResetCodeAndEmailInfoModel>> GetUserResetCodeAndEmailInfoAsync(string username)
        {
            throw new NotImplementedException();
        }


        public async Task<CommandResult<User>> InsertAsync(User entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.Users.AddAsync(entity);
                return CommandResult<User>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<User>.FailureInAddData();
            }
        }

        public async Task<CommandResult<List<User>>> UpdateRange(List<User> entities)
        {
            try
            {
                _context.Users.UpdateRange(entities);
                return CommandResult<List<User>>.Success(DBOperationMessages.DataEditedCorrectly, entities);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<List<User>>.FailureInUpdateData();
            }
        }

        public async Task<CommandResult> IsValidResetPasswordCodeAsync(string username, string code)
        {
            if (true)
            {
                //valid username and code
                return CommandResult.Success("");
            }
            else
            {
                //invalid username and password
                return CommandResult.Failure("");
            }
        }

        public CommandResult<User> Update(User entity)
        {
            try
            {
                _context.Users.Update(entity);
                return CommandResult<User>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<User>.FailureInUpdateData();
            }
        }

       


        public async Task<CommandResult> UpdateWrongPasswordCountAsync(string userName, int count, CancellationToken cancellationToken)
        {
            try
            {
                var user=await _context.Users.FirstOrDefaultAsync(x=>x.UserName==userName,cancellationToken);
                user.WrongPasswordCount = count;
                _context.Users.Update(user);
                return CommandResult.Success(DBOperationMessages.DataEditedCorrectly);
            }
            catch (Exception ex)
            {
              await  _logManager.RaiseLogAsync(ex,cancellationToken);
                return CommandResult<User>.FailureInUpdateData();
            }
        }

        public CommandResult<IQueryable<User>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.Users.AsQueryable();
                return CommandResult<IQueryable<User>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<User>>.FailureInRetrivingData(null);
            }
        }

        public async Task<CommandResult<User>> InsertCustomerUserAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                await _context.AddAsync(user, cancellationToken);
                return CommandResult<User>.Success(DBOperationMessages.DataAddedCorrectly, user);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<User>.FailureInAddData();
            }
        }

        public async Task<CommandResult<User>> GetUserByCustomerIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var customerUserId = await _context.Customers.Where(x => x.Id == id).Select(x => x.UserId).FirstOrDefaultAsync();
                if (customerUserId > 0)
                {
                    var user = await _context.Users.Include(x => x.Customer).FirstOrDefaultAsync(x => x.Id == customerUserId);
                    if (user is null)
                    {
                        return CommandResult<User>.Failure(DBOperationMessages.DataWasNotFound, user);
                    }
                    else
                    {
                        return CommandResult<User>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, user);
                    }
                }
                else
                {
                    return CommandResult<User>.Failure(DBOperationMessages.DataWasNotFound, null);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<User>.FailureInRetrivingData(null);
            }
        }

        public async Task<CommandResult<string>> GetUserNameByIdAsync(int userId, CancellationToken cancellationToken)
        {
            try
            {
                string? userName = await _context.Users
                    .Where(x => x.Id == userId)
                    .Select(x => x.UserName)
                    .FirstOrDefaultAsync(cancellationToken);
                if (string.IsNullOrEmpty(userName))
                {
                    return CommandResult<string>.Failure(DBOperationMessages.DataWasNotFound, string.Empty);
                }
                else
                {
                    return CommandResult<string>.Success(DBOperationMessages.DataFoundedCorrectly, userName);
                }

            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<string>.FailureInRetrivingData(string.Empty);
            }
        }

        public async Task<CommandResult> ValidateSecurityStampByUserNameAsync(string userName, Guid securityStamp)
        {
            try
            {
                if (await _context.Users.AnyAsync(x=>x.UserName==userName&x.SecurityStamp==securityStamp))
                    return CommandResult.Success(DBOperationMessages.DataFoundedCorrectly);
                else
                    return CommandResult.Failure(DBOperationMessages.DataWasNotFound);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<string>.FailureInRetrivingData(string.Empty);
            }
        }
        
        public  CommandResult ValidateSecurityStampByUserName(string userName, Guid securityStamp)
        {
            try
            {
                if (_context.Users.Any(x=>x.UserName==userName&x.SecurityStamp==securityStamp))
                    return CommandResult.Success(DBOperationMessages.DataFoundedCorrectly);
                else
                    return CommandResult.Failure(DBOperationMessages.DataWasNotFound);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<string>.FailureInRetrivingData(string.Empty);
            }
        }


        public async Task<CommandResult<User>> GetManagerUserAsync(int userId, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.Users
                    .Where(x => x.Id == userId)
                    .Include(x=>x.Manager)
                    .FirstOrDefaultAsync(cancellationToken);
                if (item is null)
                {
                    return CommandResult<User>.Failure(DBOperationMessages.DataWasNotFound, item);
                }
                else
                {
                    return CommandResult<User>.Success(DBOperationMessages.TheDataWasFetchedCorrectly, item);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<User>.FailureInRetrivingData();
            }
        }
    }
}
