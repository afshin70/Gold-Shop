using Gold.Domain.Contract.IRepositories.Base;
using Gold.Domain.Entities;
using Gold.SharedKernel.DTO.OperationResult;

namespace Gold.Domain.Contract.IRepositories
{
    public interface IEssentialTelRepository : IBaseRepository<EssentialTel, long>
    {
        CommandResult DeleteRange(List<EssentialTel> entities);
        Task<CommandResult<List<EssentialTel>>> GetAllByCustomerIdAsync(int customerId, CancellationToken cancellationToken);
        Task<CommandResult<EssentialTel>> GetByIdAndCustomerIdAsync(long id, int customerId, CancellationToken cancellationToken);
    }
}
