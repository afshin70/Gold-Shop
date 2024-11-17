using Gold.Domain.Contract.IRepositories.Base;
using Gold.Domain.Entities;
using Gold.SharedKernel.DTO.OperationResult;

namespace Gold.Domain.Contract.IRepositories
{
    public interface IProfileImageRepository : IBaseRepository<ProfileImage, long>
    {
        CommandResult DeleteRange(List<ProfileImage> entities);
        Task<CommandResult<List<ProfileImage>>> GetAllByCustomerIdAsync(int customerId, CancellationToken cancellationToken);
        Task<CommandResult<ProfileImage>> GetByIdAndCustomerIdAsync(long id, int customerId, CancellationToken cancellationToken);
    }
}
