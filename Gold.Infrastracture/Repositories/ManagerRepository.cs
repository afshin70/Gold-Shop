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
    public class ManagerRepository : IManagerRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public ManagerRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            _logManager = logManager;
        }
        public CommandResult Delete(Manager entity)
        {
            try
            {
                _context.Managers.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<User>.FailureInUpdateData();

            }
        }

        public CommandResult<IQueryable<Manager>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.Managers.AsQueryable();
                return CommandResult<IQueryable<Manager>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<Manager>>.FailureInRetrivingData(null);
            }
        }

        public async Task<CommandResult<Manager>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.Managers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
                if (item is null)
                {
                    return CommandResult<Manager>.Failure(DBOperationMessages.DataWasNotFound, item);
                }
                else
                {
                    return CommandResult<Manager>.Success(DBOperationMessages.TheDataWasFetchedCorrectly, item);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Manager>.FailureInRetrivingData();
            }
        }

        public async Task<CommandResult<Manager>> InsertAsync(Manager entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.Managers.AddAsync(entity);
                return CommandResult<Manager>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Manager>.FailureInAddData();
            }
        }

        public CommandResult<Manager> Update(Manager entity)
        {
            try
            {
                _context.Managers.Update(entity);
                return CommandResult<Manager>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<Manager>.FailureInUpdateData();
            }
        }
    }
}
