using Gold.Domain.Entities.Base;
using Gold.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.Domain.Entities.AuthEntities
{
    public class User : IBaseEntity<int>
    {
        public User()
        {
            SecurityStamp = Guid.NewGuid();
        }
        public int Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string? Email { get; set; }
        public string Mobile { get; set; }

        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }

        public Guid SecurityStamp { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public int WrongPasswordCount { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? LockDate { get; set; }
        public DateTime? LastPasswordChangeDate { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public bool IsActive { get; set; }

        public UserType  UserType { get; set; }

        public Customer Customer { get; set; }
        public Seller Seller { get; set; }
        public Manager Manager { get; set; }

        public ICollection<AdminActivity> AdminActivities { get; set; }
        public ICollection<GoldPrice> GoldPrices { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
