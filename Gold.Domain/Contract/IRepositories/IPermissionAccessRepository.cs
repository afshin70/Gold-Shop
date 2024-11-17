using Gold.Domain.Contract.IRepositories.Base;
using Gold.Domain.Entities;
using Gold.SharedKernel.DTO.OperationResult;

namespace Gold.Domain.Contract.IRepositories
{
    public interface IPermissionAccessRepository : IBaseRepository<PermissionAccess, int>
    {
        CommandResult DeleteRange(List<PermissionAccess> entities);
        Task<CommandResult<List<PermissionAccess>>> InsertRangeAsync(List<PermissionAccess> entities, CancellationToken cancellationToken);
    }
}
