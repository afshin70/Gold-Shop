using Gold.Domain.Contract.IRepositories.Base;
using Gold.Domain.Entities;
using Gold.SharedKernel.DTO.OperationResult;

namespace Gold.Domain.Contract.IRepositories
{
	public interface IBankCardNoRepository : IBaseRepository<BankCardNo, long>
    {
        CommandResult DeleteRange(List<BankCardNo> entities);
        Task<CommandResult<List<BankCardNo>>> GetAllByCustomerIdAsync(int customerId, CancellationToken cancellationToken);
        Task<CommandResult<BankCardNo>> GetByIdAndCustomerIdAsync(long id, int customerId, CancellationToken cancellationToken);
    }
}
