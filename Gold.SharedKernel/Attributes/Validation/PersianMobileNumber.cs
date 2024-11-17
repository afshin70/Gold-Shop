using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gold.SharedKernel.Attributes.Validation
{
    public class PersianMobileNumber : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null)
                return true;
            string inputValue = value.ToString();
            //string mobilePatern = "09(1[0-9]|3[1-9]|2[1-9])-?[0-9]{3}-?[0-9]{4}";
            string mobilePatern = "^(\\+98|0098|98|0)?9\\d{9}$";
            if (new Regex(mobilePatern).Match(inputValue).Success)
            {
                return true;
            }
            else
            {
                return false;
            }
            return true;
        }
    }
}
