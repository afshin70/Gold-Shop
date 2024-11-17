using Gold.Domain.Contract.IRepositories.Base;
using Gold.Domain.Entities;
using Gold.SharedKernel.DTO.OperationResult;

namespace Gold.Domain.Contract.IRepositories
{
    public interface ISellerRepository : IBaseRepository<Seller, int>
    {
        Task<CommandResult<List<Seller>>> GetAllByGalleryIdAsync(int galleryId, CancellationToken cancellationToken);
    }
}
