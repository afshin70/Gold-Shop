using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Entities;
using Gold.Infrastracture.EFCoreContext;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.EntityFrameworkCore;

namespace Gold.Infrastracture.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public PermissionRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            _logManager = logManager;
        }
        public CommandResult Delete(Permission entity)
        {
            try
            {
                _context.Permissions.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<Permission>.FailureInUpdateData();

            }
        }

        public CommandResult<IQueryable<Permission>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.Permissions.AsQueryable();
                return CommandResult<IQueryable<Permission>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<Permission>>.FailureInRetrivingData(null);
            }
        }

        public async Task<CommandResult<Permission>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.Permissions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
                if (item is null)
                {
                    return CommandResult<Permission>.Failure(DBOperationMessages.DataWasNotFound, item);
                }
                else
                {
                    return CommandResult<Permission>.Success(DBOperationMessages.TheDataWasFetchedCorrectly, item);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Permission>.FailureInRetrivingData();
            }
        }

        public async Task<CommandResult<Permission>> InsertAsync(Permission entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.Permissions.AddAsync(entity);
                return CommandResult<Permission>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Permission>.FailureInAddData();
            }
        }

        public CommandResult<Permission> Update(Permission entity)
        {
            try
            {
                _context.Permissions.Update(entity);
                return CommandResult<Permission>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<Permission>.FailureInUpdateData();
            }
        }
    }
}
