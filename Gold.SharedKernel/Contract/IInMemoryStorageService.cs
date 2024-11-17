using Gold.SharedKernel.DTO.InMemoryStorageModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.SharedKernel.Contract
{
    public interface IInMemoryStorageService
    {
        Task<MemoryData> AddDataAsync(MemoryData model);
       MemoryData UpdateData(MemoryData model);
        Task<MemoryData> GetDataAsync(Guid id);
        bool RemoveData(Guid id);
    }
}
