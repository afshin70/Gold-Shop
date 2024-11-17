using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Entities;
using Gold.Infrastracture.EFCoreContext;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.EntityFrameworkCore;

namespace Gold.Infrastracture.Repositories
{
    public class ContactUsRepository : IContactUsRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public ContactUsRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            this._logManager = logManager;
        }

        public CommandResult Delete(ContactUs entity)
        {
            try
            {
                _context.ContactUs.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<ContactUs>.FailureInRemoveData();
            }
        }

        public async Task<CommandResult<ContactUs>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.ContactUs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
                if (item is null)
                {
                    return CommandResult<ContactUs>.Failure(DBOperationMessages.DataWasNotFound, item);
                }
                else
                {
                    return CommandResult<ContactUs>.Success(DBOperationMessages.TheDataWasFetchedCorrectly, item);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<ContactUs>.FailureInRetrivingData();
            }
        }
        
        public async Task<CommandResult<ContactUs>> InsertAsync(ContactUs entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.ContactUs.AddAsync(entity, cancellationToken);
                return CommandResult<ContactUs>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<ContactUs>.Failure(DBOperationMessages.AnErrorOccurredWhileAddingData, entity);
            }
        }

        public CommandResult<ContactUs> Update(ContactUs entity)
        {
            try
            {
                _context.ContactUs.Update(entity);
                return CommandResult<ContactUs>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<ContactUs>.Failure(DBOperationMessages.AnErrorOccurredWhileEditingData, entity);
            }
        }

        public CommandResult<IQueryable<ContactUs>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.ContactUs.AsQueryable();
                return CommandResult<IQueryable<ContactUs>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<ContactUs>>.FailureInRetrivingData(null);
            }
        }
    }
}
