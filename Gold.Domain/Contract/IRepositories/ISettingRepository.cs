using Gold.Domain.Contract.IRepositories.Base;
using Gold.Domain.Entities;
using Gold.Domain.Enums;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Filters;

namespace Gold.Domain.Contract.IRepositories
{
    public interface ISettingRepository :IBaseRepository<Setting,int>
    {
        Task<CommandResult<Setting>> GetSettingByTypeAsync(SettingType type, CancellationToken cancellationToken);
        Task<CommandResult> IsDuplicateAsync(SettingType type, CancellationToken cancellationToken);
    }
}
