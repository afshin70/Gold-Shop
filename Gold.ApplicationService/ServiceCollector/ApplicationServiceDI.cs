using FluentValidation;
using Gold.ApplicationService.Concrete;
using Gold.ApplicationService.Concrete.Authentication;
using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.AuthenticationModels;
using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels;
using Gold.ApplicationService.Contract.Interfaces;
using Gold.ApplicationService.Imp;
using Gold.Domain.Entities.AuthEntities;
using Gold.Infrastracture.Configurations.JWTSystem;
using Gold.SharedKernel.DTO.FileAddress;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.ServiceCollector
{
    public static class ApplicationServiceDI
    {
        public static void AddApplicationSerices(this IServiceCollection services)
        {
            //services.AddApplicationSericesModelValidators();

            services.AddScoped<IAuthenticationManager, AuthenticationManager>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IProvinceService, ProvinceService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IGalleryService, GalleryService>();
            services.AddScoped<ISellerService, SellerService>();
            services.AddScoped<IManagerService, ManagerService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<ICustomerMessageService, CustomerMessageService>();
            services.AddScoped<IContentManagerService, ContentManagerService>();
            services.AddScoped<IProductManagementService, ProductManagementService>();
        }

        

    }
}
