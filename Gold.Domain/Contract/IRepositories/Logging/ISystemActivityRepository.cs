using Gold.Domain.Contract.IRepositories.Base;
using Gold.Domain.Entities.Base;
using Gold.Domain.Entities.LoggingEntities;
using Gold.SharedKernel.DTO.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.Domain.Contract.IRepositories.Logging
{
    public interface ISystemActivityRepository
    {
        Task<CommandResult<SystemActivityLog>> InsertAsync(SystemActivityLog entity, CancellationToken cancellationToken);
    }
}
