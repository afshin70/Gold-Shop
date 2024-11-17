using Gold.ApplicationService.Contract.DTOs.Models.ArticleModels;
using Gold.ApplicationService.Contract.DTOs.Models.FAQModels;
using Gold.ApplicationService.Contract.DTOs.Models.SiteContent;
using Gold.ApplicationService.Contract.DTOs.ViewModels.ArticleViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.FAQViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.SiteContent;
using Gold.SharedKernel.DTO.OperationResult;

namespace Gold.ApplicationService.Contract.Abstractions
{
    public interface IContentManagerService
    {
        #region Article
        Task<CommandResult> ChangeArticleOrderNumberAsync(int id, bool isUp, CancellationToken cancellationToken);
        Task<CommandResult<string>> ChangeArticleStatusAsync(int id, CancellationToken cancellationToken);
        Task<CommandResult<CreateOrEditArticleViewModel>> CreateOrEditArticleAsync(CreateOrEditArticleViewModel model, CancellationToken cancellationToken);
        CommandResult<IQueryable<ArticleModel>> GetArticleAsQuerable();
        Task<CommandResult<ArticleModel>> GetArticleByIdAsync(int id, CancellationToken cancellationToken);
        Task<CommandResult<string>> RemoveArticleAsync(int id, CancellationToken cancellationToken);
        Task<CommandResult<List<ArticleForShowInSiteModel>>> GetArticlesForShoInSiteAsync(int page, int pageSize, string searchTerm, CancellationToken cancellationToken);
        #endregion

        #region Faq And Faq Category
        Task<CommandResult<FAQCategoryModel>> GetFAQCategoryByIdAsync(int id, CancellationToken cancellationToken);
        CommandResult<IQueryable<FAQCategoryModel>> GetFAQCategoryAsQuerable();
        Task<CommandResult<CreateOrEditFAQCategoryViewModel>> CreateOrEditFAQCategoryAsync(CreateOrEditFAQCategoryViewModel model, CancellationToken cancellationToken);
        Task<CommandResult<string>> RemoveFAQCategoryAsync(int id, CancellationToken cancellationToken);
        Task<CommandResult> ChangeFAQCategoryOrderNumberAsync(int id, bool isUp, CancellationToken cancellationToken);
        Task<CommandResult<FAQModel>> GetFAQByIdAsync(int id, CancellationToken cancellationToken);
        Task<CommandResult<CreateOrEditFAQViewModel>> CreateOrEditFAQAsync(CreateOrEditFAQViewModel model, CancellationToken cancellationToken);
        Task<CommandResult<string>> RemoveFAQAsync(int id, CancellationToken cancellationToken);
        CommandResult<IQueryable<FAQModel>> GetFAQAsQuerable();
        Task<CommandResult> ChangeFAQOrderNumberAsync(int id, bool isUp, CancellationToken cancellationToken);
        Task<CommandResult<CreateOrEditArticleMediaViewModel>> ChangeImageOrVideoAsync(CreateOrEditArticleMediaViewModel model, CancellationToken cancellationToken);
        Task<CommandResult> RemoveImageOrVideoAsync(int id, bool isVideo, CancellationToken cancellationToken);
        Task<CommandResult<ArticleForShowInSiteModel>> GetArticleInfoForShoInSiteAsync(int id, CancellationToken cancellationToken);
        Task<CommandResult<ContactUsViweModel>> CreateContactUsMessageAsync(ContactUsViweModel model, CancellationToken cancellationToken);
        CommandResult<IQueryable<ContactUsMessageModel>> GetContactUsMessagesAsQuerable();
        Task<CommandResult<ContactUsMessageModel>> RemoveMessageAsync(int id, CancellationToken cancellationToken);
        CommandResult<List<FAQModel>> GetFAQAsList();
        #endregion
    }
}