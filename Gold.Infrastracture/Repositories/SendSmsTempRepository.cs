using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Entities;
using Gold.Infrastracture.EFCoreContext;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.EntityFrameworkCore;

namespace Gold.Infrastracture.Repositories
{
    public class SendSmsTempRepository : ISendSmsTempRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public SendSmsTempRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            this._logManager = logManager;
        }
        public CommandResult Delete(SendSmsTemp entity)
        {
            try
            {
                _context.SmsTemps.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<SendSmsTemp>.FailureInRemoveData();
            }
        }

        public CommandResult DeleteRange(List<SendSmsTemp> entities)
        {
            try
            {
                _context.SmsTemps.RemoveRange(entities);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<SendSmsTemp>.FailureInRemoveData();
            }
        }

        public CommandResult<IQueryable<SendSmsTemp>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.SmsTemps.AsQueryable();
                return CommandResult<IQueryable<SendSmsTemp>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<SendSmsTemp>>.FailureInRetrivingData(null);
            }
        }

        public async Task<CommandResult<SendSmsTemp>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.SmsTemps.FirstOrDefaultAsync(x => x.Id == id);
                if (item is null)
                    return CommandResult<SendSmsTemp>.Failure(DBOperationMessages.DataWasNotFound, item);
                else
                    return CommandResult<SendSmsTemp>.Success("", item);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<SendSmsTemp>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }


        public async Task<CommandResult<SendSmsTemp>> InsertAsync(SendSmsTemp entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.SmsTemps.AddAsync(entity);
                return CommandResult<SendSmsTemp>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<SendSmsTemp>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public CommandResult<SendSmsTemp> Update(SendSmsTemp entity)
        {
            try
            {
                _context.SmsTemps.Update(entity);
                return CommandResult<SendSmsTemp>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<SendSmsTemp>.FailureInUpdateData();
            }
        }
    }
}
