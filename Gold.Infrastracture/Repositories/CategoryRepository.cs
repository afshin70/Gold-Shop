using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Entities;
using Gold.Infrastracture.EFCoreContext;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.EntityFrameworkCore;

namespace Gold.Infrastracture.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public CategoryRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            this._logManager = logManager;
        }

        public CommandResult Delete(Category entity)
        {
            try
            {
                _context.Categories.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                 _logManager.RaiseLog(ex);
                return CommandResult<Category>.FailureInRemoveData();
            }
        }

        public async Task<CommandResult<Category>> GetByIdAsync(int id,CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
                if (item is null)
                {
                    return CommandResult<Category>.Failure(DBOperationMessages.DataWasNotFound, item);
                }
                else
                {
                    return CommandResult<Category>.Success(DBOperationMessages.TheDataWasFetchedCorrectly, item);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Category>.FailureInRetrivingData();
            }
        }
      
        public async Task<CommandResult<Category>> InsertAsync(Category entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.Categories.AddAsync(entity, cancellationToken);
                return CommandResult<Category>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Category>.Failure(DBOperationMessages.AnErrorOccurredWhileAddingData, entity);
            }
        }

        public CommandResult<Category> Update(Category entity)
        {
            try
            {
                _context.Categories.Update(entity);
                return CommandResult<Category>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                 _logManager.RaiseLog(ex);
                return CommandResult<Category>.Failure(DBOperationMessages.AnErrorOccurredWhileEditingData, entity);
            }
        }

      

        public CommandResult<IQueryable<Category>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.Categories.AsQueryable();
                return CommandResult<IQueryable<Category>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<Category>>.FailureInRetrivingData(null);
            }
        }

    }
}
