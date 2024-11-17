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
    public class EditInformationRequestRepository : IEditInformationRequestRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public EditInformationRequestRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            this._logManager = logManager;
        }

        public async Task<CommandResult<EditInformationRequest>> InsertAsync(EditInformationRequest entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.EditInformationRequests.AddAsync(entity);
                return CommandResult<EditInformationRequest>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<EditInformationRequest>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public CommandResult<EditInformationRequest> Update(EditInformationRequest entity)
        {
            try
            {
                _context.EditInformationRequests.Update(entity);
                return CommandResult<EditInformationRequest>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<EditInformationRequest>.FailureInUpdateData();
            }
        }

        public CommandResult Delete(EditInformationRequest entity)
        {
            try
            {
                _context.EditInformationRequests.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<EditInformationRequest>.FailureInRemoveData();
            }
        }

        public async Task<CommandResult<EditInformationRequest>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.EditInformationRequests.FirstOrDefaultAsync(x => x.Id == id);
                if (item is null)
                    return CommandResult<EditInformationRequest>.Failure(DBOperationMessages.DataWasNotFound, item);
                else
                    return CommandResult<EditInformationRequest>.Success("", item);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<EditInformationRequest>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public CommandResult<IQueryable<EditInformationRequest>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.EditInformationRequests.AsQueryable();
                return CommandResult<IQueryable<EditInformationRequest>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<EditInformationRequest>>.FailureInRetrivingData(null);
            }
        }
    }
}
