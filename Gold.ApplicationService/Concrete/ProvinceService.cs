using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.CityModels;
using Gold.Domain.Entities;
using Gold.Infrastracture.Repositories.UOW;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Concrete
{
    public class ProvinceService : IProvinceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogManager _logManager;

        public ProvinceService(IUnitOfWork unitOfWork, ILogManager logManager)
        {
            _unitOfWork = unitOfWork;
            _logManager = logManager;
        }

        public async Task<CommandResult<IEnumerable<ProvinceModel>>> GetAllProvincesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _unitOfWork.CityRepository.GetAllProvincesAsync(cancellationToken);
                if (result.IsSuccess)
                {
                    var provinces = result.Data.Select(x => new ProvinceModel
                    {
                        Id = x.Id,
                        Title = x.Title
                    }).ToList();
                    return CommandResult<IEnumerable<ProvinceModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, provinces);
                }
                else
                {
                    return CommandResult<IEnumerable<ProvinceModel>>.Failure(UserMessages.ErrorInLoadProvinces, new List<ProvinceModel>());
                }
            }
            catch (Exception ex)
            {
                //log Exception
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<IEnumerable<ProvinceModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new List<ProvinceModel>());
            }

        }
        public async Task<CommandResult<List<SelectListItem>>> GetAllProvincesSelectListItemAsync(int selectedId=0,CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _unitOfWork.CityRepository.GetAllProvincesAsync(cancellationToken);
                if (result.IsSuccess)
                {
                    var provinces = result.Data.Select(x => new SelectListItem
                    {
                        Selected=(x.Id == selectedId),
                        Value = x.Id.ToString(),
                        Text = x.Title
                    }).ToList();
                    return CommandResult<List<SelectListItem>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, provinces);
                }
                else
                {
                    return CommandResult<List<SelectListItem>>.Failure(UserMessages.ErrorInLoadProvinces, new());
                }
            }
            catch (Exception ex)
            {
                //log Exception
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<SelectListItem>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }

        }

		//public async Task<CommandResult<IEnumerable<ProvinceModel>>> GetAllProvincesWithCitiesAsync(CancellationToken cancellationToken)
		//{
		//    try
		//    {
		//        var result = await _unitOfWork.CityRepository.GetAllAsync(cancellationToken);
		//        if (result.IsSuccess)
		//        {
		//            List<ProvinceModel> list = result.Data.Select(province => new ProvinceModel
		//            {
		//                Id= province.Id,
		//                Title= province.Title,
		//                Cities=result.Data.Select(city=>new CityModel
		//                {
		//                    Id= city.Id,
		//                    ProvinceId=province.Id,
		//                    Title= city.Title
		//                }).ToList()
		//            }).ToList();
		//            return  CommandResult<IEnumerable<ProvinceModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted,list);
		//        }
		//        else
		//        {
		//            return CommandResult<IEnumerable<ProvinceModel>>.Failure(OperationResultMessage.OperationIsFailure, null);
		//        }
		//    }
		//    catch (Exception ex)
		//    {
		//        //log Exception
		//        await _logManager.RaiseLogAsync(ex, cancellationToken);
		//        return CommandResult<IEnumerable<ProvinceModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
		//    }

		//}

		public async Task<CommandResult<IEnumerable<CityModel>>> GetCitiesOfProvinceAsync(int provinceId, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _unitOfWork.CityRepository.GetCitiesByParentIdAsync(provinceId, cancellationToken);
                if (result.IsSuccess)
                {
                    var list = result.Data.Select(x => new CityModel
                    {
                        Id = x.Id,
                        Title = x.Title
                    }).ToList();
                    return CommandResult<IEnumerable<CityModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, list);
                }
                else
                {
                    return CommandResult<IEnumerable<CityModel>>.Failure(UserMessages.ErrorInLoadCities, new List<CityModel>());
                }
            }
            catch (Exception ex)
            {
                //log Exception
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<IEnumerable<CityModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new List<CityModel>());
            }
        }
        public async Task<CommandResult<List<SelectListItem>>> GetCitiesOfProvinceSelectListItemAsync(int provinceId, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _unitOfWork.CityRepository.GetCitiesByParentIdAsync(provinceId, cancellationToken);
                if (result.IsSuccess)
                {
                    var list = result.Data.Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Title
                    }).ToList();
                    return CommandResult<List<SelectListItem>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, list);
                }
                else
                {
                    return CommandResult<List<SelectListItem>>.Failure(UserMessages.ErrorInLoadCities, new());
                }
            }
            catch (Exception ex)
            {
                //log Exception
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<SelectListItem>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }

        public async Task<CommandResult<List<SelectListItem>>> GetCitiesOfProvinceWithSelectedCityAsync(int cityId, CancellationToken cancellationToken = default)
        {
            try
            {
                CommandResult<City> selectedCityResult = await _unitOfWork.CityRepository.GetCityAsync(cityId, cancellationToken);

               
                List<SelectListItem> cities = new List<SelectListItem>();
                if (selectedCityResult.IsSuccess)
                {
                    var citiesResult = await GetCitiesOfProvinceAsync(selectedCityResult.Data.ParentId.Value, cancellationToken);
                    if (citiesResult.IsSuccess)
                    {
                        cities = citiesResult.Data.Select(x => new SelectListItem
                        {
                            Value = x.Id.ToString(),
                            Text = x.Title,
                            Selected = x.Id == selectedCityResult.Data.Id
                        }).ToList();
                    }

                    return CommandResult<List<SelectListItem>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, cities);
                }
                else
                {
                    return CommandResult<List<SelectListItem>>.Failure(OperationResultMessage.OperationIsFailure, cities);
                }
            }
            catch (Exception ex)
            {
                //log Exception
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<SelectListItem>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }

        public async Task<CommandResult<List<SelectListItem>>> GetCitiesOfProvinceWithSelectedCityAsync(string cityName, CancellationToken cancellationToken = default)
        {
            try
            {
                CommandResult<City> selectedCityResult = await _unitOfWork.CityRepository.GetCityAsync(cityName, cancellationToken);
                List<SelectListItem> cities = new List<SelectListItem>();
                if (selectedCityResult.IsSuccess)
                {
                    var citiesResult = await GetCitiesOfProvinceAsync(selectedCityResult.Data.ParentId.Value, cancellationToken);
                    if (citiesResult.IsSuccess)
                    {
                        cities = citiesResult.Data.Select(x => new SelectListItem
                        {
                            Value = x.Id.ToString(),
                            Text = x.Title,
                            Selected = x.Id == selectedCityResult.Data.Id
                        }).ToList();
                    }

                    return CommandResult<List<SelectListItem>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, cities);
                }
                else
                {
                    return CommandResult<List<SelectListItem>>.Failure(OperationResultMessage.OperationIsFailure, cities);
                }
            }
            catch (Exception ex)
            {
                //log Exception
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<SelectListItem>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }

        public async Task<CommandResult<List<SelectListItem>>> GetProviancesWithSelectedCityAsync(int cityId, CancellationToken cancellationToken = default)
        {
            try
            {
                CommandResult<City> selectedCityResult = await _unitOfWork.CityRepository.GetCityAsync(cityId, cancellationToken);
                var provianceesResult = await GetAllProvincesAsync(cancellationToken);
                List<SelectListItem> proviances = new List<SelectListItem>();
                if (selectedCityResult.IsSuccess)
                {
                    if (provianceesResult.IsSuccess)
                    {
                        proviances = provianceesResult.Data.Select(x => new SelectListItem
                        {
                            Value = x.Id.ToString(),
                            Text = x.Title,
                            Selected = x.Id == selectedCityResult.Data.ParentId
                        }).ToList();
                    }

                    return CommandResult<List<SelectListItem>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, proviances);
                }
                else
                {
                    proviances = provianceesResult.Data.Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Title,
                    }).ToList();
                    return CommandResult<List<SelectListItem>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, proviances);
                }
            }
            catch (Exception ex)
            {
                //log Exception
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<SelectListItem>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }

		public async Task<CommandResult<List<SelectListItem>>> GetProviancesWithSelectedCityAsync(string cityName, CancellationToken cancellationToken = default)
		{
			try
			{
				CommandResult<City> selectedCityResult = await _unitOfWork.CityRepository.GetCityAsync(cityName, cancellationToken);
				var provianceesResult = await GetAllProvincesAsync(cancellationToken);
				List<SelectListItem> proviances = new List<SelectListItem>();
				if (selectedCityResult.IsSuccess)
				{
					if (provianceesResult.IsSuccess)
					{
						proviances = provianceesResult.Data.Select(x => new SelectListItem
						{
							Value = x.Id.ToString(),
							Text = x.Title,
							Selected = x.Id == selectedCityResult.Data.ParentId
						}).ToList();
					}

					return CommandResult<List<SelectListItem>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, proviances);
				}
				else
				{
					proviances = provianceesResult.Data.Select(x => new SelectListItem
					{
						Value = x.Id.ToString(),
						Text = x.Title,
					}).ToList();
					return CommandResult<List<SelectListItem>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, proviances);
				}
			}
			catch (Exception ex)
			{
				//log Exception
				await _logManager.RaiseLogAsync(ex, cancellationToken);
				return CommandResult<List<SelectListItem>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
			}
		}

		public async Task<CommandResult<bool>> IsValidCityIdAsync(int cityId, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _unitOfWork.CityRepository.IsExistCityAsync(cityId, cancellationToken);
                if (result.IsSuccess)
                {
                    if (result.Data)
                    {
                        return CommandResult<bool>.Success(UserMessages.SelectedCityIsValid, result.Data);

                    }
                    else
                    {
                        return CommandResult<bool>.Failure(UserMessages.SelectedCityIsNotValid, result.Data);
                    }
                }
                else
                {
                    return CommandResult<bool>.Failure(result.Message, false);
                }
            }
            catch (Exception ex)
            {
                //log Exception
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<bool>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, false);
            }
        }
    }
}
