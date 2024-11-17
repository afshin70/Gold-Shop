using Gold.Infrastracture.EFCoreContext;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.InMemoryStorageModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.Infrastracture.ExternalService
{
    public class InMemoryStorageService : IInMemoryStorageService
    {
        private readonly InMemoryStorageContext _context;
        private readonly ILogManager _logManager;

        public InMemoryStorageService(InMemoryStorageContext context, ILogManager logManager)
        {
            this._context = context;
            this._logManager = logManager;
        }
        public async Task<MemoryData> AddDataAsync(MemoryData model)
        {
            try
            {
                await _context.MemoryDatas.AddAsync(model);
                await _context.SaveChangesAsync();
                return model;
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return null;
            }
        }

        public async Task<MemoryData> GetDataAsync(Guid id)
        {
            try
            {
                return await _context.MemoryDatas.FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return null;
            }
        }

        public bool RemoveData(Guid id)
        {
            try
            {
                var item = _context.MemoryDatas.FirstOrDefault(x => x.Id == id);
                if (item is null)
                {
                    return true;
                }
                _context.MemoryDatas.Remove(item);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return false;
            }
        }

        public MemoryData UpdateData(MemoryData model)
        {
            try
            {
                _context.MemoryDatas.Update(model);
                _context.SaveChanges();
                return model;
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return null;
            }
        }
    }
}
