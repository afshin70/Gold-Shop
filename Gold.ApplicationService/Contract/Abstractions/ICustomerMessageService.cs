using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.CustomerMessageModels;
using Gold.SharedKernel.DTO.OperationResult;

namespace Gold.ApplicationService.Contract.Interfaces
{
	public interface ICustomerMessageService
	{
		Task<CommandResult> CreateAsync(CreateCustomerMessageModel model, bool saveNow = true, CancellationToken cancellationToken = default);
	}
}
