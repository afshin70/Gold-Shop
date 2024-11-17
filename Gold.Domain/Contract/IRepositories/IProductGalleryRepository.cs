using Gold.Domain.Contract.IRepositories.Base;
using Gold.Domain.Entities;
using Gold.SharedKernel.DTO.OperationResult;

namespace Gold.Domain.Contract.IRepositories
{
    public interface IProductGalleryRepository : IBaseRepository<ProductGallery, long>
    {
        CommandResult DeleteRange(List<ProductGallery> entities);
        CommandResult<List<ProductGallery>> UpdateRange(List<ProductGallery> entities);
    }
}
