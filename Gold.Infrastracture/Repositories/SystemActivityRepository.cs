using Gold.Domain.Contract.IRepositories.Logging;
using Gold.Domain.Entities;
using Gold.Domain.Entities.LoggingEntities;
using Gold.Infrastracture.EFCoreContext;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.Infrastracture.Repositories
{
    public class SystemActivityRepository : ISystemActivityRepository
    {
        private readonly ApplicationLoggerContext _context;
        private readonly ILogManager _logManager;

        public SystemActivityRepository(ApplicationLoggerContext context, ILogManager logManager)
        {
            this._context = context;
            this._logManager = logManager;
        }
        

        public async Task<CommandResult<SystemActivityLog>> InsertAsync(SystemActivityLog entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.SystemActivityLogs.AddAsync(entity, cancellationToken);
                return CommandResult<SystemActivityLog>.Success(DBOperationMessages.DataAddedCorrectly, entity);
            }
            catch (Exception ex)
            {
                return CommandResult<SystemActivityLog>.Failure(DBOperationMessages.AnErrorOccurredWhileAddingData,null);
            }
        }

        
    }
}
