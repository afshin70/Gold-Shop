using Gold.Domain.Contract.IRepositories.Base;
using Gold.Domain.Entities;
using Gold.SharedKernel.DTO.OperationResult;

namespace Gold.Domain.Contract.IRepositories
{
    public interface IInstallmentRepository : IBaseRepository<Installment, long>
    {
        Task<CommandResult<Installment>> GetFirstNotPayedAsync(long documentId, CancellationToken cancellationToken = default);
    }
}
