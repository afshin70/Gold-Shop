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
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public PaymentRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            this._logManager = logManager;
        }

        public async Task<CommandResult<Payment>> InsertAsync(Payment entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.Payments.AddAsync(entity);
                return CommandResult<Payment>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Payment>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public CommandResult<Payment> Update(Payment entity)
        {
            try
            {
                _context.Payments.Update(entity);
                return CommandResult<Payment>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<Payment>.FailureInUpdateData();
            }
        }

        public CommandResult Delete(Payment entity)
        {
            try
            {
                _context.Payments.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<Payment>.FailureInRemoveData();
            }
        }

        public async Task<CommandResult<Payment>> GetByIdAsync(long id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.Payments.FirstOrDefaultAsync(x => x.Id == id);
                if (item is null)
                    return CommandResult<Payment>.Failure(DBOperationMessages.DataWasNotFound, item);
                else
                    return CommandResult<Payment>.Success("", item);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Payment>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public CommandResult<IQueryable<Payment>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.Payments.AsQueryable();
                return CommandResult<IQueryable<Payment>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<Payment>>.FailureInRetrivingData(null);
            }
        }
    }
}
