using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Entities;
using Gold.Infrastracture.EFCoreContext;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;

namespace Gold.Infrastracture.Repositories
{
	public class EssentialTelRepository : IEssentialTelRepository
	{
		private readonly ApplicationContext _context;
		private readonly ILogManager _logManager;

		public EssentialTelRepository(ApplicationContext applicationContext, ILogManager logManager)
		{
			_context = applicationContext;
			this._logManager = logManager;
		}
		public async Task<CommandResult<EssentialTel>> InsertAsync(EssentialTel entity, CancellationToken cancellationToken)
		{
			try
			{
				await _context.EssentialTels.AddAsync(entity);
				return CommandResult<EssentialTel>.Success(DBOperationMessages.DataAddedCorrectly, entity);
			}
			catch (Exception ex)
			{
				await _logManager.RaiseLogAsync(ex, cancellationToken);
				return CommandResult<EssentialTel>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
			}
		}

		public CommandResult<EssentialTel> Update(EssentialTel entity)
		{
			try
			{
				_context.EssentialTels.Update(entity);
				return CommandResult<EssentialTel>.Success(DBOperationMessages.DataEditedCorrectly, entity);
			}
			catch (Exception ex)
			{
				_logManager.RaiseLog(ex);
				return CommandResult<EssentialTel>.FailureInUpdateData();
			}
		}

		public CommandResult Delete(EssentialTel entity)
		{
			try
			{
				_context.EssentialTels.Remove(entity);
				return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
			}
			catch (Exception ex)
			{
				_logManager.RaiseLog(ex);
				return CommandResult<EssentialTel>.FailureInRemoveData();
			}
		}

		public CommandResult DeleteRange(List<EssentialTel> entities)
		{
			try
			{
				_context.EssentialTels.RemoveRange(entities);
				return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
			}
			catch (Exception ex)
			{
				_logManager.RaiseLog(ex);
				return CommandResult.Failure(DBOperationMessages.AnErrorOccurredWhileRemovingData);
			}
		}

		public async Task<CommandResult<EssentialTel>> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			try
			{
				var item = await _context.EssentialTels.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
				if (item is null)
					return CommandResult<EssentialTel>.Failure(DBOperationMessages.DataWasNotFound, item);
				else
					return CommandResult<EssentialTel>.Success(DBOperationMessages.DataFoundedCorrectly, item);

			}
			catch (Exception ex)
			{
				await _logManager.RaiseLogAsync(ex, cancellationToken);
				return CommandResult<EssentialTel>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
			}
		}

		public CommandResult<IQueryable<EssentialTel>> GetAllAsIQueryable()
		{
			try
			{
				var data = _context.EssentialTels.AsQueryable();
				return CommandResult<IQueryable<EssentialTel>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
			}
			catch (Exception ex)
			{
				_logManager.RaiseLog(ex);
				return CommandResult<IQueryable<EssentialTel>>.FailureInRetrivingData(null);
			}
		}

		public async Task<CommandResult<List<EssentialTel>>> GetAllByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
		{
			try
			{
				List<EssentialTel> list = _context.EssentialTels.Where(x => x.CustomerId == customerId).ToList();
				return CommandResult<List<EssentialTel>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, list);
			}
			catch (Exception ex)
			{
				await _logManager.RaiseLogAsync(ex, cancellationToken);
				return CommandResult<List<EssentialTel>>.FailureInRetrivingData(null);
			}
		}

		public async Task<CommandResult<EssentialTel>> GetByIdAndCustomerIdAsync(long id, int customerId, CancellationToken cancellationToken)
		{
			try
			{
				var item = await _context.EssentialTels.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id & x.CustomerId == customerId);
				if (item is null)
					return CommandResult<EssentialTel>.Failure(DBOperationMessages.DataWasNotFound, item);
				else
					return CommandResult<EssentialTel>.Success(DBOperationMessages.DataFoundedCorrectly, item);

			}
			catch (Exception ex)
			{
				await _logManager.RaiseLogAsync(ex, cancellationToken);
				return CommandResult<EssentialTel>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
			}
		}
	}
}
