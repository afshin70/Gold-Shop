using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Gold.SharedKernel.ExtentionMethods
{
    public static class StringsExtensions
    {
        public static string GeneratePassword(this string phrase)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuwxyz~!@#$%^&*";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, 6)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());
            return phrase + result;
        }

        public static bool IsEmptyOrNull(this string input)
        {
            return string.IsNullOrEmpty(input);
        }
        public static string GenerateSlug(this string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase))
            {
                return "";
            }
            phrase = phrase.ToLowerInvariant().Replace(" ", "-");
            phrase = RemoveDiacritics(phrase);
            phrase = RemoveReservedUrlCharacters(phrase);

            return phrase.ToLowerInvariant();
        }

        private static string RemoveReservedUrlCharacters(string text)
        {
            var reservedCharacters = new List<string> { "!", "#", "$", "&", "'", "(", ")", "*", ",", "/", ":", ";", "=", "?", "@", "[", "]", "\"", "%", ".", "<", ">", "\\", "^", "_", "'", "{", "}", "|", "~", "`", "+" };

            foreach (var chr in reservedCharacters)
            {
                text = text.Replace(chr, "");
            }

            return text;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Converts Persian and Arabic digits of a given string to their equivalent English digits.
        /// </summary>
        /// <param name="data">Persian number</param>
        /// <returns></returns>
        public static string ToEnglishNumbers(this string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return string.Empty;
            return data.ToEnglishNumberByCultureInfo();
        }
        public static string ToEnglishNumberByCultureInfo(this string input)
        {
            var culture = new System.Globalization.CultureInfo("en-US");
            var output = "";
            foreach (var c in input)
            {
                if (char.IsDigit(c))
                {
                    output += char.GetNumericValue(c).ToString(culture);
                }
                else
                {
                    output += c;
                }
            }
            return output;
        }
        /// <summary>
        /// Converts Persian and Arabic digits of a given string to their equivalent English digits.
        /// </summary>
        /// <param name="data">Persian number</param>
        /// <returns></returns>
        public static string ToEnglishNumbersByUniCode(this string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return string.Empty;
            return
                data.Replace("\u0660", "0") //٠
                    .Replace("\u06F0", "0") //۰
                    .Replace("\u0661", "1") //١
                    .Replace("\u06F1", "1") //۱
                    .Replace("\u0662", "2") //٢
                    .Replace("\u06F2", "2") //۲
                    .Replace("\u0663", "3") //٣
                    .Replace("\u06F3", "3") //۳
                    .Replace("\u0664", "4") //٤
                    .Replace("\u06F4", "4") //۴
                    .Replace("\u0665", "5") //٥
                    .Replace("\u06F5", "5") //۵
                    .Replace("\u0666", "6") //٦
                    .Replace("\u06F6", "6") //۶
                    .Replace("\u0667", "7") //٧
                    .Replace("\u06F7", "7") //۷
                    .Replace("\u0668", "8") //٨
                    .Replace("\u06F8", "8") //۸
                    .Replace("\u0669", "9") //٩
                    .Replace("\u06F9", "9") //۹
                ;
        }
        public static string ToEnNumber(this string persianStr)
        {
            Dictionary<char, char> LettersDictionary = new Dictionary<char, char>
            {
                ['۰'] = '0',
                ['۱'] = '1',
                ['۲'] = '2',
                ['۳'] = '3',
                ['۴'] = '4',
                ['۵'] = '5',
                ['۶'] = '6',
                ['۷'] = '7',
                ['۸'] = '8',
                ['۹'] = '9'
            };
            foreach (var item in persianStr)
            {
                persianStr = persianStr.Replace(item, LettersDictionary[item]);
            }
            return persianStr;
        }
        public static string ToFaNumber(this string englishStr)
        {
            Dictionary<char, char> LettersDictionary = new Dictionary<char, char>
            {
                ['0'] = '۰',
                ['1'] = '۱',
                ['2'] = '۲',
                ['3'] = '۳',
                ['4'] = '۴',
                ['5'] = '۵',
                ['6'] = '۶',
                ['7'] = '۷',
                ['8'] = '۸',
                ['9'] = '۹'
            };
            foreach (var item in englishStr)
            {
                englishStr = englishStr.Replace(item, LettersDictionary[item]);
            }
            return englishStr;
        }
       
        private static string[] yakan = new string[10] { "صفر", "یک", "دو", "سه", "چهار", "پنج", "شش", "هفت", "هشت", "نه" };
        private static string[] dahgan = new string[10] { "", "", "بیست", "سی", "چهل", "پنجاه", "شصت", "هفتاد", "هشتاد", "نود" };
        private static string[] dahyek = new string[10] { "ده", "یازده", "دوازده", "سیزده", "چهارده", "پانزده", "شانزده", "هفده", "هجده", "نوزده" };
        private static string[] sadgan = new string[10] { "", "یکصد", "دویست", "سیصد", "چهارصد", "پانصد", "ششصد", "هفتصد", "هشتصد", "نهصد" };
        private static string[] basex = new string[5] { "", "هزار", "میلیون", "میلیارد", "تریلیون" };
        private static string Getnum3(int num3)
        {
            string s = "";
            int d3, d12;
            d12 = num3 % 100;
            d3 = num3 / 100;
            if (d3 != 0)
                s = sadgan[d3] + " و ";
            if ((d12 >= 10) && (d12 <= 19))
            {
                s = s + dahyek[d12 - 10];
            }
            else
            {
                int d2 = d12 / 10;
                if (d2 != 0)
                    s = s + dahgan[d2] + " و ";
                int d1 = d12 % 10;
                if (d1 != 0)
                    s = s + yakan[d1] + " و ";
                s = s.Substring(0, s.Length - 3);
            };
            return s;
        }
        /// <summary>
        /// یک - دو - سه - چهار و ...
        /// </summary>
        /// <param name="snum"></param>
        /// <returns></returns>
        public static string ToPersianAlphabetNumber(this string snum)
        {
            string stotal = "";
            if (snum == "") return "صفر";
            if (snum == "0")
            {
                return yakan[0];
            }
            else
            {
                snum = snum.PadLeft(((snum.Length - 1) / 3 + 1) * 3, '0');
                int L = snum.Length / 3 - 1;
                for (int i = 0; i <= L; i++)
                {
                    int b = int.Parse(snum.Substring(i * 3, 3));
                    if (b != 0)
                        stotal = stotal + Getnum3(b) + " " + basex[L - i] + " و ";
                }
                stotal = stotal.Substring(0, stotal.Length - 3);
            }
            return stotal.Substring(0,stotal.Length-1);
        }

        /// <summary>
        /// یکم - دوم - سوم - چهارم ...
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToPersianAlphabetNumber2(this string input)
        {
            string str = ToPersianAlphabetNumber(input) + "م";
            return str
                .Replace("سهم", "سوم")
                .Replace("سیم","سی ام");
        }
        /// <summary>
        /// اول - دوم - سوم - چهارم ...
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToPersianAlphabetNumber3(this string input)
        {
            string str = ToPersianAlphabetNumber(input) + "م";
            return str
                .Replace("یکم", "اول")
                .Replace("سهم", "سوم")
                .Replace("سیم", "سی ام");
        }
        public static string GetFaNumberRandomString(int length)
        {
            var random = new Random();
            const string chars = "۰١۲۳۴۵۶۷۸۹";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        
        public static string GetEnNumberRandomString(int length)
        {
            var random = new Random();
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

		public static string FormatMoney(this object input)
		{
			try
			{
				long value = Convert.ToInt64(input);
				if (value != 0)
					return value.ToString("N0");
				return "0";
			}
			catch {return string.Empty; }
		}

        public static string RemoveHtmlTags(this string? input,string relpaceWith="")
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            return Regex.Replace(input, "<.*?>", relpaceWith);
        }

        public static string EnglishToPersian(this string persianStr)
        {
            persianStr= persianStr.ToEnglishNumberByCultureInfo();
            Dictionary<string, string> LettersDictionary = new Dictionary<string, string>
            {
                ["0"] = "صفر",
                ["1"] = "یک",
                ["2"] = "دو",
                ["3"] = "سه",
                ["4"] = "چهار",
                ["5"] = "۵",
                ["6"] = "۶",
                ["7"] = "۷",
                ["8"] = "۸",
                ["9"] = "۹"
            };
            return LettersDictionary.Aggregate(persianStr, (current, item) =>
                         current.Replace(item.Key, item.Value));
        }
    }
}
