using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Contract.IRepositories.Base;
using Gold.Domain.Entities;
using Gold.Domain.Entities.AuthEntities;
using Gold.Domain.Entities.Base;
using Gold.Infrastracture.EFCoreContext;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Enums;
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
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public DocumentRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            this._logManager = logManager;
        }

        public async Task<CommandResult<Document>> InsertAsync(Document entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.Documents.AddAsync(entity);
                return CommandResult<Document>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Document>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public CommandResult<Document> Update(Document entity)
        {
            try
            {
                _context.Documents.Update(entity);
                return CommandResult<Document>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<Document>.FailureInUpdateData();
            }
        }

        public CommandResult Delete(Document entity)
        {
            try
            {
                _context.Documents.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<Document>.FailureInRemoveData();
            }
        }

        public async Task<CommandResult<Document>> GetByIdAsync(long id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.Documents.FirstOrDefaultAsync(x => x.Id == id);
                if (item is null)
                    return CommandResult<Document>.Failure(DBOperationMessages.DataWasNotFound, item);
                else
                    return CommandResult<Document>.Success(DBOperationMessages.DataFoundedCorrectly, item);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Document>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public CommandResult<IQueryable<Document>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.Documents.AsQueryable();
                return CommandResult<IQueryable<Document>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<Document>>.FailureInRetrivingData(null);
            }
        }

        public async Task<CommandResult<bool>> IsExistDocumentNumberBySatusAsync(DocumentStatus documentStatus, int documentNumber, CancellationToken cancellationToken)
        {
            try
            {
                var isExist = _context.Documents.Any(x => x.Status == documentStatus & x.DocumentNo == documentNumber);
                if (isExist)
                {

                    return CommandResult<bool>.Success(DBOperationMessages.DataFoundedCorrectly, true);
                }
                else
                {
                    return CommandResult<bool>.Failure(DBOperationMessages.DataWasNotFound, false);
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<bool>.FailureInRetrivingData(false);
            }
        }

        public async Task<CommandResult<string>> GetAdminDescriptionAsync(long documentId, CancellationToken cancellationToken)
        {
            try
            {
                string? description = await _context.Documents.Where(x => x.Id == documentId).Select(x => x.AdminDescription).FirstOrDefaultAsync(cancellationToken);
                return CommandResult<string>.Success(DBOperationMessages.DataFoundedCorrectly, description);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<string>.FailureInRetrivingData(string.Empty);
            }
        }

        public async Task<CommandResult<DocumentStatus>> GetDocumentStatusByInstallmentIdAsync(long installmentId, CancellationToken cancellationToken)
        {
            try
            {
                DocumentStatus? status =await _context.Installments
                    .Where(x=>x.Id==installmentId)
                    .Include(x=>x.Document)
                    .Select(x=>x.Document.Status)
                    .FirstOrDefaultAsync(cancellationToken);
                if (status.HasValue)
                {

                    return CommandResult<DocumentStatus>.Success(DBOperationMessages.DataFoundedCorrectly, status.Value);
                }
                else
                {
                    return CommandResult<DocumentStatus>.Failure(DBOperationMessages.DataWasNotFound, status.Value);
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<DocumentStatus>.FailureInRetrivingData(0);
            }
        }
    }
}
