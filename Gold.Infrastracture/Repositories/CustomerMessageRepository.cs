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
    public class CustomerMessageRepository : ICustomerMessageRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public CustomerMessageRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            this._logManager = logManager;
        }

        public async Task<CommandResult<CustomerMessage>> InsertAsync(CustomerMessage entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.CustomerMessages.AddAsync(entity);
                return CommandResult<CustomerMessage>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CustomerMessage>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public CommandResult<CustomerMessage> Update(CustomerMessage entity)
        {
            try
            {
                _context.CustomerMessages.Update(entity);
                return CommandResult<CustomerMessage>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<CustomerMessage>.FailureInUpdateData();
            }
        }

        public CommandResult Delete(CustomerMessage entity)
        {
            try
            {
                _context.CustomerMessages.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<CustomerMessage>.FailureInRemoveData();
            }
        }

        public async Task<CommandResult<CustomerMessage>> GetByIdAsync(long id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.CustomerMessages.FirstOrDefaultAsync(x => x.Id == id);
                if (item is null)
                    return CommandResult<CustomerMessage>.Failure(DBOperationMessages.DataWasNotFound, item);
                else
                    return CommandResult<CustomerMessage>.Success("", item);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CustomerMessage>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public CommandResult<IQueryable<CustomerMessage>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.CustomerMessages.AsQueryable();
                return CommandResult<IQueryable<CustomerMessage>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<CustomerMessage>>.FailureInRetrivingData(null);
            }
        }
    }
}
