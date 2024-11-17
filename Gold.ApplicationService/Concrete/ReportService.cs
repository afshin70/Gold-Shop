using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.DocumentModels;
using Gold.ApplicationService.Contract.DTOs.Models.DocumentModels;
using Gold.ApplicationService.Contract.DTOs.Models.ReportModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.AuthenticationViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.DocumentViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.ReportViewModels;
using Gold.Infrastracture.Repositories.UOW;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.ExtentionMethods;
using Gold.SharedKernel.Tools;
using Microsoft.EntityFrameworkCore;

namespace Gold.ApplicationService.Concrete
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogManager _logManager;

        public ReportService(IUnitOfWork unitOfWork, ILogManager logManager)
        {
            _unitOfWork = unitOfWork;
            _logManager = logManager;
        }


        public CommandResult<IQueryable<ConfirmedPaymentReport>> GetPaymentsReport(PaymentReportSearchViewModel model)
        {
            try
            {
                var result = _unitOfWork.PaymentRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    var query = result.Data
                        .Include(x => x.Installment)
                        .ThenInclude(c => c.Document)
                        .ThenInclude(x => x.Customer)
                        .ThenInclude(x => x.User)
                        .AsQueryable();

                    if (model.DocumentNumber.HasValue)
                        query = query.Where(x => x.Installment.Document.DocumentNo == model.DocumentNumber.Value);

                    if (!string.IsNullOrEmpty(model.FromDatePayment) & !string.IsNullOrEmpty(model.ToDatePayment))
                    {
                        DateTime? fromDate = model.FromDatePayment.ParsePersianToGorgian();
                        DateTime? toDate = model.ToDatePayment.ParsePersianToGorgian();
                        if (toDate.HasValue & fromDate.HasValue)
                        {
                            query = query.Where(x => x.Date.Date >= fromDate.Value.Date & x.Date.Date <= toDate.Value.Date);
                        }
                    }
                    else if (!string.IsNullOrEmpty(model.FromDatePayment))
                    {
                        DateTime? fromDate = model.FromDatePayment.ParsePersianToGorgian();
                        if (fromDate.HasValue)
                        {
                            query = query.Where(x => x.Date.Date >= fromDate.Value.Date);
                        }
                    }
                    else if (!string.IsNullOrEmpty(model.ToDatePayment))
                    {
                        DateTime? toDate = model.ToDatePayment.ParsePersianToGorgian();
                        if (toDate.HasValue)
                        {
                            query = query.Where(x => x.Date.Date <= toDate.Value.Date);
                        }
                    }

                    IQueryable<ConfirmedPaymentReport> finallQuery = query.OrderByDescending(x => x.Date)
                        .Select(x => new ConfirmedPaymentReport
                        {
                            Id = x.Id,
                            FullName = x.Installment.Document.Customer.User.FullName,
                            DocumentNumber = x.Installment.Document.DocumentNo,
                            InstallmentAmount = x.Installment.Amount,
                            InstallmentNumber = x.Installment.Number,
                            PaymentAmount = x.Amount,
                            PersianInstallmentDate = x.Installment.Date.GeorgianToPersian(ShowMode.OnlyDate),
                            PersianPaymentDate = x.Date.GeorgianToPersian(ShowMode.OnlyDate),
                            ImageName = x.ImageName ?? string.Empty,
                        }).AsQueryable();

                    return CommandResult<IQueryable<ConfirmedPaymentReport>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, finallQuery);
                }
                else
                {
                    return CommandResult<IQueryable<ConfirmedPaymentReport>>.Failure(result.Message, null);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<ConfirmedPaymentReport>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public CommandResult<List<AdminMenuModel>> GetManagersOperationReportAdminMenus()
        {
            try
            {
                var result = _unitOfWork.AdminActivityRepository.GetAllAsIQueryable().Data
                    .Include(x => x.AdminMenu)
                    .Select(x => new AdminMenuModel
                    {
                        Id = x.AdminMenu.Id,
                        ActionName = x.AdminMenu.ActionName,
                        ControllerName = x.AdminMenu.ControllerName,
                        IconName = x.AdminMenu.IconName,
                        MenuGroupId = x.AdminMenu.AdminMenuGroupID,
                        Title = x.AdminMenu.Title,
                    })
                    .Distinct()
                    .ToList();
                result ??= new List<AdminMenuModel>();
                return CommandResult<List<AdminMenuModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, result);

            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<List<AdminMenuModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }

        public CommandResult<IQueryable<ManagersOperationReportModel>> GetManagersOperationReport(ManagersOperationReportViewModel model)
        {
            try
            {
                var result = _unitOfWork.AdminActivityRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    var query = result.Data
                        .Include(x => x.User)
                        .Include(c => c.AdminMenu)
                        .Where(x => 
                        (x.User.UserType == UserType.Admin & x.User.UserName == "siteAdmin") ||
                        x.User.UserType == UserType.Seller ||
                        x.User.UserType == UserType.Manager)
                        .AsQueryable();

                    if (model.UserId.HasValue)
                        query = query.Where(x => x.UserId == model.UserId.Value);

                    if (model.AdminMenuId.HasValue)
                        query = query.Where(x => x.AdminMenuID == model.AdminMenuId.Value);

                    if (!string.IsNullOrEmpty(model.Description))
                        query = query.Where(x => x.Description.Contains(model.Description));

                    if (model.ActivityType.HasValue)
                        query = query.Where(x => x.ActivityType == model.ActivityType.Value);

                    if (!string.IsNullOrEmpty(model.FromDate) | !string.IsNullOrEmpty(model.ToDate))
                    {
                        DateTime? fromDate = model.FromDate.ParsePersianToGorgian();
                        DateTime? toDate = model.ToDate.ParsePersianToGorgian();

                        if (fromDate.HasValue & toDate.HasValue)
                            query = query.Where(i => i.Date >= fromDate.Value & i.Date <= toDate.Value);

                        else if (fromDate.HasValue)
                            query = query.Where(i => i.Date >= fromDate.Value);

                        else if (toDate.HasValue)
                            query = query.Where(i => i.Date <= toDate.Value);
                    }

                    var finallQuery = query
                         .OrderByDescending(x => x.Date)
                         .Select(x => new ManagersOperationReportModel
                         {
                             Id = x.Id,
                             UserName = x.User.FullName,
                             Description = x.Description,
                             Operation = x.ActivityType.GetDisplayName(),
                             Page = x.AdminMenu.Title,
                             PersianDate = x.Date.GeorgianToPersian(ShowMode.OnlyDateAndTime)
                         })
                         .AsQueryable();

                    return CommandResult<IQueryable<ManagersOperationReportModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, finallQuery);
                }
                else
                {
                    return CommandResult<IQueryable<ManagersOperationReportModel>>.Failure(result.Message, null);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<ManagersOperationReportModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<long>> PaymentAmountSumAsync(PaymentReportSearchViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                var result = _unitOfWork.PaymentRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    var query = result.Data
                        .Include(x => x.Installment)
                        .ThenInclude(c => c.Document)
                        .ThenInclude(x => x.Customer)
                        .ThenInclude(x => x.User)
                        .AsQueryable();

                    if (model.DocumentNumber.HasValue)
                        query = query.Where(x => x.Installment.Document.DocumentNo == model.DocumentNumber.Value);

                    if (!string.IsNullOrEmpty(model.FromDatePayment) & !string.IsNullOrEmpty(model.ToDatePayment))
                    {
                        DateTime? fromDate = model.FromDatePayment.ParsePersianToGorgian();
                        DateTime? toDate = model.ToDatePayment.ParsePersianToGorgian();
                        if (toDate.HasValue & fromDate.HasValue)
                        {
                            query = query.Where(x => x.Date.Date >= fromDate.Value.Date & x.Date.Date <= toDate.Value.Date);
                        }
                    }
                    else if (!string.IsNullOrEmpty(model.FromDatePayment))
                    {
                        DateTime? fromDate = model.FromDatePayment.ParsePersianToGorgian();
                        if (fromDate.HasValue)
                        {
                            query = query.Where(x => x.Date.Date >= fromDate.Value.Date);
                        }
                    }
                    else if (!string.IsNullOrEmpty(model.ToDatePayment))
                    {
                        DateTime? toDate = model.ToDatePayment.ParsePersianToGorgian();
                        if (toDate.HasValue)
                        {
                            query = query.Where(x => x.Date.Date <= toDate.Value.Date);
                        }
                    }

                    var sumAmount = await query.SumAsync(x => x.Amount, cancellationToken);
                    return CommandResult<long>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, sumAmount);
                }
                else
                {
                    return CommandResult<long>.Failure(result.Message, 0);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<long>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, 0);
            }
        }
    }
}
