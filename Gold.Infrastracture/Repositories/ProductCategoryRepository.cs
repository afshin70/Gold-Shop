using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Entities;
using Gold.Infrastracture.EFCoreContext;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.EntityFrameworkCore;

namespace Gold.Infrastracture.Repositories
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public ProductCategoryRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            this._logManager = logManager;
        }

        public CommandResult Delete(ProductCategory entity)
        {
            try
            {
                _context.ProductCategories.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                 _logManager.RaiseLog(ex);
                return CommandResult<ProductCategory>.FailureInRemoveData();
            }
        }
        
        public CommandResult DeleteRange(List<ProductCategory> entities)
        {
            try
            {
                _context.ProductCategories.RemoveRange(entities);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                 _logManager.RaiseLog(ex);
                return CommandResult<ProductCategory>.FailureInRemoveData();
            }
        }

        public async Task<CommandResult<ProductCategory>> GetByIdAsync(long id,CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.ProductCategories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
                if (item is null)
                {
                    return CommandResult<ProductCategory>.Failure(DBOperationMessages.DataWasNotFound, item);
                }
                else
                {
                    return CommandResult<ProductCategory>.Success(DBOperationMessages.TheDataWasFetchedCorrectly, item);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<ProductCategory>.FailureInRetrivingData();
            }
        }
      
        public async Task<CommandResult<ProductCategory>> InsertAsync(ProductCategory entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.ProductCategories.AddAsync(entity, cancellationToken);
                return CommandResult<ProductCategory>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<ProductCategory>.Failure(DBOperationMessages.AnErrorOccurredWhileAddingData, entity);
            }
        }
        
        public async Task<CommandResult<List<ProductCategory>>> InsertRangeAsync(List<ProductCategory> entities, CancellationToken cancellationToken)
        {
            try
            {
                await _context.ProductCategories.AddRangeAsync(entities, cancellationToken);
                return CommandResult<List<ProductCategory>>.Success(DBOperationMessages.DataAddedCorrectly, entities);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<ProductCategory>>.Failure(DBOperationMessages.AnErrorOccurredWhileAddingData, entities);
            }
        }

        public CommandResult<ProductCategory> Update(ProductCategory entity)
        {
            try
            {
                _context.ProductCategories.Update(entity);
                return CommandResult<ProductCategory>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                 _logManager.RaiseLog(ex);
                return CommandResult<ProductCategory>.Failure(DBOperationMessages.AnErrorOccurredWhileEditingData, entity);
            }
        }

      

        public CommandResult<IQueryable<ProductCategory>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.ProductCategories.AsQueryable();
                return CommandResult<IQueryable<ProductCategory>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<ProductCategory>>.FailureInRetrivingData(null);
            }
        }

    }
}
