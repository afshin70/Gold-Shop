using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Entities;
using Gold.Infrastracture.EFCoreContext;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.EntityFrameworkCore;

namespace Gold.Infrastracture.Repositories
{
    public class BankCardNoRepository : IBankCardNoRepository
	{
		private readonly ApplicationContext _context;
		private readonly ILogManager _logManager;

		public BankCardNoRepository(ApplicationContext applicationContext, ILogManager logManager)
		{
			_context = applicationContext;
			this._logManager = logManager;
		}
		public async Task<CommandResult<BankCardNo>> InsertAsync(BankCardNo entity, CancellationToken cancellationToken)
		{
			try
			{
				await _context.BankCardNo.AddAsync(entity);
				return CommandResult<BankCardNo>.Success(DBOperationMessages.DataAddedCorrectly, entity);
			}
			catch (Exception ex)
			{
				await _logManager.RaiseLogAsync(ex, cancellationToken);
				return CommandResult<BankCardNo>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
			}
		}

		public CommandResult<BankCardNo> Update(BankCardNo entity)
		{
			try
			{
				_context.BankCardNo.Update(entity);
				return CommandResult<BankCardNo>.Success(DBOperationMessages.DataEditedCorrectly, entity);
			}
			catch (Exception ex)
			{
				_logManager.RaiseLog(ex);
				return CommandResult<BankCardNo>.FailureInUpdateData();
			}
		}

		public CommandResult Delete(BankCardNo entity)
		{
			try
			{
				_context.BankCardNo.Remove(entity);
				return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
			}
			catch (Exception ex)
			{
				_logManager.RaiseLog(ex);
				return CommandResult<BankCardNo>.FailureInRemoveData();
			}
		}

		public CommandResult DeleteRange(List<BankCardNo> entities)
		{
			try
			{
				_context.BankCardNo.RemoveRange(entities);
				return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
			}
			catch (Exception ex)
			{
				_logManager.RaiseLog(ex);
				return CommandResult.Failure(DBOperationMessages.AnErrorOccurredWhileRemovingData);
			}
		}

		public async Task<CommandResult<BankCardNo>> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			try
			{
				var item = await _context.BankCardNo.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
				if (item is null)
					return CommandResult<BankCardNo>.Failure(DBOperationMessages.DataWasNotFound, item);
				else
					return CommandResult<BankCardNo>.Success(DBOperationMessages.DataFoundedCorrectly, item);

			}
			catch (Exception ex)
			{
				await _logManager.RaiseLogAsync(ex, cancellationToken);
				return CommandResult<BankCardNo>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
			}
		}

		public CommandResult<IQueryable<BankCardNo>> GetAllAsIQueryable()
		{
			try
			{
				var data = _context.BankCardNo.AsQueryable();
				return CommandResult<IQueryable<BankCardNo>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
			}
			catch (Exception ex)
			{
				_logManager.RaiseLog(ex);
				return CommandResult<IQueryable<BankCardNo>>.FailureInRetrivingData(null);
			}
		}

		public async Task<CommandResult<List<BankCardNo>>> GetAllByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
		{
			try
			{
				List<BankCardNo> list = _context.BankCardNo.Where(x => x.CustomerId == customerId).ToList();
				return CommandResult<List<BankCardNo>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, list);
			}
			catch (Exception ex)
			{
				await _logManager.RaiseLogAsync(ex, cancellationToken);
				return CommandResult<List<BankCardNo>>.FailureInRetrivingData(null);
			}
		}

		public async Task<CommandResult<BankCardNo>> GetByIdAndCustomerIdAsync(long id, int customerId, CancellationToken cancellationToken)
		{
			try
			{
				var item = await _context.BankCardNo.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id & x.CustomerId == customerId);
				if (item is null)
					return CommandResult<BankCardNo>.Failure(DBOperationMessages.DataWasNotFound, item);
				else
					return CommandResult<BankCardNo>.Success(DBOperationMessages.DataFoundedCorrectly, item);

			}
			catch (Exception ex)
			{
				await _logManager.RaiseLogAsync(ex, cancellationToken);
				return CommandResult<BankCardNo>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
			}
		}
	}

}
