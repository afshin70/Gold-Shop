using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Gold.SharedKernel.Attributes.Validation
{
	public class MonyFormat : ValidationAttribute
	{
		private readonly string _splitCahr;
		private readonly bool _isFloat;
		public MonyFormat(string splitCahr,bool isFloat=false)
		{
			_splitCahr = splitCahr;
			_isFloat = isFloat;
		}
		public override bool IsValid(object? value)
		{
			if (value == null)
				return true;
			else
			{
				string _value = value.ToString().Replace(_splitCahr, "");
				if (_isFloat)
				{
                    if (!double.TryParse(_value, out double amount))
                        return false;
                    else
                    if (amount < 0)
                        return false;
                }
                else
				{
                    if (!long.TryParse(_value, out long amount))
                        return false;
                    else
                    if (amount < 0)
                        return false;
                }
				return true;
			}
		}


	}
}
