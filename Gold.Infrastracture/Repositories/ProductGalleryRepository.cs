using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Entities;
using Gold.Infrastracture.EFCoreContext;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.EntityFrameworkCore;

namespace Gold.Infrastracture.Repositories
{
    public class ProductGalleryRepository : IProductGalleryRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public ProductGalleryRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            this._logManager = logManager;
        }

        public CommandResult Delete(ProductGallery entity)
        {
            try
            {
                _context.ProductGallery.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                 _logManager.RaiseLog(ex);
                return CommandResult<ProductGallery>.FailureInRemoveData();
            }
        }
        
        public CommandResult DeleteRange(List<ProductGallery> entities)
        {
            try
            {
                _context.ProductGallery.RemoveRange(entities);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                 _logManager.RaiseLog(ex);
                return CommandResult<ProductGallery>.FailureInRemoveData();
            }
        }

        public async Task<CommandResult<ProductGallery>> GetByIdAsync(long id,CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.ProductGallery.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
                if (item is null)
                {
                    return CommandResult<ProductGallery>.Failure(DBOperationMessages.DataWasNotFound, item);
                }
                else
                {
                    return CommandResult<ProductGallery>.Success(DBOperationMessages.TheDataWasFetchedCorrectly, item);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<ProductGallery>.FailureInRetrivingData();
            }
        }
      
        public async Task<CommandResult<ProductGallery>> InsertAsync(ProductGallery entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.ProductGallery.AddAsync(entity, cancellationToken);
                return CommandResult<ProductGallery>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<ProductGallery>.Failure(DBOperationMessages.AnErrorOccurredWhileAddingData, entity);
            }
        }

        public CommandResult<ProductGallery> Update(ProductGallery entity)
        {
            try
            {
                _context.ProductGallery.Update(entity);
                return CommandResult<ProductGallery>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                 _logManager.RaiseLog(ex);
                return CommandResult<ProductGallery>.Failure(DBOperationMessages.AnErrorOccurredWhileEditingData, entity);
            }
        }
        public CommandResult<List<ProductGallery>> UpdateRange(List<ProductGallery> entities)
        {
            try
            {
                _context.ProductGallery.UpdateRange(entities);
                return CommandResult<List<ProductGallery>>.Success(DBOperationMessages.DataEditedCorrectly, entities);
            }
            catch (Exception ex)
            {
                 _logManager.RaiseLog(ex);
                return CommandResult<List<ProductGallery>>.Failure(DBOperationMessages.AnErrorOccurredWhileEditingData, entities);
            }
        }

      

        public CommandResult<IQueryable<ProductGallery>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.ProductGallery.AsQueryable();
                return CommandResult<IQueryable<ProductGallery>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<ProductGallery>>.FailureInRetrivingData(null);
            }
        }

    }
}
