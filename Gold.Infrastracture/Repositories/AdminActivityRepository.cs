using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Entities;
using Gold.Infrastracture.EFCoreContext;
using Gold.Infrastracture.LogSystem;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.EntityFrameworkCore;

namespace Gold.Infrastracture.Repositories
{
    public class AdminActivityRepository : IAdminActivityRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public AdminActivityRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            this._logManager = logManager;
        }

        public async Task<CommandResult<AdminActivity>> InsertAsync(AdminActivity entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.AdminActivities.AddAsync(entity);
                return CommandResult<AdminActivity>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<AdminActivity>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public CommandResult<AdminActivity> Update(AdminActivity entity)
        {
            try
            {
                _context.AdminActivities.Update(entity);
                return CommandResult<AdminActivity>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<AdminActivity>.FailureInUpdateData();
            }
        }

        public CommandResult Delete(AdminActivity entity)
        {
            try
            {
                _context.AdminActivities.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<AdminActivity>.FailureInRemoveData();
            }
        }

        public async Task<CommandResult<AdminActivity>> GetByIdAsync(long id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.AdminActivities.FirstOrDefaultAsync(x => x.Id == id);
                if (item is null)
                    return CommandResult<AdminActivity>.Failure(DBOperationMessages.DataWasNotFound, item);
                else
                    return CommandResult<AdminActivity>.Success("", item);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<AdminActivity>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public CommandResult<IQueryable<AdminActivity>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.AdminActivities.AsQueryable();
                return CommandResult<IQueryable<AdminActivity>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<AdminActivity>>.FailureInRetrivingData(null);
            }
        }
    }
}
