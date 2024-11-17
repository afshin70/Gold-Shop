using Gold.Domain.Contract.IRepositories.Base;
using Gold.Domain.Entities;

namespace Gold.Domain.Contract.IRepositories
{
    public interface ICustomerMessageRepository : IBaseRepository<CustomerMessage, long>
    {
    }
}
