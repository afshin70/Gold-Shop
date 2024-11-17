using Gold.Resources;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.ExtentionMethods;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.SharedKernel.DTO.OperationResult
{
    public class CommandResult
    {
        public string Message { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }

        /// <summary>
        /// عملیات موفق
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static CommandResult Success(string message)
        {
            return new CommandResult()
            {
                Message = message,
                IsSuccess=true
            };
        }
        /// <summary>
        /// خطا
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static CommandResult Failure(string message)
        {
            return new CommandResult()
            {
                Message = message,
                IsSuccess=false
            };
        }
    }

    public class CommandResult<TData> : CommandResult
    {
        public TData Data { get; set; }

        /// <summary>
        /// عملیات موفق
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static CommandResult<TData> Success(string message, TData data)
        {
            return new CommandResult<TData>()
            {
                Message = message,
                IsSuccess = true,
                Data = data
            };
        }
        /// <summary>
        /// خطا
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static CommandResult<TData> Failure(string message, TData data)
        {
            return new CommandResult<TData>()
            {
                Message = message,
                IsSuccess=false,
                Data = data
            };
        }
        /// <summary>
        /// خطا در هنگام حذف داده
        /// </summary>
        /// <returns></returns>
        public static CommandResult<TData> FailureInRemoveData(TData data)
        {
            return new CommandResult<TData>()
            {
                Message = DBOperationMessages.AnErrorOccurredWhileRemovingData,
                IsSuccess=false,
                Data = data
            };
        }

        /// <summary>
        /// خطا در هنگام  بروزرسانی داده ها
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static CommandResult<TData> FailureInUpdateData( TData data)
        {
            return new CommandResult<TData>()
            {
                Message = DBOperationMessages.AnErrorOccurredWhileEditingData,
                IsSuccess=false,
                Data = data
            };
        }
        /// <summary>
        /// خطا در هنگام واکشی داده ها
        /// </summary>
        /// <returns></returns>
        public static CommandResult<TData> FailureInRetrivingData(TData data)
        {
            return new CommandResult<TData>()
            {
                Message = DBOperationMessages.AnErrorOccurredWhileRetrievingData,
                IsSuccess=false,
                Data = data
            };
        }

        /// <summary>
        /// خطا در هنگام افزودن داده ها
        /// </summary>
        /// <returns></returns>
        public static CommandResult<TData> FailureInAddData( TData data)
        {
            return new CommandResult<TData>()
            {
                Message = DBOperationMessages.AnErrorOccurredWhileAddingData,
                IsSuccess=false,
                Data = data
            };
        }
        /// <summary>
        /// خطا در هنگام حذف داده
        /// </summary>
        /// <returns></returns>
        public static CommandResult<TData> FailureInRemoveData()
        {
            return new CommandResult<TData>()
            {
                Message = DBOperationMessages.AnErrorOccurredWhileRemovingData,
                IsSuccess=false,
                Data =  default(TData)
            };
        } 

        /// <summary>
        /// خطا در هنگام ذخیره سازی داده ها
        /// </summary>
        /// <returns></returns>
        public static CommandResult<TData> FailureInSaveData()
        {
            return new CommandResult<TData>()
            {
                Message = DBOperationMessages.AnErrorOccurredWhileSavingData,
                IsSuccess=false,
                Data =  default(TData)
            };
        }
        /// <summary>
        /// خطا در هنگام عملیات
        /// </summary>
        /// <returns></returns>
        public static CommandResult<TData> FailureInOperation()
        {
            return new CommandResult<TData>()
            {
                Message = DBOperationMessages.AnErrorOccurredInOperation,
                IsSuccess = false,
                Data = default(TData)
            };
        }
        
        /// <summary>
        /// خطا در هنگام حذف داده به علت استفاده در جای دیگر
        /// </summary>
        /// <returns></returns>
        public static CommandResult<TData> FailureInDeleteDataUseElsewhere()
        {
            return new CommandResult<TData>()
            {
                Message = DBOperationMessages.NotAllowedToDeleteDueToUseElsewhere,
                IsSuccess = false,
                Data = default(TData)
            };
        }
        
        /// <summary>
        /// خطا در هنگام  بروزرسانی داده ها
        /// </summary>
        /// <returns></returns>
        public static CommandResult<TData> FailureInUpdateData()
        {
            return new CommandResult<TData>()
            {
                Message = DBOperationMessages.AnErrorOccurredWhileEditingData,
                IsSuccess=false,
                Data =  default(TData)
            };
        }
        
        /// <summary>
        /// خطا در هنگام واکشی داده ها
        /// </summary>
        /// <returns></returns>
        public static CommandResult<TData> FailureInRetrivingData()
        {
            return new CommandResult<TData>()
            {
                Message = DBOperationMessages.AnErrorOccurredWhileRetrievingData,
                IsSuccess=false,
                Data =  default(TData)
            };
        }

        /// <summary>
        /// خطا در هنگام افزودن داده ها
        /// </summary>
        /// <returns></returns>
        public static CommandResult<TData> FailureInAddData()
        {
            return new CommandResult<TData>()
            {
                Message = DBOperationMessages.AnErrorOccurredWhileAddingData,
                IsSuccess=false,
                Data = default(TData)
            };
        }
    }
}
