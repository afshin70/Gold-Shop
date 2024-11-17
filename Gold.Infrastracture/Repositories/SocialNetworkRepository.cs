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
    public class SocialNetworkRepository : ISocialNetworkRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public SocialNetworkRepository(ApplicationContext applicationContext, ILogManager logManager)
        {
            _context = applicationContext;
            this._logManager = logManager;
        }

        public async Task<CommandResult<SocialNetwork>> InsertAsync(SocialNetwork entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.SocialNetworks.AddAsync(entity, cancellationToken);
                return CommandResult<SocialNetwork>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<SocialNetwork>.FailureInAddData();
            }
        }



        public CommandResult<SocialNetwork> Update(SocialNetwork entity)
        {
            try
            {
                _context.SocialNetworks.Update(entity);
                return CommandResult<SocialNetwork>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<SocialNetwork>.FailureInUpdateData();
            }
        }
        public CommandResult Delete(SocialNetwork entity)
        {
            try
            {
                _context.SocialNetworks.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<SocialNetwork>.FailureInRemoveData();

            }
        }

        public async Task<CommandResult<SocialNetwork>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.SocialNetworks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
                if (item is null)
                {
                    return CommandResult<SocialNetwork>.Failure(DBOperationMessages.DataWasNotFound, item);
                }
                else
                {
                    return CommandResult<SocialNetwork>.Success(DBOperationMessages.TheDataWasFetchedCorrectly, item);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<SocialNetwork>.FailureInRetrivingData();
            }
        }


        public CommandResult<IQueryable<SocialNetwork>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.SocialNetworks.AsQueryable();
                return CommandResult<IQueryable<SocialNetwork>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<SocialNetwork>>.FailureInRetrivingData(null);
            }
        }
    }
}
