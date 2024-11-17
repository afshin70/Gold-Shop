using Gold.Domain.Contract.IRepositories.Base;
using Gold.Domain.Entities;
using Gold.SharedKernel.DTO.OperationResult;

namespace Gold.Domain.Contract.IRepositories
{
    public interface IAdminMenuRepository
    {
        CommandResult<IQueryable<AdminMenu>> GetAllAsIQueryable();
        Task<CommandResult<List<AdminMenu>>> GetAllByIdsAsync(List<byte>? ids, CancellationToken cancellationToken = default);
    }
}
