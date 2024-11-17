using Gold.SharedKernel.ExtentionMethods;
using System.ComponentModel.DataAnnotations;

namespace Gold.SharedKernel.Attributes.Validation
{
    public class PersianDate : ValidationAttribute
    {
        private readonly string _splitCahr;
        public PersianDate(string splitCahr)
        {
            _splitCahr = splitCahr;
        }
        public override bool IsValid(object? value)
        {
            try
            {
                if (value == null)
                    return true;
                string _value = (string)value;
                var date = _value.ToEnglishNumbers().Split(_splitCahr);
                string year = date[0];
                string month = date[1];
                string day = date[2];

                if (int.TryParse(year, out var _year))
                {
                    if (_year is < 1000 or > 9999)
                        return false;
                }
                else
                    return false;

                if (int.TryParse(month, out var _month))
                {
                    if (_month is < 1 or > 12)
                        return false;
                }
                else
                    return false;

                if (int.TryParse(day, out var _day))
                {
                    if (_day is < 1 or > 31)
                        return false;
                }
                else
                    return false;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


    }
}
