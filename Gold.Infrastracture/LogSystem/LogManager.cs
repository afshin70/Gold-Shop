using Gold.Domain.Contract.IRepositories.Logging;
using Gold.Domain.Entities.LoggingEntities;
using Gold.Infrastracture.EFCoreContext;
using Gold.Infrastracture.Repositories;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.Convertor;
using Gold.SharedKernel.DTO.LogModels;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.ExtentionMethods;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gold.Infrastracture.LogSystem
{
    public class LogManager : ILogManager
    {
        private readonly ApplicationLoggerContext _context;
        public LogManager(ApplicationLoggerContext context)
        {
            _context = context;
        }
        public CommandResult RaiseLog(Exception exception)
        {

            try
            {
                var model = new SystemActivityLog
                {
                    Message = exception.Message,
                    RaiseDate = DateTime.Now,
                    Source = string.IsNullOrEmpty(exception.Source) ? string.Empty : exception.Source,
                    StackTrace = string.IsNullOrEmpty(exception.StackTrace) ? string.Empty : exception.StackTrace,
                    ExtraData = exception.InnerException is null ? string.Empty : exception.InnerException.GetAllMessages(),
                };
                _context.SystemActivityLogs.Add(model);
                _context.SaveChanges();
                return CommandResult.Success(OperationResultMessage.OperationIsSuccessfullyCompleted);
            }
            catch (Exception)
            {
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }

        public CommandResult RaiseLog(Exception exception, string extraData)
        {
            try
            {
                var model = new SystemActivityLog
                {
                    Message = exception.Message,
                    RaiseDate = DateTime.Now,
                    Source = string.IsNullOrEmpty(exception.Source) ? string.Empty : exception.Source,
                    StackTrace = string.IsNullOrEmpty(exception.StackTrace) ? string.Empty : exception.StackTrace,
                    ExtraData = extraData,
                };
                _context.SystemActivityLogs.Add(model);
                _context.SaveChanges();
                return CommandResult.Success(OperationResultMessage.OperationIsSuccessfullyCompleted);
            }
            catch (Exception)
            {
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }

        public CommandResult RaiseWebRequestLog(Exception exception, HttpContext httpContext)
        {
            try
            {
                var webRequestModel = new WebRequestModel
                {
                    IP = httpContext.Connection.RemoteIpAddress == null ? string.Empty : httpContext.Connection.RemoteIpAddress.ToString(),
                    Method = httpContext.Request.Method,
                    Path = httpContext.Request.Path,
                    QueryString = string.IsNullOrEmpty(httpContext.Request.QueryString.Value) ? string.Empty : httpContext.Request.QueryString.Value,
                    StatuseCode = httpContext.Response.StatusCode,
                    UserAgent = httpContext.Request.Headers["User-Agent"].ToString(),
                    UserName = string.IsNullOrEmpty(httpContext.User.Identity?.Name) ? string.Empty : httpContext.User.Identity.Name
                };
                string? jsonExtraData = webRequestModel.ConvertObjectToJson();
                string extraData = string.IsNullOrEmpty(jsonExtraData) ? string.Empty : jsonExtraData;
                var model = new SystemActivityLog
                {
                    Message = exception.Message,
                    RaiseDate = DateTime.Now,
                    Source = string.IsNullOrEmpty(exception.Source) ? string.Empty : exception.Source,
                    StackTrace = string.IsNullOrEmpty(exception.StackTrace) ? string.Empty : exception.StackTrace,
                    ExtraData = extraData,
                };
                _context.SystemActivityLogs.Add(model);
                _context.SaveChanges();
                return CommandResult.Success(OperationResultMessage.OperationIsSuccessfullyCompleted);
            }
            catch (Exception)
            {
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }

        public async Task<CommandResult> RaiseLogAsync(Exception exception, CancellationToken cancellationToken)
        {

            try
            {
                var model = new SystemActivityLog
                {
                    Message = exception.Message,
                    RaiseDate = DateTime.Now,
                    Source = string.IsNullOrEmpty(exception.Source) ? string.Empty : exception.Source,
                    StackTrace = string.IsNullOrEmpty(exception.StackTrace) ? string.Empty : exception.StackTrace,
                    ExtraData = exception.InnerException is null ? string.Empty : exception.InnerException.GetAllMessages(),
                };
                await _context.SystemActivityLogs.AddAsync(model, cancellationToken);
                await _context.SaveChangesAsync();
                return CommandResult.Success(OperationResultMessage.OperationIsSuccessfullyCompleted);
            }
            catch (Exception)
            {
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }

        public async Task<CommandResult> RaiseLogAsync(Exception exception, string extraData, CancellationToken cancellationToken)
        {
            try
            {
                var model = new SystemActivityLog
                {
                    Message = exception.Message,
                    RaiseDate = DateTime.Now,
                    Source = string.IsNullOrEmpty(exception.Source) ? string.Empty : exception.Source,
                    StackTrace = string.IsNullOrEmpty(exception.StackTrace) ? string.Empty : exception.StackTrace,
                    ExtraData = extraData.IsEmptyOrNull()?string.Empty: extraData,
                };
                await _context.SystemActivityLogs.AddAsync(model, cancellationToken);
                await _context.SaveChangesAsync();
                return CommandResult.Success(OperationResultMessage.OperationIsSuccessfullyCompleted);
            }
            catch (Exception)
            {
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }

        public async Task<CommandResult> RaiseWebRequestLogAsync(Exception exception, HttpContext httpContext, CancellationToken cancellationToken)
        {
            try
            {
                var webRequestModel = new WebRequestModel
                {
                    IP = httpContext.Connection.RemoteIpAddress == null ? string.Empty : httpContext.Connection.RemoteIpAddress.ToString(),
                    Method = httpContext.Request.Method,
                    Path = httpContext.Request.Path,
                    QueryString = string.IsNullOrEmpty(httpContext.Request.QueryString.Value) ? string.Empty : httpContext.Request.QueryString.Value,
                    StatuseCode = httpContext.Response.StatusCode,
                    UserAgent = httpContext.Request.Headers["User-Agent"].ToString(),
                    UserName = string.IsNullOrEmpty(httpContext.User.Identity?.Name) ? string.Empty : httpContext.User.Identity.Name
                };
                string jsonExtraData = webRequestModel.ConvertObjectToJson();
                string extraData = string.IsNullOrEmpty(jsonExtraData) ? string.Empty : jsonExtraData;
                var model = new SystemActivityLog
                {
                    Message = exception.Message,
                    RaiseDate = DateTime.Now,
                    Source = string.IsNullOrEmpty(exception.Source) ? string.Empty : exception.Source,
                    StackTrace = string.IsNullOrEmpty(exception.StackTrace) ? string.Empty : exception.StackTrace,
                    ExtraData = extraData,
                };
                await _context.SystemActivityLogs.AddAsync(model, cancellationToken);
                await _context.SaveChangesAsync();
                return CommandResult.Success(OperationResultMessage.OperationIsSuccessfullyCompleted);
            }
            catch (Exception)
            {
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }

    }
}
