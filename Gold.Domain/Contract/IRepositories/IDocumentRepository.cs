using Gold.Domain.Contract.IRepositories.Base;
using Gold.Domain.Entities;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Enums;

namespace Gold.Domain.Contract.IRepositories
{
    public interface IDocumentRepository : IBaseRepository<Document, long>
    {
        Task<CommandResult<string>> GetAdminDescriptionAsync(long documentId, CancellationToken cancellationToken);
        Task<CommandResult<DocumentStatus>> GetDocumentStatusByInstallmentIdAsync(long installmentId, CancellationToken cancellationToken);
        Task<CommandResult<bool>> IsExistDocumentNumberBySatusAsync(DocumentStatus documentStatus, int documentNumber, CancellationToken cancellationToken);
    }
}
