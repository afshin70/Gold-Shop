using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Gold.SharedKernel.Attributes.Validation
{
    public class PhoneNumber : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null)
                return true;
            string inputValue = value.ToString();
            string phonePatern = "^0[0-9]{2,}[0-9]{7,}$";
            if (new Regex(phonePatern).Match(inputValue).Success)
            {
                return true;
            }
            return false;
        }
    }
}
