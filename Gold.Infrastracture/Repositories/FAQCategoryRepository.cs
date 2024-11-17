using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Entities;
using Gold.Infrastracture.EFCoreContext;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.EntityFrameworkCore;

namespace Gold.Infrastracture.Repositories
{
    public class FAQCategoryRepository : IFAQCategoryRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public FAQCategoryRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            this._logManager = logManager;
        }

        public CommandResult Delete(FAQCategory entity)
        {
            try
            {
                _context.FAQCategories.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                 _logManager.RaiseLog(ex);
                return CommandResult<FAQCategory>.FailureInRemoveData();
            }
        }

        public async Task<CommandResult<FAQCategory>> GetByIdAsync(int id,CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.FAQCategories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
                if (item is null)
                {
                    return CommandResult<FAQCategory>.Failure(DBOperationMessages.DataWasNotFound, item);
                }
                else
                {
                    return CommandResult<FAQCategory>.Success(DBOperationMessages.TheDataWasFetchedCorrectly, item);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<FAQCategory>.FailureInRetrivingData();
            }
        }

        public async Task<CommandResult<FAQCategory>> InsertAsync(FAQCategory entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.FAQCategories.AddAsync(entity, cancellationToken);
                return CommandResult<FAQCategory>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<FAQCategory>.Failure(DBOperationMessages.AnErrorOccurredWhileAddingData, entity);
            }
        }

        public CommandResult<FAQCategory> Update(FAQCategory entity)
        {
            try
            {
                _context.FAQCategories.Update(entity);
                return CommandResult<FAQCategory>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                 _logManager.RaiseLog(ex);
                return CommandResult<FAQCategory>.Failure(DBOperationMessages.AnErrorOccurredWhileEditingData, entity);
            }
        }

       

        public CommandResult<IQueryable<FAQCategory>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.FAQCategories.AsQueryable();
                return CommandResult<IQueryable<FAQCategory>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<FAQCategory>>.FailureInRetrivingData(null);
            }
        }
    }
}
