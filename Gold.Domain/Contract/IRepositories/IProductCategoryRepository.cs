using Gold.Domain.Contract.IRepositories.Base;
using Gold.Domain.Entities;
using Gold.SharedKernel.DTO.OperationResult;

namespace Gold.Domain.Contract.IRepositories
{
    public interface IProductCategoryRepository : IBaseRepository<ProductCategory, long>
    {
        CommandResult DeleteRange(List<ProductCategory> entities);
        Task<CommandResult<List<ProductCategory>>> InsertRangeAsync(List<ProductCategory> entities, CancellationToken cancellationToken);
    }
}
