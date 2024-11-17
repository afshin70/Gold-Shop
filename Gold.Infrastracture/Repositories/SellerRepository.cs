using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Contract.IRepositories.Base;
using Gold.Domain.Entities;
using Gold.Domain.Entities.Base;
using Gold.Infrastracture.EFCoreContext;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace Gold.Infrastracture.Repositories
{
    public class SellerRepository : ISellerRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public SellerRepository(ApplicationContext applicationContext,ILogManager logManager)
        {
            _context=applicationContext;
            this._logManager = logManager;
        }

        public async Task<CommandResult<Seller>> InsertAsync(Seller entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.Sellers.AddAsync(entity);
                return CommandResult<Seller>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Seller>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public CommandResult<Seller> Update(Seller entity)
        {
            try
            {
                _context.Sellers.Update(entity);
                return CommandResult<Seller>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<Seller>.FailureInUpdateData();
            }
        }

        public CommandResult Delete(Seller entity)
        {
            try
            {
                _context.Sellers.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<Seller>.FailureInRemoveData();
            }
        }

        public async Task<CommandResult<Seller>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.Sellers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (item is null)
                    return CommandResult<Seller>.Failure(DBOperationMessages.DataWasNotFound, item);
                else
                    return CommandResult<Seller>.Success(DBOperationMessages.DataFoundedCorrectly, item);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Seller>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public CommandResult<IQueryable<Seller>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.Sellers.AsQueryable();
                return CommandResult<IQueryable<Seller>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<Seller>>.FailureInRetrivingData(null);
            }
        }

        public async Task<CommandResult<List<Seller>>> GetAllByGalleryIdAsync(int galleryId, CancellationToken cancellationToken)
        {
            try
            {
                var list =await _context.Sellers.Where(x=>x.GalleryId==galleryId).Include(c=>c.User).ToListAsync();
                return CommandResult<List<Seller>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, list);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<List<Seller>>.FailureInRetrivingData(null);
            }
        }
    }
}
