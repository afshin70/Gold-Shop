using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Entities;
using Gold.Infrastracture.EFCoreContext;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.EntityFrameworkCore;

namespace Gold.Infrastracture.Repositories
{
    public class ProfileImageRepository : IProfileImageRepository
    {
		private readonly ApplicationContext _context;
		private readonly ILogManager _logManager;

		public ProfileImageRepository(ApplicationContext applicationContext, ILogManager logManager)
		{
			_context = applicationContext;
			this._logManager = logManager;
		}
		public async Task<CommandResult<ProfileImage>> InsertAsync(ProfileImage entity, CancellationToken cancellationToken)
		{
			try
			{
				await _context.ProfileImages.AddAsync(entity);
				return CommandResult<ProfileImage>.Success(DBOperationMessages.DataAddedCorrectly, entity);
			}
			catch (Exception ex)
			{
				await _logManager.RaiseLogAsync(ex, cancellationToken);
				return CommandResult<ProfileImage>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
			}
		}

		public CommandResult<ProfileImage> Update(ProfileImage entity)
		{
			try
			{
				_context.ProfileImages.Update(entity);
				return CommandResult<ProfileImage>.Success(DBOperationMessages.DataEditedCorrectly, entity);
			}
			catch (Exception ex)
			{
				_logManager.RaiseLog(ex);
				return CommandResult<ProfileImage>.FailureInUpdateData();
			}
		}

		public CommandResult Delete(ProfileImage entity)
		{
			try
			{
				_context.ProfileImages.Remove(entity);
				return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
			}
			catch (Exception ex)
			{
				_logManager.RaiseLog(ex);
				return CommandResult<ProfileImage>.FailureInRemoveData();
			}
		}

		public CommandResult DeleteRange(List<ProfileImage> entities)
		{
			try
			{
				_context.ProfileImages.RemoveRange(entities);
				return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
			}
			catch (Exception ex)
			{
				_logManager.RaiseLog(ex);
				return CommandResult.Failure(DBOperationMessages.AnErrorOccurredWhileRemovingData);
			}
		}

		public async Task<CommandResult<ProfileImage>> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			try
			{
				var item = await _context.ProfileImages.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
				if (item is null)
					return CommandResult<ProfileImage>.Failure(DBOperationMessages.DataWasNotFound, item);
				else
					return CommandResult<ProfileImage>.Success(DBOperationMessages.DataFoundedCorrectly, item);

			}
			catch (Exception ex)
			{
				await _logManager.RaiseLogAsync(ex, cancellationToken);
				return CommandResult<ProfileImage>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
			}
		}

		public CommandResult<IQueryable<ProfileImage>> GetAllAsIQueryable()
		{
			try
			{
				var data = _context.ProfileImages.AsQueryable();
				return CommandResult<IQueryable<ProfileImage>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
			}
			catch (Exception ex)
			{
				_logManager.RaiseLog(ex);
				return CommandResult<IQueryable<ProfileImage>>.FailureInRetrivingData(null);
			}
		}

		public async Task<CommandResult<List<ProfileImage>>> GetAllByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
		{
			try
			{
				List<ProfileImage> list = _context.ProfileImages.Where(x => x.CustomerId == customerId).ToList();
				return CommandResult<List<ProfileImage>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, list);
			}
			catch (Exception ex)
			{
				await _logManager.RaiseLogAsync(ex, cancellationToken);
				return CommandResult<List<ProfileImage>>.FailureInRetrivingData(null);
			}
		}

		public async Task<CommandResult<ProfileImage>> GetByIdAndCustomerIdAsync(long id, int customerId, CancellationToken cancellationToken)
		{
			try
			{
				var item = await _context.ProfileImages.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id & x.CustomerId == customerId);
				if (item is null)
					return CommandResult<ProfileImage>.Failure(DBOperationMessages.DataWasNotFound, item);
				else
					return CommandResult<ProfileImage>.Success(DBOperationMessages.DataFoundedCorrectly, item);

			}
			catch (Exception ex)
			{
				await _logManager.RaiseLogAsync(ex, cancellationToken);
				return CommandResult<ProfileImage>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
			}
		}
	}

}
