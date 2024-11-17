using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Entities;
using Gold.Domain.Entities.Base;
using Gold.Infrastracture.EFCoreContext;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gold.Infrastracture.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public CityRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            this._logManager = logManager;
        }

        public async Task<CommandResult<IEnumerable<City>>> GetAllProvincesAsync(CancellationToken cancellationToken)
        {
            try
            {
                var list = await _context.Cities.Where(c => c.ParentId == null).ToListAsync(cancellationToken);
                return CommandResult<IEnumerable<City>>.Success(DBOperationMessages.TheDataWasFetchedCorrectly, list);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<IEnumerable<City>>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }
        public async Task<CommandResult<IEnumerable<City>>> GetCitiesByParentIdAsync(int parentId, CancellationToken cancellationToken)
        {
            try
            {
                var list = await _context.Cities.Where(c => c.ParentId == parentId).ToListAsync(cancellationToken);
                return CommandResult<IEnumerable<City>>.Success(DBOperationMessages.TheDataWasFetchedCorrectly, list);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<IEnumerable<City>>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public async Task<CommandResult<City>> GetCityAsync(int cityId, CancellationToken cancellationToken)
        {
            try
            {
                var city = await _context.Cities.FirstOrDefaultAsync(x => x.Id == cityId, cancellationToken);
                if (city == null)
                {
                    return CommandResult<City>.Failure(DBOperationMessages.DataWasNotFound, city);
                }
                else
                {
                    return CommandResult<City>.Success(DBOperationMessages.DataFoundedCorrectly, city);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<City>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

		public async Task<CommandResult<City>> GetCityAsync(string cityName, CancellationToken cancellationToken)
		{
			try
			{
				var city = await _context.Cities.FirstOrDefaultAsync(x => x.Title == cityName, cancellationToken);
				if (city == null)
				{
					return CommandResult<City>.Failure(DBOperationMessages.DataWasNotFound, city);
				}
				else
				{
					return CommandResult<City>.Success(DBOperationMessages.DataFoundedCorrectly, city);
				}

			}
			catch (Exception ex)
			{
				await _logManager.RaiseLogAsync(ex, cancellationToken);
				return CommandResult<City>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
			}
		}

		public async Task<CommandResult<City>> GetProvinceAsync(int? cityId, CancellationToken cancellationToken)
        {
            try
            {
                var city = await _context.Cities.FirstOrDefaultAsync(x => x.Id == cityId, cancellationToken);
                if (city is null)
                    return CommandResult<City>.Failure(DBOperationMessages.DataWasNotFound, null);
                else
                {
                    var province = await _context.Cities.FirstOrDefaultAsync(x => x.Id == city.ParentId, cancellationToken);
                    if (province is null)
                        return CommandResult<City>.Failure(DBOperationMessages.DataWasNotFound, null);
                    else
                        return CommandResult<City>.Success(DBOperationMessages.DataFoundedCorrectly, province);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<City>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public async Task<CommandResult<bool>> IsExistCityAsync(int cityId, CancellationToken cancellationToken)
        {
            try
            {
                bool isExist = await _context.Cities.AnyAsync(x => x.Id == cityId & x.ParentId != null, cancellationToken);
                return CommandResult<bool>.Success(DBOperationMessages.DataFoundedCorrectly, isExist);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<bool>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, false);
            }
        }

    }
}
