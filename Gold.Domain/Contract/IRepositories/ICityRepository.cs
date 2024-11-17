using Gold.Domain.Contract.IRepositories.Base;
using Gold.Domain.Entities;
using Gold.SharedKernel.DTO.OperationResult;

namespace Gold.Domain.Contract.IRepositories
{
    public interface ICityRepository 
    {
       Task<CommandResult<IEnumerable<City>>> GetAllProvincesAsync(CancellationToken cancellationToken);
        Task<CommandResult<IEnumerable<City>>> GetCitiesByParentIdAsync(int parentId, CancellationToken cancellationToken);
        Task<CommandResult<City>> GetCityAsync(int cityId, CancellationToken cancellationToken);
		Task<CommandResult<City>> GetCityAsync(string cityName, CancellationToken cancellationToken);
		Task<CommandResult<City>> GetProvinceAsync(int? cityId, CancellationToken cancellationToken);
        Task<CommandResult<bool>> IsExistCityAsync(int cityId, CancellationToken cancellationToken);
    }
}
