using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Entities;
using Gold.Infrastracture.EFCoreContext;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.EntityFrameworkCore;

namespace Gold.Infrastracture.Repositories
{
    public class FavoritProductRepository : IFavoritProductRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public FavoritProductRepository(ApplicationContext applicationContext,ILogManager logManager)
        {
            _context=applicationContext;
            this._logManager = logManager;
        }

        public async Task<CommandResult<FavoritProduct>> InsertAsync(FavoritProduct entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.FavoritProducts.AddAsync(entity);
                return CommandResult<FavoritProduct>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<FavoritProduct>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public CommandResult<FavoritProduct> Update(FavoritProduct entity)
        {
            try
            {
                _context.FavoritProducts.Update(entity);
                return CommandResult<FavoritProduct>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<FavoritProduct>.FailureInUpdateData();
            }
        }

        public CommandResult Delete(FavoritProduct entity)
        {
            try
            {
                _context.FavoritProducts.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<Seller>.FailureInRemoveData();
            }
        }

        public async Task<CommandResult<FavoritProduct>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.FavoritProducts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (item is null)
                    return CommandResult<FavoritProduct>.Failure(DBOperationMessages.DataWasNotFound, item);
                else
                    return CommandResult<FavoritProduct>.Success(DBOperationMessages.DataFoundedCorrectly, item);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<FavoritProduct>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public CommandResult<IQueryable<FavoritProduct>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.FavoritProducts.AsQueryable();
                return CommandResult<IQueryable<FavoritProduct>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<FavoritProduct>>.FailureInRetrivingData(null);
            }
        }
    }
}
