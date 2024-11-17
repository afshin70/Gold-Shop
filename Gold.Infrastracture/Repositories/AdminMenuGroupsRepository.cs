using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Entities;
using Gold.Infrastracture.EFCoreContext;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.EntityFrameworkCore;

namespace Gold.Infrastracture.Repositories
{
    public class AdminMenuGroupsRepository : IAdminMenuGroupsRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public AdminMenuGroupsRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            _logManager = logManager;
        }
        public CommandResult Delete(AdminMenuGroup entity)
        {
            try
            {
                _context.AdminMenuGroups.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<AdminMenuGroup>.FailureInUpdateData();

            }
        }

        public CommandResult<IQueryable<AdminMenuGroup>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.AdminMenuGroups.AsQueryable();
                return CommandResult<IQueryable<AdminMenuGroup>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<AdminMenuGroup>>.FailureInRetrivingData(null);
            }
        }

        public async Task<CommandResult<AdminMenuGroup>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.AdminMenuGroups.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
                if (item is null)
                {
                    return CommandResult<AdminMenuGroup>.Failure(DBOperationMessages.DataWasNotFound, item);
                }
                else
                {
                    return CommandResult<AdminMenuGroup>.Success(DBOperationMessages.TheDataWasFetchedCorrectly, item);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<AdminMenuGroup>.FailureInRetrivingData();
            }
        }

        public async Task<CommandResult<AdminMenuGroup>> InsertAsync(AdminMenuGroup entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.AdminMenuGroups.AddAsync(entity);
                return CommandResult<AdminMenuGroup>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<AdminMenuGroup>.FailureInAddData();
            }
        }

        public CommandResult<AdminMenuGroup> Update(AdminMenuGroup entity)
        {
            try
            {
                _context.AdminMenuGroups.Update(entity);
                return CommandResult<AdminMenuGroup>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<AdminMenuGroup>.FailureInUpdateData();
            }
        }
    }
}
