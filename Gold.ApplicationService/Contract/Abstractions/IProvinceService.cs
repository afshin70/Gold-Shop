using Gold.ApplicationService.Contract.DTOs.Models.CityModels;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.Abstractions
{
    public interface IProvinceService
    {
        Task<CommandResult<IEnumerable<ProvinceModel>>> GetAllProvincesAsync(CancellationToken cancellationToken = default);
        Task<CommandResult<List<SelectListItem>>> GetAllProvincesSelectListItemAsync(int selectedId=0,CancellationToken cancellationToken=default);
        Task<CommandResult<IEnumerable<CityModel>>> GetCitiesOfProvinceAsync(int provinceId, CancellationToken cancellationToken = default);
        Task<CommandResult<List<SelectListItem>>> GetCitiesOfProvinceSelectListItemAsync(int provinceId, CancellationToken cancellationToken = default);
        Task<CommandResult<List<SelectListItem>>> GetCitiesOfProvinceWithSelectedCityAsync(int cityId, CancellationToken cancellationToken = default);
        Task<CommandResult<List<SelectListItem>>> GetCitiesOfProvinceWithSelectedCityAsync(string cityName, CancellationToken cancellationToken = default);
        Task<CommandResult<List<SelectListItem>>> GetProviancesWithSelectedCityAsync(int cityId, CancellationToken cancellationToken = default);
        Task<CommandResult<List<SelectListItem>>> GetProviancesWithSelectedCityAsync(string cityName, CancellationToken cancellationToken = default);
        Task<CommandResult<bool>> IsValidCityIdAsync(int cityId,CancellationToken cancellationToken = default);
    }
}
