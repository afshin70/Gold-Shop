using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Gold.SharedKernel.Attributes.Validation
{
    public class PhoneOrMobileNumber : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null)
                return true;
            string inputValue = value.ToString();
            string phonePatern = "^0[0-9]{2,}[0-9]{7,}$";
            string mobilePatern = "09(1[0-9]|3[1-9]|2[1-9])-?[0-9]{3}-?[0-9]{4}";
            if (new Regex(mobilePatern).Match(inputValue).Success | new Regex(phonePatern).Match(inputValue).Success)
            {
                return true;
            }

            return false;

        }
    }
}
