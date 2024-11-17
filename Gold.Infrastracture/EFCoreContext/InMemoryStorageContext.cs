using Gold.SharedKernel.DTO.InMemoryStorageModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.Infrastracture.EFCoreContext
{
    public class InMemoryStorageContext : DbContext
    {
        public InMemoryStorageContext(DbContextOptions<InMemoryStorageContext> options) : base(options)
        {

        }
        public DbSet<MemoryData> MemoryDatas { get; set; }
    }
}
