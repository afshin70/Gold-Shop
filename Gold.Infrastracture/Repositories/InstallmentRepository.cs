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
    public class InstallmentRepository : IInstallmentRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public InstallmentRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            this._logManager = logManager;
        }

        public async Task<CommandResult<Installment>> InsertAsync(Installment entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.Installments.AddAsync(entity);
                return CommandResult<Installment>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Installment>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public CommandResult<Installment> Update(Installment entity)
        {
            try
            {
                _context.Installments.Update(entity);
                return CommandResult<Installment>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<Installment>.FailureInUpdateData();
            }
        }

        public CommandResult Delete(Installment entity)
        {
            try
            {
                _context.Installments.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<Installment>.FailureInRemoveData();
            }
        }

        public async Task<CommandResult<Installment>> GetByIdAsync(long id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.Installments.FirstOrDefaultAsync(x => x.Id == id);
                if (item is null)
                    return CommandResult<Installment>.Failure(DBOperationMessages.DataWasNotFound, item);
                else
                    return CommandResult<Installment>.Success(DBOperationMessages.DataFoundedCorrectly, item);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Installment>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }
        
        public async Task<CommandResult<Installment>> GetByIdWithCustomerMessageAsync(long id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.Installments.Include(x=>x.CustomerMessages).FirstOrDefaultAsync(x => x.Id == id);
                if (item is null)
                    return CommandResult<Installment>.Failure(DBOperationMessages.DataWasNotFound, item);
                else
                    return CommandResult<Installment>.Success(DBOperationMessages.DataFoundedCorrectly, item);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Installment>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }
        public async Task<CommandResult<Installment>> GetFirstNotPayedAsync(long documentId,CancellationToken cancellationToken=default)
        {
            try
            {
                var item = await _context.Installments
                    .Where(x=>x.DocumentId==documentId&x.IsPaid==false)
                    .OrderBy(x => x.Number)
                    .FirstOrDefaultAsync(cancellationToken);
                if (item is null)
                    return CommandResult<Installment>.Failure(DBOperationMessages.DataWasNotFound, item);
                else
                    return CommandResult<Installment>.Success(DBOperationMessages.DataFoundedCorrectly, item);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Installment>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public CommandResult<IQueryable<Installment>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.Installments.AsQueryable();
                return CommandResult<IQueryable<Installment>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<Installment>>.FailureInRetrivingData(null);
            }
        }
    }
}
