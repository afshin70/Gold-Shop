using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.Domain.Entities.Base
{
    public interface IBaseEntity: IEntity
    {
        public DateTime? LastModifiedDate { get; set; }
    }
    public interface IBaseEntity<TKey>:IEntity,IBaseEntity
    {
        public TKey Id { get; set; }
    }
    
}
