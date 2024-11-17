using Gold.SharedKernel.ExtentionMethods;
using System.ComponentModel.DataAnnotations;

namespace Gold.SharedKernel.Attributes.Validation
{
	public class BankCartNumber : ValidationAttribute
	{
		private char _spliter;
		public BankCartNumber(char spliter = '-')
		{
			_spliter = spliter;
		}
		public override bool IsValid(object value)
		{
			if (value == null)
				return true;
			
			string input = value.ToString().ToEnglishNumbers().Trim();
			if (string.IsNullOrEmpty(input))
				return true;
			var splitNumbers = input.Split(_spliter);
			if (splitNumbers.Length !=4)
				return false;
			for (int i = 0; i < splitNumbers.Length; i++)
			{
				if (splitNumbers[i].Length != 4)
					return false;
				if (!long.TryParse(splitNumbers[i], out long outNumber))
					return false;
			}
			return true;
		}
	}
}
