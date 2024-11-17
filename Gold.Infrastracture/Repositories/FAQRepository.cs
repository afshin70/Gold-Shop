using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Entities;
using Gold.Infrastracture.EFCoreContext;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.EntityFrameworkCore;

namespace Gold.Infrastracture.Repositories
{
    public class FAQRepository : IFAQRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public FAQRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            this._logManager = logManager;
        }

        public CommandResult Delete(FAQ entity)
        {
            try
            {
                _context.FAQs.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<FAQ>.FailureInRemoveData();
            }
        }

        public async Task<CommandResult<FAQ>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.FAQs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
                if (item is null)
                {
                    return CommandResult<FAQ>.Failure(DBOperationMessages.DataWasNotFound, item);
                }
                else
                {
                    return CommandResult<FAQ>.Success(DBOperationMessages.TheDataWasFetchedCorrectly, item);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<FAQ>.FailureInRetrivingData();
            }
        }

        public async Task<CommandResult<FAQ>> InsertAsync(FAQ entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.FAQs.AddAsync(entity, cancellationToken);
                return CommandResult<FAQ>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<FAQ>.Failure(DBOperationMessages.AnErrorOccurredWhileAddingData, entity);
            }
        }

        public CommandResult<FAQ> Update(FAQ entity)
        {
            try
            {
                _context.FAQs.Update(entity);
                return CommandResult<FAQ>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<FAQ>.Failure(DBOperationMessages.AnErrorOccurredWhileEditingData, entity);
            }
        }



        public CommandResult<IQueryable<FAQ>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.FAQs.AsQueryable();
                return CommandResult<IQueryable<FAQ>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<FAQ>>.FailureInRetrivingData(null);
            }
        }
    }
}
