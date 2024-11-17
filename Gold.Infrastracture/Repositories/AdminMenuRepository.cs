using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Entities;
using Gold.Infrastracture.EFCoreContext;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.EntityFrameworkCore;

namespace Gold.Infrastracture.Repositories
{
    public class AdminMenuRepository : IAdminMenuRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;

        public AdminMenuRepository(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            _logManager = logManager;
        }


        public CommandResult<IQueryable<AdminMenu>> GetAllAsIQueryable()
        {
            try
            {
                var data = _context.AdminMenus.AsQueryable();
                return CommandResult<IQueryable<AdminMenu>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, data);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<AdminMenu>>.FailureInRetrivingData(null);
            }
        }
        public async Task<CommandResult<List<AdminMenu>>> GetAllByIdsAsync(List<byte>? ids, CancellationToken cancellationToken = default)
        {
            try
            {
                List<AdminMenu> list = new List<AdminMenu>();
                if (ids != null)
                {
                    list = await _context.AdminMenus.Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);
                }
                return CommandResult<List<AdminMenu>>.Success(DBOperationMessages.TheDataListWasFetchedCorrectly, list);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<AdminMenu>>.FailureInRetrivingData(null);
            }
        }

    }
}
