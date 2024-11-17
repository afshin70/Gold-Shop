using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Entities;
using Gold.Domain.Entities.Base;
using Gold.Domain.Enums;
using Gold.Infrastracture.EFCoreContext;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gold.Infrastracture.Repositories
{
    public class SettingRepository : ISettingRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public SettingRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            this._logManager = logManager;
        }

        public CommandResult Delete(Setting entity)
        {
            try
            {
                _context.Settings.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                 _logManager.RaiseLog(ex);
                return CommandResult<Setting>.FailureInRemoveData();
            }
        }

        public async Task<CommandResult<Setting>> GetByIdAsync(int id,CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.Settings.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
                if (item is null)
                {
                    return CommandResult<Setting>.Failure(DBOperationMessages.DataWasNotFound, item);
                }
                else
                {
                    return CommandResult<Setting>.Success(DBOperationMessages.TheDataWasFetchedCorrectly, item);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Setting>.FailureInRetrivingData();
            }
        }
        public async Task<CommandResult<Setting>> GetSettingByTypeAsync(SettingType type,CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.Settings.FirstOrDefaultAsync(x => x.Type == type, cancellationToken);
                if (item is null)
                    return CommandResult<Setting>.FailureInRetrivingData();
                return CommandResult<Setting>.Success(DBOperationMessages.TheDataWasFetchedCorrectly, item);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Setting>.FailureInRetrivingData();
            }
        }
 
        public async Task<CommandResult<Setting>> InsertAsync(Setting entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.Settings.AddAsync(entity, cancellationToken);
                return CommandResult<Setting>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Setting>.Failure(DBOperationMessages.AnErrorOccurredWhileAddingData, entity);
            }
        }

        public CommandResult<Setting> Update(Setting entity)
        {
            try
            {
                _context.Settings.Update(entity);
                return CommandResult<Setting>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                 _logManager.RaiseLog(ex);
                return CommandResult<Setting>.Failure(DBOperationMessages.AnErrorOccurredWhileEditingData, entity);
            }
        }

        public async Task<CommandResult> IsDuplicateAsync(SettingType type, CancellationToken cancellationToken)
        {
            try
            {
                bool isExist = await _context.Settings.AnyAsync(x => x.Type == type,cancellationToken);
                if (isExist)
                    return CommandResult<bool>.Success(DBOperationMessages.ExistenceOfDuplicateData, isExist);
                else
                    return CommandResult<bool>.Failure(DBOperationMessages.NotExistenceOfDuplicateData, isExist);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<bool>.FailureInRetrivingData();
            }

        }

        public CommandResult<IQueryable<Setting>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.Settings.AsQueryable();
                return CommandResult<IQueryable<Setting>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<Setting>>.FailureInRetrivingData(null);
            }
        }
    }
}
