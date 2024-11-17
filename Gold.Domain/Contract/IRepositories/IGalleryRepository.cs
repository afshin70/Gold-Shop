using Gold.Domain.Contract.IRepositories.Base;
using Gold.Domain.Entities;
using Gold.SharedKernel.DTO.OperationResult;

namespace Gold.Domain.Contract.IRepositories
{
    public interface IGalleryRepository : IBaseRepository<Gallery, int>
    {
        Task<CommandResult<List<Gallery>>> GetAllAsync(CancellationToken cancellationToken);
        Task<CommandResult<bool>> IsExistByNameAsync(string name, CancellationToken cancellationToken);
    }
}
