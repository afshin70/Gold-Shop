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
    public class CollateralTypeRepository : ICollateralTypeRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public CollateralTypeRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            this._logManager = logManager;
        }

        public async Task<CommandResult<CollateralType>> InsertAsync(CollateralType entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.CollateralTypes.AddAsync(entity);
                return CommandResult<CollateralType>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CollateralType>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public CommandResult<CollateralType> Update(CollateralType entity)
        {
            try
            {
                _context.CollateralTypes.Update(entity);
                return CommandResult<CollateralType>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<CollateralType>.FailureInUpdateData();
            }
        }

        public CommandResult Delete(CollateralType entity)
        {
            try
            {
                _context.CollateralTypes.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<CollateralType>.FailureInRemoveData();
            }
        }

        public async Task<CommandResult<CollateralType>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.CollateralTypes.FirstOrDefaultAsync(x => x.Id == id);
                if (item is null)
                    return CommandResult<CollateralType>.Failure(DBOperationMessages.DataWasNotFound, item);
                else
                    return CommandResult<CollateralType>.Success("", item);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CollateralType>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public CommandResult<IQueryable<CollateralType>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.CollateralTypes.AsQueryable();
                return CommandResult<IQueryable<CollateralType>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<CollateralType>>.FailureInRetrivingData(null);
            }
        }

        public async Task<CommandResult<List<CollateralType>>> GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                var data =await _context.CollateralTypes.ToListAsync();
                return CommandResult<List<CollateralType>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly,data);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<CollateralType>>.FailureInRetrivingData(null);
            }
        }
    }
}
