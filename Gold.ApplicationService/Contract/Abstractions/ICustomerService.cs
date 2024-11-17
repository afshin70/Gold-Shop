using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels;
using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.CustomerMessageModels;
using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.CustomerPymentsModel;
using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.DocumentModels;
using Gold.ApplicationService.Contract.DTOs.Models.DocumentModels;
using Gold.ApplicationService.Contract.DTOs.Models.ProductModels;
using Gold.ApplicationService.Contract.DTOs.Models.UserModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.CustomerPaymentsViewModel;
using Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.DocumentViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.SendMessageViewModels;
using Gold.Domain.Entities;
using Gold.Domain.Enums;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.DTO.PaginationModels;
using Gold.SharedKernel.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.Interfaces
{
    public interface ICustomerService
    {
        Task<CommandResult<EssentialTelViewModel>> AddEssentialTelAsync(EssentialTelViewModel model, CancellationToken cancellationToken);
        Task<CommandResult<CreateOrEditCustomerViewModel>> CreateCustomerAsync(CreateOrEditCustomerViewModel model, CancellationToken cancellationToken);
        Task<CommandResult<CreateOrEditCustomerViewModel>> GetCustomerInfoForEditAsync(int customerId, CancellationToken cancellationToken);
       CommandResult<IQueryable<CustomerModel>> GetCustomerListAsQuerable();
        Task<CommandResult<EssentialTelModel>> GetEssentialTelAsync(int id, CancellationToken cancellationToken);
        Task<CommandResult<int>> GetUserIdOfCustomerAsync(int customerId, CancellationToken cancellationToken);
        Task<CommandResult> IsExistCustomerByIdAsync(int id, CancellationToken cancellationToken);
        Task<CommandResult<CreateOrEditCustomerViewModel>> UpdateCustomerAsync(CreateOrEditCustomerViewModel model, CancellationToken cancellationToken);
        Task<CommandResult> ChangeEssentialTelOrderNumberAsync(long id,bool isUp, CancellationToken cancellationToken);
        CommandResult<IQueryable<EssentialTelModel>> GetEssentialTelListAsQuerable();
        Task<CommandResult<List<EssentialTelModel>>> GetCustomerEssentialTelListAsync(int customerId, CancellationToken cancellationToken);
        Task<CommandResult<EssentialTelViewModel>> EditEssentialTelAsync(EssentialTelViewModel model, CancellationToken cancellationToken);
        Task<CommandResult<EssentialTelModel>> GetEssentialTelAsync(long id, int customerId, CancellationToken cancellationToken);
        Task<CommandResult<string>> ResetCustomerPasswordAsync(int customerId, CancellationToken cancellationToken);
        Task<CommandResult<string>> RemoveCustomerAsync(int customerId, CancellationToken cancellationToken);
        Task<CommandResult<string>> RemoveEssentialNumberAsync(long id, int customerId, CancellationToken cancellationToken);
        
        Task<CommandResult<int>> GetCustomerIdByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken);
        Task<CommandResult<bool>> IsExistDocumentByStatusAnNumberAsync(DocumentStatus documentStatus, int documentNumber, CancellationToken cancellationToken);
        //Task<CommandResult<DocumentOwnerInformationModel>> GetDocumentOwnerInfoAsync(int customerId, CancellationToken cancellationToken);
        Task<CommandResult<DocumentOwnerInformationModel>> GetDocumentOwnerInfoAsync(string nationalCose, CancellationToken cancellationToken);
        Task<CommandResult<bool>> IsExistCustomerByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken);


        Task<CommandResult<long>> InstallmentAmountCalculator(long remainAmount,byte installmentCount, CancellationToken cancellationToken);
        Task<CommandResult<bool>> IsActiveCustomerByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken);
        CommandResult<List<InstallmentDateModel>> CalculateInstallmentDate(byte installmentCount, string installmentDate);
        Task<CommandResult<List<SelectListItem>>> GetCollateralTypesAsync(int selectedId, CancellationToken cancellationToken=default);
        Task<CommandResult<EditCustomerSummaryInfoViewModel>> GetCustomerSummaryInfo(string nationalCode, CancellationToken cancellationToken);
        Task<CommandResult<EditCustomerSummaryInfoViewModel>> UpdateCustomerSummaryInfoAsync(EditCustomerSummaryInfoViewModel model, CancellationToken cancellationToken);
        Task<CommandResult<CreateDocumentViewModel>> CreateDocumentAsync(CreateDocumentViewModel model, CancellationToken cancellationToken);
        CommandResult<IQueryable<DocumentModel>> GetDocumentListAsQuerable(SearchDocumentViewModel model);

        List<SelectListItem> GetDocumentStatusSelectListItems(int selectedItem = 0);
        Task<CommandResult<List<SelectListItem>>> GetCollateralsTypeSelectListItemsAsync(int selectedItem = 0, CancellationToken cancellationToken = default);
        Task<CommandResult<DocumentDetailModel>> GetDocumentDetailAsync(long id, CancellationToken cancellationToken);
        Task<CommandResult<string>> GetDocumentDescriptionAsync(long documentId, CancellationToken cancellationToken);
        Task<CommandResult<EditDocumentDescriptionViewModel>> UpdateDocumentAdminDescriptionAsync(EditDocumentDescriptionViewModel model, CancellationToken cancellationToken);
        Task<CommandResult<InstallmentPymentsDetailModel>> GetInstallmentPaymentsAsync(long installmentId, CancellationToken cancellationToken);
        Task<CommandResult<CreateOrEditCollateralViewModel>> GetCollateralForEditAsync(long documentId, long collateralId, CancellationToken cancellationToken);
        CommandResult<List<CollateralModel>> GetCollaterals(long documentId);
        Task<CommandResult<CreateOrEditCollateralViewModel>> UpdateCollateralAsync(CreateOrEditCollateralViewModel model, CancellationToken cancellationToken);
        Task<CommandResult<CreateOrEditCollateralViewModel>> AddCollateralAsync(CreateOrEditCollateralViewModel model, CancellationToken cancellationToken);
        Task<CommandResult<string>> RemoveCollateralAsync(long id, long documentId, CancellationToken cancellationToken);
        Task<CommandResult<CreateOrEditPaymentViewModel>> GetPaymentForEditAsync(long paymentId, long instllmentId, CancellationToken cancellationToken);
        List<SelectListItem> GetPamentTypesSelectListItem(PaymentType? selectedItem);
        Task<CommandResult<List<PaymentModel>>> GetPaymentsByInstallmentIdAsync(long instllmentId, CancellationToken cancellationToken);
        Task<CommandResult<CreateOrEditPaymentViewModel>> CreateOrEditPaymentAsync(CreateOrEditPaymentViewModel model, CancellationToken cancellationToken);
        Task<CommandResult<InstallmentDetailModel>> GetInstallmentDetailAsync(long installmentId, CancellationToken cancellationToken=default);
        Task<CommandResult<List<InstallmentModel>>> GetInstallmentsAsync(long documentId, CancellationToken cancellationToken);
        Task<CommandResult<bool>> IsPayInstallmentAsync(long installmentId, CancellationToken cancellationToken);
        Task<CommandResult<bool>> RemoveInstallmentPayment(long installmentId, long paymentId, LogUserActivityModel logUserActivity, CancellationToken cancellationToken);
        Task<CommandResult<bool>> IsPayableInstallmentAsync(long installmentId, CancellationToken cancellationToken);
        Task<CommandResult<int>> RemoveDocumentAsync(long documentId, CancellationToken cancellationToken);
        Task<CommandResult<InstallmentInfo>> GetInstallmentsInfoAsync(long documentId, CancellationToken cancellationToken);
        Task<CommandResult<List<CollateralInfoModel>>> GetCollateralInfoAsync(long documentId, CancellationToken cancellationToken);
        Task<CommandResult<DocumentStatus>> GetDocumentStatusByIdAsync(long documentId, CancellationToken cancellationToken);
        Task<CommandResult<int>> SettleDocumentAsync(SettleDocumentViewModel model, CancellationToken cancellationToken);
        Task<CommandResult<SettleDocumentViewModel>> GetDocumentForSettleAsync(long documentId, CancellationToken cancellationToken);
        //Task<CommandResult> UnPaymentDocumentInstallment(long installmentId, long documentId, CancellationToken cancellationToken);
        Task<CommandResult> UnPaymentDocumentInstallment(long installmentId, long documentId, LogUserActivityModel logUserActivity, CancellationToken cancellationToken);
        Task<CommandResult<bool>> IsUnPayableInstallmentAsync(long installmentId, CancellationToken cancellationToken);
        Task<CommandResult<int>> UnSettleDocumentAsync(long documentId, CancellationToken cancellationToken);
        Task<CommandResult<EditDocumentViewModel>> GetDocumentForEditAsync(long documentId, CancellationToken cancellationToken);
        Task<CommandResult<EditDocumentViewModel>> EditDocumentInfoAsync(EditDocumentViewModel model, CancellationToken cancellationToken);
        CommandResult<IQueryable<PaymentReceiptsInPendingConfirmationModel>> GetPaymentReceiptsInPendingConfirmationListAsQuerable(PaymentReceiptsInPendingConfirmationSearchViewModel model);
        Task<CommandResult<ConfirmCustomerPaymentViewModel>> GetCustomerPaymentForConfirmAsync(long customerPayentId, CancellationToken cancellationToken);
        Task<CommandResult<ConfirmCustomerPaymentViewModel>> ConfirmCustomerPaymentAsync(ConfirmCustomerPaymentViewModel model, LogUserActivityModel logUserActivity, CancellationToken cancellationToken);
        Task<CommandResult> RemoveCustomerPaymentAsync(long customerPaymentId, LogUserActivityModel logModel, CancellationToken cancellationToken);
        CommandResult<IQueryable<ConfirmedPaymentReport>> GetPaymentListAsQuerable(PaymentReportSearchViewModel model);
        Task<CommandResult<string>> GetFullNameByIdAsync(int customerId, CancellationToken cancellationToken);
        Task<CommandResult<string>> GetFullNameByDocumentIdAsync(long documentId, CancellationToken cancellationToken);
        Task<CommandResult<int>> GetDocumentNumberByIdAsync(long documentId, CancellationToken cancellationToken);
        Task<CommandResult<string>> GetFullNameByEditInformationRequestIdAsync(int id, CancellationToken cancellationToken);
        Task<CommandResult<long>> InstallmentAmountSumAsync(SearchDocumentViewModel model, CancellationToken cancellationToken);
        Task<CommandResult<long>> RemainAmountSumAsync(SearchDocumentViewModel model, CancellationToken cancellationToken);
		Task<CommandResult<long>> GetDocumentIdByNumberAsync(int documentNo, CancellationToken cancellationToken);

        Task<CommandResult<List<InstallmentPaymentReminderModel>>> GetAllInstallmentsForReminderAsync(DateTime date, CancellationToken cancellationToken = default);
		Task<CommandResult> SendMessageToCustomerAsync(CreateCustomerMessageModel model, bool saveData = false, CancellationToken cancellationToken = default);
		Task<string> GenerateMessageAsync(SettingType messageType, Dictionary<string, string> parameters);
		Task<CommandResult<bool>> IsFirstUnPayedInstallmentAsync(long installmentId, CancellationToken cancellationToken);
		Task<CommandResult<CustomerProfileModel>> GetCustomerProfileInfoAsync(string userName, CancellationToken cancellationToken=default);
        Task<CommandResult<List<DocumentInfoModel>>> GetDocumentsInfoAsync(string userName, CancellationToken cancellationToken);
        Task<CommandResult<CustomerDocumentDetailModel>> GetCustomerDocumentDetailAsync(string userName, long documentId, CancellationToken cancellationToken);
        Task<CommandResult<List<CustomerPaymentModel>>> GetCustomerPaymentsAsync(long documentId, CancellationToken cancellationToken);
        Task<CommandResult<DocumentInfoForCustomerPayment>> GetDocumentInfoForCustomerPaymentAsync(long documentId, CancellationToken cancellationToken);
        Task<CommandResult> CreateCustomerPaymentAsync(CustomerPaymentViewModel model, CancellationToken cancellationToken);
        //Task<CommandResult> HaveDocumentsAsync(string userName, CancellationToken cancellationToken=default);
        Task<CommandResult<Pagination<CustomerMessageModel>>> GetMessagesAsync(string userName,int page, CancellationToken cancellationToken);
        Task<CommandResult<CustomerProfileViewModel>> EditProfileAsync(CustomerProfileViewModel model, string userName, CancellationToken cancellationToken);
        Task<CommandResult<EditInformationRequestViewModel>> EditInformationRequestAsync(EditInformationRequestViewModel model, string userName, CancellationToken cancellationToken);
        Task<CommandResult> HaveDocumentAsync(string userName, CancellationToken cancellationToken = default);
		Task<CommandResult> IsExistCustomerByMobileAsync(int id, string mobile, CancellationToken cancellationToken = default);
		Task<CommandResult<CalculationInstallmentDelayModel>> CalculationInstallmentDelayInfoAsync(long installmentId,long selectedPaymentId, string paymentDate, CancellationToken cancellationToken);
        Task<CommandResult<string>> GetCollateralImageAsync(long documentId, string userName, CancellationToken cancellationToken);
		Task<CommandResult<string>> GeneratePaymentDescriptionAsync(long installmentId, long paymentId, long newAmount, CancellationToken cancellationToken);
        CommandResult<IQueryable<CustomerReportModel>> GetCustomerReportListAsQuerable();
        Task<CommandResult<string>> GetCustomerAccountStatus(int customerId, CancellationToken cancellationToken);
		Task<CommandResult<RejectionCustomerPaymentViewModel>> RejectCustomerPaymentAsync(RejectionCustomerPaymentViewModel model, LogUserActivityModel logModel, CancellationToken cancellationToken);
		Task<CommandResult<RejectionCustomerPaymentViewModel>> GetCustomerPaymentForRejectAsync(long customerPayentId, CancellationToken cancellationToken);
        Task<CommandResult<List<CardNumberModel>>> GetCustomerCardNumberListAsync(int customerId, CancellationToken cancellationToken);
        CommandResult<IQueryable<CardNumberModel>> GetCardNumberListAsQuerable();
        Task<CommandResult<CardNumberModel>> GetCardNumberAsync(long id, int customerId, CancellationToken cancellationToken);
        Task<CommandResult<CardNumberViewModel>> AddCardNumberAsync(CardNumberViewModel model, CancellationToken cancellationToken);
        Task<CommandResult<CardNumberViewModel>> EditCardNumberAsync(CardNumberViewModel model, CancellationToken cancellationToken);
        Task<CommandResult<string>> RemoveCardNumberAsync(long id, int customerId, CancellationToken cancellationToken);
        Task<CommandResult> ChangeCardNumberOrderNumberAsync(long id, bool isUp, CancellationToken cancellationToken);
        Task<CommandResult<List<ProfileImagesModel>>> GetProfileImagesAsync(int customerId, CancellationToken cancellationToken);
        Task<CommandResult<string>> RemoveProfileImageAsync(long id, int customerId, CancellationToken cancellationToken);
		Task<CommandResult<long>> GetDocumentSumOfAmount(long documentId, CancellationToken cancellationToken);
		/// <summary>
		/// مبلغ مانده کل تا قسط فعلی
		/// </summary>
		/// <param name="installmentId">ای دی قسط فعلی</param>
		/// <param name="cancellationToken"></param>
		/// <returns>مبلغ مانده کل با محاسبه دیرکرد</returns>
		Task<CommandResult<long>> GetSumOfRemainAmountToInstallment(long installmentId,long paymentId=0, CancellationToken cancellationToken=default);
		Task<CommandResult<PaymentDescriptionWithMessageModel>> GeneratePaymentDescriptionWithMessageAsync(long installmentId, long paymentId, long newAmount, int delayDay, CancellationToken cancellationToken);
        CommandResult<IQueryable<CustomerBankCardNoModel>> GetCustomerBankCardNumberListAsQuerable(string? BankCardNumber);
        CommandResult<IQueryable<CustomerPhoneNumberModel>> GetCustomerPhoneNumberListAsQuerable(string? PhoneNumber);
        Task<CommandResult<List<BirthdayMessageModel>>> GetCustomerInfoForBirthdayMessage(CancellationToken cancellationToken = default);
        Task<CommandResult<List<SendMessageModel>>> GetCustomerListForSendMessageAsync(SendMessageViewModel model, CancellationToken cancellationToken);
        Task<CommandResult> SendMessageToCustomersAsync(SendMessageContentViewModel model, CancellationToken cancellationToken);
        Task<CommandResult<ProfileImagesModel>> GetProfileImageByUserIdAsync(int userId, CancellationToken cancellationToken);
		Task<CommandResult<List<CardNumberModel>>> GetCustomerCardNumberListByUserIdAsync(int userId, CancellationToken cancellationToken);
		Task<CommandResult<int?>> GetCustomerIdByUserIdAsync(int userId, CancellationToken cancellationToken);
        Task<CommandResult<InstantSettlementModel>> GetInstantSettlementInfoAsync(long documentId, CancellationToken cancellationToken);
		Task<CommandResult<SettleDocumentMessageModel>> GenerateSettleDocumentMessageAsync(long documentId,string settleDate, string deliveryDate, long returnedAmount, long discountAmount, CancellationToken cancellationToken);
        Task<CommandResult<bool>> IsCheackPaymentDateAsync(long installmentId, long paymentId, DateTime paymentDate, CancellationToken cancellationToken);
        Task<CommandResult<bool>> HasDocumentAsync(int userId, CancellationToken cancellationToken = default);
        Task<CommandResult> AddProductToFavoritsAsync(long productId, int userId, CancellationToken cancellationToken);
        Task<CommandResult> RemoveProductFromFavoritsAsync(long productId, int userId, CancellationToken cancellationToken);
        Task<CommandResult<List<ProductForShowInSiteModel>>> GetAllProductsInFavoritAsync(int userId, CancellationToken cancellationToken);
        Task<CommandResult<bool>> IsProductInFavoritsAsync(long id, int userId);
    }
}
