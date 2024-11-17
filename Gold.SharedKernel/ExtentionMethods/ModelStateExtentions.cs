using Gold.EndPoints.Presentation.Models;
using Gold.Resources;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Gold.SharedKernel.ExtentionMethods
{
    public static class ModelStateExtentions
    {
        public static string GetFirstErrorMessage(this ModelStateDictionary modelState)
        {
            if (modelState.Values.Any())
            {
                foreach (var item in modelState.Values)
                {
                    if (item.Errors.Any())
                    {
                        foreach (var err in item.Errors)
                        {
                            return err.ErrorMessage;
                        }
                    }

                }

            }
            
            return string.Empty;
        }

        public static List<string> GetAllErrorMessages(this ModelStateDictionary modelState)
        {
            List<string> errorMessages = new List<string>();
            if (modelState.Values.Any())
            {
                foreach (var item in modelState.Values)
                {
                    if (item.Errors.Any())
                    {
                        foreach (var err in item.Errors)
                        {
                            errorMessages.Add( err.ErrorMessage);
                        }
                    }
                }
            }
            return errorMessages;
        }

        public static ToastrResult GetAllErrorMessagesAsToast(this ModelStateDictionary modelState, ToastrType toastrType,bool isSuccess, string title)
        {
            ToastrResult toastrResult = new ToastrResult
            {
                Title = title,
                IsSuccess= isSuccess,
                Type= toastrType,
            };
            
            string errorMessages = string.Empty;
            if (modelState.Values.Any())
            {
                foreach (var item in modelState.Values)
                {
                    if (item.Errors.Any())
                    {
                        foreach (var err in item.Errors)
                        {
                            errorMessages+=$"<br>{err.ErrorMessage}";
                        }
                    }
                }
            }
            toastrResult.Message = errorMessages;

            return toastrResult;
        } 
        
        public static ToastrResult<TModel> GetAllErrorMessagesAsToast<TModel>(this ModelStateDictionary modelState, ToastrType toastrType,bool isSuccess, string title, TModel model)
        {
            ToastrResult<TModel> toastrResult = new ToastrResult<TModel>
            {
                Title = title,
                IsSuccess= isSuccess,
                Type= toastrType,
                Data= model
            };
            
            string errorMessages = string.Empty;
            if (modelState.Values.Any())
            {
                foreach (var item in modelState.Values)
                {
                    if (item.Errors.Any())
                    {
                        foreach (var err in item.Errors)
                        {
                            errorMessages+=$"<br>{err.ErrorMessage}";
                        }
                    }
                }
            }
            toastrResult.Message = errorMessages;

            return toastrResult;
        }
    }
}
