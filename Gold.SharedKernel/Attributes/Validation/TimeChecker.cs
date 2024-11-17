using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Gold.SharedKernel.ExtentionMethods;

namespace Gold.SharedKernel.Attributes.Validation
{
    public class OnlyPersianAndArabic : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            return base.IsValid(value);
        }
    }
    public class TimeChecker : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null)
                return true;
            if (value.GetType() != typeof(string))
                return false;

            //string[] time;
            try
            {
                var time = value.ToString().ToEnglishNumberByCultureInfo().Split(":");
                string h = time[0];
                string m = time[1];
                if (!byte.TryParse(h, out var hour))
                {
                    return false;
                }

                if (hour > 23 | hour < 0)
                {
                    return false;
                }

                if (!byte.TryParse(m, out var minute))
                {
                    return false;
                }

                if (minute > 59 | minute < 0)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
