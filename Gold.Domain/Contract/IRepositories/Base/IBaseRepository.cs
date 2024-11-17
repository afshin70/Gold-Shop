using Gold.Domain.Entities;
using Gold.Domain.Entities.AuthEntities;
using Gold.Domain.Entities.Base;
using Gold.SharedKernel.DTO.OperationResult;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.Domain.Contract.IRepositories.Base
{
    public interface IBaseRepository<TEntity, TKey> where TEntity : IEntity
    {
        Task<CommandResult<TEntity>> InsertAsync(TEntity entity, CancellationToken cancellationToken);
        CommandResult<TEntity> Update(TEntity entity);
        CommandResult Delete(TEntity entity);
        Task<CommandResult<TEntity>> GetByIdAsync(TKey id, CancellationToken cancellationToken);
        CommandResult<IQueryable<TEntity>> GetAllAsIQueryable();




        // CommandResult<TEntity> Insert(TEntity entity);
        // CommandResult<IEnumerable<TEntity>> GetAll();
        //Task<CommandResult<IEnumerable<TEntity>>> GetAllAsync(CancellationToken cancellationToken);
        //Task<CommandResult<IEnumerable<TEntity>>> GetAllAsync(int skip, int take, CancellationToken cancellationToken);

        //CommandResult<TEntity> GetById(TKey id);
        //Task<CommandResult<bool>> IsExistAsync(TKey id, CancellationToken cancellationToken);
    }

    //public interface IRangeBaseRepository<TEntity, TKey> where TEntity : IEntity
    //{
    //    //CommandResult<IEnumerable<TEntity>> InsertRange(IEnumerable<TEntity> entities);
    //    Task<CommandResult<IEnumerable<TEntity>>> InsertRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);
    //    CommandResult<IEnumerable<TEntity>> UpdateRange(IEnumerable<TEntity> entities);
    //    CommandResult DeleteRange(IEnumerable<TEntity> entities);

    //}
}
