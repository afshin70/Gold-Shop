using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.CustomerMessageModels;
using Gold.ApplicationService.Contract.Interfaces;
using Gold.Domain.Enums;
using Gold.Resources;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.ExtentionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Utility.Jobs
{
    public class RaminderMessageJobService
	{
		private readonly ICustomerService _customerService;
		public RaminderMessageJobService(ICustomerService customerService)
		{
			_customerService = customerService;
        }

        public async Task ReminderOneDayBeforeInstallmentDateAsync()
		{
           
            var ReminderOf1DayBeforeInstallmentDateResult = await _customerService.GetAllInstallmentsForReminderAsync(DateTime.Now.Date.AddDays(1));
		
			if (ReminderOf1DayBeforeInstallmentDateResult.IsSuccess)
			{
				Dictionary<string, string> messageParameters = new Dictionary<string, string>();
				List<CreateCustomerMessageModel> createCustomerMessages= new List<CreateCustomerMessageModel>();
					foreach (var item in ReminderOf1DayBeforeInstallmentDateResult.Data)
					{
						messageParameters.Add("0",(item.Gender.HasValue?(item.Gender.Value == GenderType.Men?Captions.Mr:Captions.Lady) :string.Empty));//جنسیت
						messageParameters.Add("1", item.FullName);//نام و نام خانوادگی
						messageParameters.Add("2", item.DocumentNumber.ToString());//شماره سند
						messageParameters.Add("3", item.DocumentDate.GeorgianToPersian(ShowMode.OnlyDate));//تاریخ سند
						messageParameters.Add("4", item.InstallmentCount.ToString());//تعداد کل اقساط به عدد
                    messageParameters.Add("5", item.InstallmentCount.ToString().ToPersianAlphabetNumber());//تعداد کل اقساط به حروف
                    messageParameters.Add("6", item.InstallmentAmount.ToString("N0"));//مبلغ قسط
						messageParameters.Add("7", item.InstallmentDay.ToString());//روز قسط به عدد
                    messageParameters.Add("8", item.InstallmentDay.ToString().ToPersianAlphabetNumber2());//روز قسط به حروف
                    messageParameters.Add("9", item.InstallmentNumber.ToString());//شماره قسط به عدد
                    messageParameters.Add("10", item.InstallmentNumber.ToString().ToPersianAlphabetNumber3());//شماره قسط به حروف
                    messageParameters.Add("11", item.TotalDelayDays.ToString());//تعداد روز دیرکرد کل
						var message = new CreateCustomerMessageModel
						{
							CustomerId = item.CustomerId,
							DocumentId = item.DocumentId,
							installmentId = item.InstallmentId,
							MessageSettingType = SettingType.MessageType_ReminderOfTheDayBeforeTheInstallmentDate,
							Parameters = messageParameters,
							Title = Captions.MessageTitle_PaymentReminder,
							MessageType=CustomerMessageType.PaymentReminder,
                        };
					await _customerService.SendMessageToCustomerAsync(message,true);
				}
			}

            var ReminderOf7DayAfterInstallmentDateResult = await _customerService.GetAllInstallmentsForReminderAsync(DateTime.Now.Date.AddDays(-7));
			if (ReminderOf7DayAfterInstallmentDateResult.IsSuccess)
			{
				Dictionary<string, string> messageParameters = new Dictionary<string, string>();
				List<CreateCustomerMessageModel> createCustomerMessages = new List<CreateCustomerMessageModel>();
				foreach (var item in ReminderOf7DayAfterInstallmentDateResult.Data)
				{
                    messageParameters.Add("0", (item.Gender.HasValue ? (item.Gender.Value == GenderType.Men ? Captions.Mr : Captions.Lady) : string.Empty));//جنسیت
                    messageParameters.Add("1", item.FullName);//نام و نام خانوادگی
                    messageParameters.Add("2", item.DocumentNumber.ToString());//شماره سند
                    messageParameters.Add("3", item.DocumentDate.GeorgianToPersian(ShowMode.OnlyDate));//تاریخ سند
                    messageParameters.Add("4", item.InstallmentCount.ToString());//تعداد کل اقساط به عدد
                    messageParameters.Add("5", item.InstallmentCount.ToString().ToPersianAlphabetNumber());//تعداد کل اقساط به حروف
                    messageParameters.Add("6", item.InstallmentAmount.ToString("N0"));//مبلغ قسط
                    messageParameters.Add("7", item.InstallmentDay.ToString());//روز قسط به عدد
                    messageParameters.Add("8", item.InstallmentDay.ToString().ToPersianAlphabetNumber2());//روز قسط به حروف
                    messageParameters.Add("9", item.InstallmentNumber.ToString());//شماره قسط به عدد
                    messageParameters.Add("10", item.InstallmentNumber.ToString().ToPersianAlphabetNumber3());//شماره قسط به حروف
                    messageParameters.Add("11", item.TotalDelayDays.ToString());//تعداد روز دیرکرد کل
                    var message = new CreateCustomerMessageModel
					{
						CustomerId = item.CustomerId,
						DocumentId = item.DocumentId,
						installmentId = item.InstallmentId,
						MessageSettingType = SettingType.MessageType_Reminder7DaysAfterTheUnpaidInstallmentDate,
						Parameters = messageParameters,
						Title = Captions.MessageTitle_LatePaymentReminder,
						MessageType=CustomerMessageType.Reminder7Days,
                    };
					await _customerService.SendMessageToCustomerAsync(message, true);
				}
			}
		}
	}
}
