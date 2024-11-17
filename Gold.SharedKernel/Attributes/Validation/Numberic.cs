using Gold.SharedKernel.ExtentionMethods;
using System.ComponentModel.DataAnnotations;

namespace Gold.SharedKernel.Attributes.Validation
{

    //public class NumbericRange : ValidationAttribute
    //{
    //    private Type _type;
    //    private string _min;
    //    private string _max;
    //    private string? _spliter;
    //    public NumbericRange(Type type, string min, string max, string? spliter)
    //    {
    //        _type = type;
    //        _min = min;
    //        _max = max;
    //        _spliter = spliter;
    //    }
    //    public override bool IsValid(object? value)
    //    {
    //        if (value == null)
    //        {
    //            return true;
    //        }

    //        string input = value.ToString();
    //        if (_spliter != null)
    //        {
    //            input = value.ToString().Replace(_spliter, "");
    //        }

    //        if (decimal.TryParse(input, out decimal i))
    //        {
    //            if (i is <=(_type)_max)
    //            {

    //            }
    //            return true;
    //        }
    //        if (decimal.TryParse(input, out decimal dd))
    //            return true;

    //        return false;
    //    }
    //}
    public class Numberic : ValidationAttribute
    {
        private string? _spliter;
        public Numberic(string? spliter)
        {
            _spliter = spliter;
        }
        public override bool IsValid(object? value)
        {
            if (value == null)
                return true;

            string input = value.ToString().ToEnglishNumbers();
            if (_spliter != null)
                input = value.ToString().Replace(_spliter, "");

            if (long.TryParse(input, out long i))
                return true;
            if (decimal.TryParse(input, out decimal dd))
                return true;
            return false;
        }
    }
}
