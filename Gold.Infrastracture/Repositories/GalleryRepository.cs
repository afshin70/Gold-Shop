using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Entities;
using Gold.Infrastracture.EFCoreContext;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;

namespace Gold.Infrastracture.Repositories
{
    public class GalleryRepository : IGalleryRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public GalleryRepository(ApplicationContext applicationContext, ILogManager logManager)
        {
            _context = applicationContext;
            _logManager = logManager;
        }

        public async Task<CommandResult<Gallery>> InsertAsync(Gallery entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.Galleries.AddAsync(entity);
                return CommandResult<Gallery>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Gallery>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public CommandResult<Gallery> Update(Gallery entity)
        {
            try
            {
                _context.Galleries.Update(entity);
                return CommandResult<Gallery>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<Gallery>.FailureInUpdateData();
            }
        }

        public CommandResult Delete(Gallery entity)
        {
            try
            {
                _context.Galleries.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<Gallery>.FailureInRemoveData();
            }
        }

        public async Task<CommandResult<Gallery>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.Galleries.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (item is null)
                    return CommandResult<Gallery>.Failure(DBOperationMessages.DataWasNotFound, item);
                else
                    return CommandResult<Gallery>.Success(DBOperationMessages.DataFoundedCorrectly, item);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Gallery>.Failure(DBOperationMessages.AnErrorOccurredWhileRetrievingData, null);
            }
        }

        public CommandResult<IQueryable<Gallery>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.Galleries.AsQueryable();
                return CommandResult<IQueryable<Gallery>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<Gallery>>.FailureInRetrivingData(null);
            }
        }

        public async Task<CommandResult<bool>> IsExistByNameAsync(string name, CancellationToken cancellationToken)
        {
            try
            {
                var isExist = await _context.Galleries.AnyAsync(x => x.Name == name);
                    return CommandResult<bool>.Success(DBOperationMessages.ExistenceOfDuplicateData, isExist);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<bool>.FailureInRetrivingData(false);
            }
        }

        public async Task<CommandResult<List<Gallery>>> GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                var data =await _context.Galleries.ToListAsync();
                return CommandResult<List<Gallery>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex,cancellationToken );
                return CommandResult<List<Gallery>>.FailureInRetrivingData(null);
            }
        }
    }
}
