using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.SharedKernel.Tools
{
    public static class GeneratorTools
    {
        public static string GenerateOTP(int lenght = 6)
        {
            string[] allowedCharacters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            string sOTP = string.Empty;
            string sTempChars = string.Empty;
            Random rand = new Random();
            for (int i = 0; i < lenght; i++)
            {
                int p = rand.Next(0, allowedCharacters.Length);
                sTempChars = allowedCharacters[rand.Next(0, allowedCharacters.Length)];
                sOTP += sTempChars;
            }
            return sOTP;
        }
    }
}
