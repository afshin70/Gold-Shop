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
    public class CustomerPaymentRepository : ICustomerPaymentRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public CustomerPaymentRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            this._logManager = logManager;
        }

        public async Task<CommandResult<CustomerPayment>> InsertAsync(CustomerPayment entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.CustomerPayments.AddAsync(entity);
                return CommandResult<CustomerPayment>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CustomerPayment>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public CommandResult<CustomerPayment> Update(CustomerPayment entity)
        {
            try
            {
                _context.CustomerPayments.Update(entity);
                return CommandResult<CustomerPayment>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<CustomerPayment>.FailureInUpdateData();
            }
        }

        public CommandResult Delete(CustomerPayment entity)
        {
            try
            {
                _context.CustomerPayments.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<CustomerPayment>.FailureInRemoveData();
            }
        }

        public async Task<CommandResult<CustomerPayment>> GetByIdAsync(long id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.CustomerPayments.FirstOrDefaultAsync(x => x.Id == id);
                if (item is null)
                    return CommandResult<CustomerPayment>.Failure(DBOperationMessages.DataWasNotFound, item);
                else
                    return CommandResult<CustomerPayment>.Success("", item);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CustomerPayment>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public CommandResult<IQueryable<CustomerPayment>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.CustomerPayments.AsQueryable();
                return CommandResult<IQueryable<CustomerPayment>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<CustomerPayment>>.FailureInRetrivingData(null);
            }
        }
    }
}
