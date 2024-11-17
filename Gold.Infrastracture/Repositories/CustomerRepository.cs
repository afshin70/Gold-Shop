using Gold.Domain.Contract.DTOs.UserModels.CustomerModels;
using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Contract.IRepositories.Base;
using Gold.Domain.Entities;
using Gold.Domain.Entities.AuthEntities;
using Gold.Domain.Entities.Base;
using Gold.Infrastracture.EFCoreContext;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gold.Infrastracture.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public CustomerRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            this._logManager = logManager;
        }

        public async Task<CommandResult<Customer>> InsertAsync(Customer entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.Customers.AddAsync(entity);
                return CommandResult<Customer>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Customer>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public CommandResult<Customer> Update(Customer entity)
        {
            try
            {
                _context.Customers.Update(entity);
                return CommandResult<Customer>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<Customer>.FailureInUpdateData();
            }
        }

        public CommandResult Delete(Customer entity)
        {
            try
            {
                _context.Customers.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<Customer>.FailureInRemoveData();
            }
        }

        public async Task<CommandResult<Customer>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.Customers.FirstOrDefaultAsync(x => x.Id == id);
                if (item is null)
                    return CommandResult<Customer>.Failure(DBOperationMessages.DataWasNotFound, item);
                else
                    return CommandResult<Customer>.Success("", item);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Customer>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public CommandResult<IQueryable<Customer>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.Customers.AsQueryable();
                return CommandResult<IQueryable<Customer>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<Customer>>.FailureInRetrivingData(null);
            }
        }

        public CommandResult<IQueryable<Customer>> GetAllAsIQueryable(bool includeUser)
        {
            try
            {
                IQueryable<Customer> customerIquerable;
                if (includeUser)
                {
                    customerIquerable = _context.Customers.Include(x => x.User).AsQueryable();
                }
                customerIquerable = _context.Customers.Include(x => x.User).AsQueryable();
                return CommandResult<IQueryable<Customer>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, customerIquerable);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<Customer>>.FailureInRetrivingData(null);
            }
        }

        public async Task<CommandResult<bool>> IsExistAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                if (await _context.Customers.AnyAsync(x => x.Id == id, cancellationToken))
                    return CommandResult<bool>.Success(DBOperationMessages.DataFoundedCorrectly, true);
                else
                    return CommandResult<bool>.Failure(DBOperationMessages.DataWasNotFound, false);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<bool>.FailureInRetrivingData(false);
            }
        }

        public async Task<CommandResult<int>> GetUserIdAsync(int customerId, CancellationToken cancellationToken)
        {
            try
            {
                int userId= await _context.Customers.Where(x => x.Id == customerId).Select(x => x.UserId).FirstOrDefaultAsync();
                if (userId>0)
                    return CommandResult<int>.Success(DBOperationMessages.DataFoundedCorrectly, userId);
                else
                    return CommandResult<int>.Failure(DBOperationMessages.DataWasNotFound, userId);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex,cancellationToken);
                return CommandResult<int>.FailureInRetrivingData(0);
            }
        }

        public async Task<CommandResult<bool>> IsExistByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken)
        {
            try
            {
                bool isExist = await _context.Customers.AnyAsync(x => x.NationalCode == nationalCode);
                if (isExist)
                    return CommandResult<bool>.Success(DBOperationMessages.DataFoundedCorrectly, true);
                else
                    return CommandResult<bool>.Failure(DBOperationMessages.DataWasNotFound, false);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<bool>.FailureInRetrivingData(false);
            }
        }

        public async Task<CommandResult<bool>> IsActiveByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken)
        {
            try
            {
                var userId = await _context.Customers.Where(x => x.NationalCode == nationalCode).Select(x=>x.UserId).FirstOrDefaultAsync();
                if (await _context.Users.AnyAsync(x=>x.Id== userId&x.IsActive&x.UserType==SharedKernel.Enums.UserType.Customer))
                {
                    return CommandResult<bool>.Success(DBOperationMessages.DataFoundedCorrectly, true);
                }
                else
                {
                    return CommandResult<bool>.Failure(DBOperationMessages.DataWasNotFound, false);
                }
                    
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<bool>.FailureInRetrivingData(false);
            }
        }

        public async  Task<CommandResult<CustomerMobileAndFullName>> GetCustomerNameAndMobileByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken)
        {
            try
            {
                var customerInfo = await _context.Customers
                    .Include(u=>u.User)
                    .Where(x => x.NationalCode == nationalCode)
                    .Select(x => new CustomerMobileAndFullName
                    {
                        CustomerId=x.Id,
                        NationalCode=x.NationalCode,
                        FullName=x.User.FullName,
                        Mobile=x.User.Mobile
                    }).FirstOrDefaultAsync();

                if (customerInfo is not null)
                {
                    return CommandResult<CustomerMobileAndFullName>.Success(DBOperationMessages.DataFoundedCorrectly, customerInfo);
                }
                else
                {
                    return CommandResult<CustomerMobileAndFullName>.Failure(DBOperationMessages.DataWasNotFound, null);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CustomerMobileAndFullName>.FailureInRetrivingData(null);
            }
        }

        public async Task<CommandResult<int>> GetCustomerIdByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken)
        {
            try
            {
                int customerId=await _context.Customers.Where(x=>x.NationalCode== nationalCode).Select(x=>x.Id).FirstOrDefaultAsync();
                if (customerId>0)
                {
                    return CommandResult<int>.Success(DBOperationMessages.DataFoundedCorrectly, customerId);
                }
                else
                {
                    return CommandResult<int>.Failure(DBOperationMessages.DataWasNotFound, customerId);
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<int>.FailureInRetrivingData(0);
            }
        }
    }
}
