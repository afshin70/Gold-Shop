using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.CustomerMessageModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels;
using Gold.ApplicationService.Contract.Interfaces;
using Gold.Domain.Entities;
using Gold.Domain.Enums;
using Gold.Infrastracture.Repositories.UOW;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using System.Threading;

namespace Gold.ApplicationService.Imp
{
	public class CustomerMessageService : ICustomerMessageService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogManager _logManager;
		private readonly ISettingService _settingService;


		public CustomerMessageService(IUnitOfWork unitOfWork, ILogManager logManager, ISettingService settingService)
		{
			_unitOfWork = unitOfWork;
			_logManager = logManager;
			_settingService = settingService;
		}

		public async Task<CommandResult> CreateAsync(CreateCustomerMessageModel model, bool saveNow = true, CancellationToken cancellationToken = default)
		{
			try
			{
				var newCustomerMessage = new CustomerMessage
				{
					CustomerId = model.CustomerId,
					Date = DateTime.Now,
					DocumentId = model.DocumentId,
					installmentId = model.installmentId,
					Title = model.Title,
					Type=model.MessageType,
				};
				newCustomerMessage.Message =await GenerateMessageAsync(model.MessageSettingType, model.Parameters);
				var addResult = await _unitOfWork.CustomerMessageRepository.InsertAsync(newCustomerMessage, cancellationToken);
				if (addResult.IsSuccess)
				{
					if (saveNow)
					{
						var result = await _unitOfWork.CommitChangesAsync(cancellationToken);
						if (result.IsSuccess)
							return CommandResult.Failure(result.Message);
						else
							return CommandResult.Failure(result.Message);
					}
					else
						return CommandResult.Success(OperationResultMessage.OperationIsSuccessfullyCompleted);
				}
				else
					return CommandResult.Failure(addResult.Message);
			}
			catch (Exception ex)
			{
				await _logManager.RaiseLogAsync(ex, cancellationToken);
				return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
			}
		}
		private async Task<string> GenerateMessageAsync(SettingType messageType,Dictionary<string,string> parameters)
		{
			string messageContent=string.Empty;

			if (messageType == SettingType.MessageType_DocumentRegistration |
				messageType == SettingType.MessageType_ReminderOfTheDayBeforeTheInstallmentDate |
				messageType == SettingType.MessageType_Reminder7DaysAfterTheUnpaidInstallmentDate |
				messageType == SettingType.MessageType_DeficitPayment |
				messageType == SettingType.MessageType_Payment |
				messageType == SettingType.MessageType_SettleDocument |
				messageType == SettingType.MessageType_OverPayment)
			{
				var settingResult = await _settingService.GetSettingAsync<string>(messageType);
				messageContent = settingResult.Data;
			}

			foreach (var param in parameters)
			{
				string index = "{" + param.Key + "}";
				messageContent = messageContent.Replace(index, param.Value);
			}
			return messageContent;
		}
	}
}

