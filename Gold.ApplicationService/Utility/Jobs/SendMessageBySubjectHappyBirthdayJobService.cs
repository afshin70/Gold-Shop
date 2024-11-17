using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.CustomerMessageModels;
using Gold.ApplicationService.Contract.Interfaces;
using Gold.Domain.Enums;
using Gold.Resources;
using Gold.SharedKernel.Enums;

namespace Gold.ApplicationService.Utility.Jobs
{
    public class SendMessageBySubjectHappyBirthdayJobService
    {
		private readonly ICustomerService _customerService;
		public SendMessageBySubjectHappyBirthdayJobService(ICustomerService customerService)
		{
			_customerService = customerService;
        }

        public async Task SendMessageBySubjectHappyBirthdayAsync()
		{
           
            var customerInfoResult = await _customerService.GetCustomerInfoForBirthdayMessage();
		
			if (customerInfoResult.IsSuccess)
			{
                //جنسیت {0} - نام و نام خانوداگی {1}  - سال تولد {2} - اسم ماه تولد {3} - شماره ماه تولد {4} - روز تولد {5} -شماره روز تولد {6}
                Dictionary<string, string> messageParameters = new Dictionary<string, string>();
				List<CreateCustomerMessageModel> createCustomerMessages= new List<CreateCustomerMessageModel>();
					foreach (var item in customerInfoResult.Data)
					{
						messageParameters.Add("0", item.Gender);//جنسیت
						messageParameters.Add("1", item.FullName);//نام و نام خانوادگی
						messageParameters.Add("2", item.Year);//سال تولد
						messageParameters.Add("3", item.MonthName);//اسم ماه تولد
						messageParameters.Add("4", item.Month);//شماره ماه تولد
						messageParameters.Add("5", item.DayName);//روز تولد
						messageParameters.Add("6", item.Day);//شماره روز تولد
                

						var message = new CreateCustomerMessageModel
						{
							CustomerId = item.CustomerId,
							DocumentId = null,
							MessageSettingType = SettingType.MessageType_HappyBirthday,
							Parameters = messageParameters,
							Title = Captions.MessageTitle_HappyBirthday,
                            MessageType=CustomerMessageType.HappyBirthday,
                        };
					await _customerService.SendMessageToCustomerAsync(message,true);
                    messageParameters = new Dictionary<string, string>();
                }
			}
		}
	}
}
