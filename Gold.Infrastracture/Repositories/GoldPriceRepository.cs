using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Entities;
using Gold.Infrastracture.EFCoreContext;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.EntityFrameworkCore;

namespace Gold.Infrastracture.Repositories
{
    public class GoldPriceRepository : IGoldPriceRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public GoldPriceRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            this._logManager = logManager;
        }

        public CommandResult Delete(GoldPrice entity)
        {
            try
            {
                _context.GoldPrices.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<GoldPrice>.FailureInRemoveData();
            }
        }

        public async Task<CommandResult<GoldPrice>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.GoldPrices.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
                if (item is null)
                {
                    return CommandResult<GoldPrice>.Failure(DBOperationMessages.DataWasNotFound, item);
                }
                else
                {
                    return CommandResult<GoldPrice>.Success(DBOperationMessages.TheDataWasFetchedCorrectly, item);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<GoldPrice>.FailureInRetrivingData();
            }
        }
        
        public async Task<CommandResult<GoldPrice>> InsertAsync(GoldPrice entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.GoldPrices.AddAsync(entity, cancellationToken);
                return CommandResult<GoldPrice>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<GoldPrice>.Failure(DBOperationMessages.AnErrorOccurredWhileAddingData, entity);
            }
        }

        public CommandResult<GoldPrice> Update(GoldPrice entity)
        {
            try
            {
                _context.GoldPrices.Update(entity);
                return CommandResult<GoldPrice>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<GoldPrice>.Failure(DBOperationMessages.AnErrorOccurredWhileEditingData, entity);
            }
        }

        public CommandResult<IQueryable<GoldPrice>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.GoldPrices.Include(x=>x.User).AsQueryable();
                return CommandResult<IQueryable<GoldPrice>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<GoldPrice>>.FailureInRetrivingData(null);
            }
        }
    }
}
