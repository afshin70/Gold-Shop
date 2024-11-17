using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.SharedKernel.Tools
{
    public static class NumberTools
    {
       public static long RoundNumber(string number, int countLastNumberRond)
       {
            bool checkNegativeNumber = false;
            if (number.Contains("-"))
            {
                checkNegativeNumber = true;
                number = number.Replace("-", "").Trim();
            }
            if (number.Length < countLastNumberRond) return Convert.ToInt64(number);
            string twoPart = number.Substring(number.Length - countLastNumberRond, countLastNumberRond);
            var onePart = number.Substring(number[0] == '-' ? 1 : 0, number.Length - countLastNumberRond);
            var round = Math.Round(Convert.ToDecimal(onePart + "." + twoPart)).ToString();
            for (var i = 0; i < countLastNumberRond; i++)
            {
                round += "0";
            }
            if (checkNegativeNumber)
                return Convert.ToInt64(round) * -1;
            //if (Convert.ToInt64(number) < 0)
            return Convert.ToInt64(round);

        }

        public static long RoundUpNumber(string number, int countLastNumberRond)
        {
            bool checkNegativeNumber = false;
            if (number.Contains("-"))
            {
                checkNegativeNumber = true;
                number = number.Replace("-", "").Trim();
            }
            if (number.Length < countLastNumberRond) return Convert.ToInt64(number);
            string twoPart = number.Substring(number.Length - countLastNumberRond, countLastNumberRond);
            var onePart = number.Substring(number[0] == '-' ? 1 : 0, number.Length - countLastNumberRond);
            var round = Math.Ceiling(Convert.ToDecimal(onePart + "." + twoPart)).ToString();
            for (var i = 0; i < countLastNumberRond; i++)
            {
                round += "0";
            }
            if (checkNegativeNumber)
                return Convert.ToInt64(round) * -1;
            //if (Convert.ToInt64(number) < 0)
            return Convert.ToInt64(round);

        }

        
    }
}
