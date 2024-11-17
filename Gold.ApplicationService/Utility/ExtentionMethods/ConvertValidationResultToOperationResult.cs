using FluentValidation.Results;
using Gold.SharedKernel.DTO.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Utility.ExtentionMethods
{
    public static class ConvertValidationResultToOperationResult
    {
        public static CommandResult ToOperationResult(this ValidationResult result)
        {
            return new CommandResult
            {
                IsSuccess=result.IsValid,
                //ValidationErrors=result.ToDictionary(),
            };
        }
    }
}
