using Gold.ApplicationService.Contract.DTOs.Models.FAQModels;
using Gold.ApplicationService.Contract.DTOs.Models.GoldPriceModels;
using Gold.ApplicationService.Contract.DTOs.Models.SettingsModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.FAQViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.GoldPriceViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.SettingsViewModels;
using Gold.Domain.Entities;
using Gold.Domain.Enums;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.Abstractions
{
    public interface ISettingService
    {
        Task<CommandResult<TModel>> GetSettingAsync<TModel>(SettingType type, CancellationToken cancellationToken=default);
        Task<CommandResult<TModel>> UpdateSettingAsync<TModel>(TModel model, SettingType type, CancellationToken cancellationToken = default);
        Task<CommandResult<TModel>> CreateSettingAsync<TModel>(TModel model, SettingType type, CancellationToken cancellationToken = default);
        CommandResult<IQueryable<SocialNetwork>> GetSocialNetworkAsIQueryable();
        Task<CommandResult<SocialNetworkViewModel>> AddSocialNetworkAsync(SocialNetworkViewModel model, CancellationToken cancellationToken = default);
        Task<CommandResult<SocialNetworkViewModel>> GetSocialNetworkByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<CommandResult<string>> RemoveSocialNetworkAsync(int socialNetworkId, CancellationToken cancellationToken = default);
        Task<CommandResult<SocialNetworkViewModel>> UpdateSocialNetworkAsync(SocialNetworkViewModel model, CancellationToken cancellationToken = default);
        CommandResult<string> GetMessageContentParameters(SettingType messageType);
        CommandResult<List<SelectListItem>> GetMessageTypesAsSelectListItem();
        CommandResult<List<SettingType>> GetMessageTypesAsList();
        /// <summary>
        /// for show in layout
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<CommandResult<List<SocialNetworkModel>>> GetAllSocialNetworkAsync(CancellationToken cancellationToken = default);
        Task<CommandResult<GoldPriceViewModel>> AddGoldPriceInfoAsync(GoldPriceViewModel model, CancellationToken cancellationToken);
        CommandResult<IQueryable<GoldPrice>> GetGoldPriceAsIQueryable();
        Task<CommandResult<string>> RemoveGoldPriceAsync(int goldPriceId, CancellationToken cancellationToken = default);
		Task<CommandResult<GoldPriceViewModel?>> GetLastGoldPriceInfoAsync(CancellationToken cancellationToken);
        Task<CommandResult<GoldPriceInfoModel>> GetGoldPriceInfoAsync(CancellationToken cancellationToken);
        CommandResult<List<SettingType>> GetSiteContentTypesAsList();
        Task<CommandResult<CalculatedGoldPriceModel>> GetGoldPriceAsync(GoldCalculatorViewModel model, CancellationToken cancellationToken);
        CommandResult<long> CalcInvoiceTotalPrice(decimal gramsGoldPrice, decimal weight, decimal wage, decimal stonePrice, decimal galleryProfit, decimal tax);
        decimal CalcGoldPrice(decimal amount, decimal weight);
        decimal CalcWageAmount(decimal goldPrice, decimal wage);
        decimal CalcGalleryProfitAmount(decimal stonePrice, decimal wageAmount, decimal goldPrice, decimal galleryProfit);
        decimal CalcTax(decimal wageAmount, decimal galleryProfitAmount, decimal tax);
        decimal InvoceTotalAmount(decimal goldPrice, decimal wageAmount, decimal galleryProfitAmount, decimal taxAmount, decimal stonePrice);

        //IQueryable<SocialNetwork> GetSocialNetwork2AsIQueryable();
    }
}
