using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Contract.IRepositories.Base;
using Gold.Domain.Entities;
using Gold.Domain.Entities.AuthEntities;
using Gold.Domain.Entities.Base;
using Gold.Infrastracture.EFCoreContext;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gold.Infrastracture.Repositories
{
    public class CollateralRepository : ICollateralRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public CollateralRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            this._logManager = logManager;
        }

        public async Task<CommandResult<Collateral>> InsertAsync(Collateral entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.Collaterals.AddAsync(entity);
                return CommandResult<Collateral>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Collateral>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public CommandResult<Collateral> Update(Collateral entity)
        {
            try
            {
                _context.Collaterals.Update(entity);
                return CommandResult<Collateral>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<Collateral>.FailureInUpdateData();
            }
        }

        public CommandResult Delete(Collateral entity)
        {
            try
            {
                _context.Collaterals.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<Collateral>.FailureInRemoveData();
            }
        }

        public async Task<CommandResult<Collateral>> GetByIdAsync(long id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.Collaterals.FirstOrDefaultAsync(x => x.Id == id);
                if (item is null)
                    return CommandResult<Collateral>.Failure(DBOperationMessages.DataWasNotFound, item);
                else
                    return CommandResult<Collateral>.Success(DBOperationMessages.DataFoundedCorrectly, item);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Collateral>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public CommandResult<IQueryable<Collateral>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.Collaterals.AsQueryable();
                return CommandResult<IQueryable<Collateral>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<Collateral>>.FailureInRetrivingData(null);
            }
        }
    }
}
