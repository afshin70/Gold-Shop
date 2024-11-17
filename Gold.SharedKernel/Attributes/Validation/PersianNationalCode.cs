using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Gold.SharedKernel.Attributes.Validation
{
    public class PersianNationalCode : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
			try
			{
				if (value == null) return true;
				string MeliCode = value.ToString();
				ArrayList array = new ArrayList();
				//int sum;
				int ret = 0;
				//0=not valid Format
				//1= Valid
				//2= Not Exist
				if (MeliCode.Length != 10 || MeliCode == "0000000000" || MeliCode == "1111111111" ||
					MeliCode == "2222222222" || MeliCode == "3333333333" || MeliCode == "4444444444" ||
					MeliCode == "5555555555" || MeliCode == "6666666666" || MeliCode == "7777777777" ||
					MeliCode == "8888888888" || MeliCode == "9999999999" || MeliCode == "1234567891")
					ret = 0;
				else
				{
					//convert to int
					long code = 0;
					try
					{
						code = Convert.ToInt64(MeliCode);
					}
					catch
					{
						ret = 0;
					}
					if (code != 0)
					{
						int sumCode = 0;

						for (int i = 0; i < 9; i++)
						{
							string index = MeliCode[i].ToString();
							sumCode += int.Parse(index) * (10 - i);
						}

						int r = sumCode % 11;
						if (r < 2)
						{
							if (int.Parse(MeliCode[9].ToString()) != r)
							{

								ret = 2;
							}
							else
							{
								ret = 1;
							}
						}
						else if (r >= 2)
						{
							if (int.Parse(MeliCode[9].ToString()) != 11 - r)
							{
								ret = 2;
							}
							else
							{
								ret = 1;
							}
						}
					}
				}

				if (ret == 1) return true;
				return false;
			}
			catch (Exception)
			{
				return false;
			}
        }
    }
}
