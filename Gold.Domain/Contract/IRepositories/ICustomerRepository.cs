using Gold.Domain.Contract.DTOs.UserModels.CustomerModels;
using Gold.Domain.Contract.IRepositories.Base;
using Gold.Domain.Entities;
using Gold.SharedKernel.DTO.OperationResult;

namespace Gold.Domain.Contract.IRepositories
{
    public interface ICustomerRepository : IBaseRepository<Customer, int>
    {
        CommandResult<IQueryable<Customer>> GetAllAsIQueryable(bool includeUser);
        Task<CommandResult<int>> GetCustomerIdByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken);
        Task<CommandResult<CustomerMobileAndFullName>> GetCustomerNameAndMobileByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken);
        Task<CommandResult<int>> GetUserIdAsync(int customerId, CancellationToken cancellationToken);
        Task<CommandResult<bool>> IsActiveByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken);
        Task<CommandResult<bool>> IsExistAsync(int id, CancellationToken cancellationToken);
        Task<CommandResult<bool>> IsExistByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken);
    }
}
