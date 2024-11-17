using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Entities;
using Gold.Infrastracture.EFCoreContext;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.EntityFrameworkCore;

namespace Gold.Infrastracture.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public ArticleRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            this._logManager = logManager;
        }

        public CommandResult Delete(Article entity)
        {
            try
            {
                _context.Articles.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                 _logManager.RaiseLog(ex);
                return CommandResult<Article>.FailureInRemoveData();
            }
        }

        public async Task<CommandResult<Article>> GetByIdAsync(int id,CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.Articles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
                if (item is null)
                {
                    return CommandResult<Article>.Failure(DBOperationMessages.DataWasNotFound, item);
                }
                else
                {
                    return CommandResult<Article>.Success(DBOperationMessages.TheDataWasFetchedCorrectly, item);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Article>.FailureInRetrivingData();
            }
        }

        public async Task<CommandResult<Article>> InsertAsync(Article entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.Articles.AddAsync(entity, cancellationToken);
                return CommandResult<Article>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Article>.Failure(DBOperationMessages.AnErrorOccurredWhileAddingData, entity);
            }
        }

        public CommandResult<Article> Update(Article entity)
        {
            try
            {
                _context.Articles.Update(entity);
                return CommandResult<Article>.Success(DBOperationMessages.DataEditedCorrectly, entity);
            }
            catch (Exception ex)
            {
                 _logManager.RaiseLog(ex);
                return CommandResult<Article>.Failure(DBOperationMessages.AnErrorOccurredWhileEditingData, entity);
            }
        }



        public CommandResult<IQueryable<Article>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.Articles.AsQueryable();
                return CommandResult<IQueryable<Article>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<Article>>.FailureInRetrivingData(null);
            }
        }
    }
}
