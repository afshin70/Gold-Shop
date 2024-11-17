using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.AspNetCore.Http;

namespace Gold.SharedKernel.Contract
{
    public interface ILogManager
    {
        CommandResult RaiseLog(Exception exception);
        CommandResult RaiseLog(Exception exception, string extraData);
        CommandResult RaiseWebRequestLog(Exception exception, HttpContext httpContext);
        Task<CommandResult> RaiseLogAsync(Exception exception, CancellationToken cancellationToken);
        Task<CommandResult> RaiseLogAsync(Exception exception, string extraData, CancellationToken cancellationToken);
        Task<CommandResult> RaiseWebRequestLogAsync(Exception exception, HttpContext httpContext, CancellationToken cancellationToken);
    }
}
