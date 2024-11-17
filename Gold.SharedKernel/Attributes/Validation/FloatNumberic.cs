using Gold.SharedKernel.ExtentionMethods;
using System.ComponentModel.DataAnnotations;

namespace Gold.SharedKernel.Attributes.Validation
{
    public class FloatNumberic : ValidationAttribute
    {
        private string? _spliter;
        /// <summary>
        /// تعداد رقم اعشار
        /// </summary>
        private byte _decimalPlacesNumber;
        public FloatNumberic(string? spliter, byte decimalPlacesNumber = 0)
        {
            _spliter = spliter;
            _decimalPlacesNumber = decimalPlacesNumber;
        }
        public override bool IsValid(object? value)
        {
            if (value is null)
                return true;

            string input = value.ToString().ToEnglishNumbers();

            if (_spliter != null)
            {
                input = value.ToString().Replace(_spliter, "");
                if (_decimalPlacesNumber > 0)
                {
                    var numberArray = value.ToString().Split(_spliter);
                    if (numberArray != null)
                        if (numberArray.Length > 1)
                            if (numberArray[1].Length > _decimalPlacesNumber)
                                return false;
                }
            }

            if (long.TryParse(input, out long i))
                return true;
            if (decimal.TryParse(input, out decimal dd))
                return true;
            return false;
        }
    }
}
