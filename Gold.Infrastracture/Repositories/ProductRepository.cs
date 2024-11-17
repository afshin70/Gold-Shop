using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Entities;
using Gold.Infrastracture.EFCoreContext;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.EntityFrameworkCore;

namespace Gold.Infrastracture.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public ProductRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            this._logManager = logManager;
        }

        public CommandResult Delete(Product entity)
        {
            try
            {
                _context.Products.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                 _logManager.RaiseLog(ex);
                return CommandResult<Product>.FailureInRemoveData();
            }
        }

        public async Task<CommandResult<Product>> GetByIdAsync(long id,CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.Products.AsNoTracking()
                    .Include(x=>x.ProductCategories)
                    .Include(x=>x.Gallery)
                    .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
                if (item is null)
                {
                    return CommandResult<Product>.Failure(DBOperationMessages.DataWasNotFound, item);
                }
                else
                {
                    return CommandResult<Product>.Success(DBOperationMessages.TheDataWasFetchedCorrectly, item);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Product>.FailureInRetrivingData();
            }
        }
      
        public async Task<CommandResult<Product>> InsertAsync(Product entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.Products.AddAsync(entity, cancellationToken);
                return CommandResult<Product>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Product>.Failure(DBOperationMessages.AnErrorOccurredWhileAddingData, entity);
            }
        }

        public CommandResult<Product> Update(Product entity)
        {
            try
            {
                _context.Products.Update(entity);
                return CommandResult<Product>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                 _logManager.RaiseLog(ex);
                return CommandResult<Product>.Failure(DBOperationMessages.AnErrorOccurredWhileEditingData, entity);
            }
        }

      

        public CommandResult<IQueryable<Product>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.Products
                    .Include(x=>x.RegistrarUser)
                    .Include(x=>x.Gallery)
                    .AsQueryable();
                return CommandResult<IQueryable<Product>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<Product>>.FailureInRetrivingData(null);
            }
        }

      
    }
}
