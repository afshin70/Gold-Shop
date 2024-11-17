using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using Gold.Infrastracture.EFCoreContext;
using Gold.Domain.Contract.DTOs;
using Gold.Infrastracture.Repositories;
using Gold.Domain.Contract.IRepositories;
using Gold.SharedKernel.Contract;
using Gold.Infrastracture.ExternalService;
using Gold.Infrastracture.LogSystem;
using Gold.Infrastracture.Repositories.UOW;
using Microsoft.Extensions.Configuration;

namespace Gold.Infrastracture.ServiceCollector
{
    public static class InfrastractureDI
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            //services.AddTransient<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork,UnitOfWork>();
            //services.AddTransient<UnitOfWork>();

            services.AddTransient<IFileService, FileService>();
        }

        public static void AddEFCoreMSSQLContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ApplicationContext>(option =>
            {
                option.UseSqlServer(connectionString);
            });

        }

        public static void AddApplicationLogSystem(this IServiceCollection services, string connectionString)
        {
            services.AddTransient<ILogManager, LogManager>();
            services.AddDbContext<ApplicationLoggerContext>(option =>
            {
                option.UseSqlServer(connectionString);
            });
        } 
        
        public static void AddInMemoryStorage(this IServiceCollection services)
        {
            services.AddDbContext<InMemoryStorageContext>(option =>
            {
                option.UseInMemoryDatabase("InMemoryStorageDb");
            });
            services.AddTransient<IInMemoryStorageService, InMemoryStorageService>();
        }
        
        public static void AddViewRender(this IServiceCollection services)
        {
            //services.AddTransient<IViewRenderService, ViewRenderService>();
            
        }
        
        public static void AddSmsService(this IServiceCollection services,ConfigurationManager configurationManager)
        {
            services.AddTransient<ISMSSender, SmsIrService>();
            
        }

        public static void AddCaptchaService(this IServiceCollection services)
        {
            services.AddTransient<ICaptchaResolver, CaptchaResolver>();
        }

    }
}
