using Gold.Domain.Entities;
using Gold.Domain.Entities.AuthEntities;
using Gold.Infrastracture.Configurations.EntitiesConfig;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.Infrastracture.EFCoreContext
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {

        }

        #region DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<EssentialTel> EssentialTels { get; set; }
      
       
        public DbSet<Setting> Settings { get; set; }
        public DbSet<SocialNetwork> SocialNetworks { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Installment> Installments { get; set; }
        public DbSet<CollateralType> CollateralTypes { get; set; }
        public DbSet<Collateral> Collaterals { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<CustomerPayment> CustomerPayments { get; set; }
        public DbSet<CustomerMessage> CustomerMessages { get; set; }
        public DbSet<EditInformationRequest> EditInformationRequests { get; set; }
        public DbSet<AdminMenuGroup> AdminMenuGroups { get; set; }
        public DbSet<AdminMenu> AdminMenus { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionAccess> PermissionAccesses { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<AdminActivity> AdminActivities { get; set; }
        public DbSet<SendSmsTemp> SmsTemps { get; set; }

		public DbSet<BankCardNo> BankCardNo { get; set; }
		public DbSet<ProfileImage> ProfileImages { get; set; }
		public DbSet<GoldPrice> GoldPrices { get; set; }
		public DbSet<FAQCategory> FAQCategories { get; set; }
		public DbSet<FAQ> FAQs { get; set; }
		public DbSet<Article> Articles { get; set; }


		public DbSet<Product> Products { get; set; }
		public DbSet<ProductGallery> ProductGallery { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<ProductCategory> ProductCategories { get; set; }
		public DbSet<FavoritProduct> FavoritProducts { get; set; }
		public DbSet<ContactUs> ContactUs { get; set; }

		#endregion

		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfig).Assembly);
            modelBuilder.HasDefaultSchema("dbo");
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
