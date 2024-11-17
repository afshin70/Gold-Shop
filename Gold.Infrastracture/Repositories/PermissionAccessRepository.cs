using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Entities;
using Gold.Domain.Entities.AuthEntities;
using Gold.Infrastracture.EFCoreContext;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.EntityFrameworkCore;

namespace Gold.Infrastracture.Repositories
{
    public class PermissionAccessRepository : IPermissionAccessRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public PermissionAccessRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            _logManager = logManager;
        }
        public CommandResult Delete(PermissionAccess entity)
        {
            try
            {
                _context.PermissionAccesses.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<User>.FailureInUpdateData();

            }
        } 
        
        public CommandResult DeleteRange(List<PermissionAccess> entities)
        {
            try
            {
                _context.PermissionAccesses.RemoveRange(entities);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<User>.FailureInUpdateData();

            }
        }

        public CommandResult<IQueryable<PermissionAccess>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.PermissionAccesses.AsQueryable();
                return CommandResult<IQueryable<PermissionAccess>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<PermissionAccess>>.FailureInRetrivingData(null);
            }
        }

        public async Task<CommandResult<PermissionAccess>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.PermissionAccesses.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
                if (item is null)
                {
                    return CommandResult<PermissionAccess>.Failure(DBOperationMessages.DataWasNotFound, item);
                }
                else
                {
                    return CommandResult<PermissionAccess>.Success(DBOperationMessages.TheDataWasFetchedCorrectly, item);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<PermissionAccess>.FailureInRetrivingData();
            }
        }

        public async Task<CommandResult<PermissionAccess>> InsertAsync(PermissionAccess entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.PermissionAccesses.AddAsync(entity);
                return CommandResult<PermissionAccess>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<PermissionAccess>.FailureInAddData();
            }
        } 
        
        public async Task<CommandResult<List<PermissionAccess>>> InsertRangeAsync(List<PermissionAccess> entities, CancellationToken cancellationToken)
        {
            try
            {
                await _context.PermissionAccesses.AddRangeAsync(entities);
                return CommandResult<List<PermissionAccess>>.Success(DBOperationMessages.DataAddedCorrectly, entities);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<PermissionAccess>>.FailureInAddData();
            }
        }

        public CommandResult<PermissionAccess> Update(PermissionAccess entity)
        {
            try
            {
                _context.PermissionAccesses.Update(entity);
                return CommandResult<PermissionAccess>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<PermissionAccess>.FailureInUpdateData();
            }
        }
    }
}
