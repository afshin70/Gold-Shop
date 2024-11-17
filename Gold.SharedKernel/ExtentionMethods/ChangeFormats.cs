using System;
using System.Globalization;

namespace Gold.SharedKernel.ExtentionMethods
{
    public static class ChangeFormats
    {
        private static string[] yakan = new string[10] { "صفر", "یک", "دو", "سه", "چهار", "پنج", "شش", "هفت", "هشت", "نه" };
        private static string[] dahgan = new string[10] { string.Empty, string.Empty, "بیست", "سی", "چهل", "پنجاه", "شصت", "هفتاد", "هشتاد", "نود" };
        private static string[] dahyek = new string[10] { "ده", "یازده", "دوازده", "سیزده", "چهارده", "پانزده", "شانزده", "هفده", "هجده", "نوزده" };
        private static string[] sadgan = new string[10] { string.Empty, "یکصد", "دویست", "سیصد", "چهارصد", "پانصد", "ششصد", "هفتصد", "هشتصد", "نهصد" };
        private static string[] basex = new string[5] { string.Empty, "هزار", "میلیون", "میلیارد", "تریلیون" };
        private static string getnum3(int num3)
        {
            string s = string.Empty;
            int d3, d12;
            d12 = num3 % 100;
            d3 = num3 / 100;
            if (d3 != 0)
                s = sadgan[d3] + " و ";
            if (d12 >= 10 && d12 <= 19)
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
        /// تبدیل عدد به حروف فارسی
        /// </summary>
        /// <param name="input">ورودی عدد</param>
        /// <returns></returns>
        public static string ToPersianCharacterString(this int input)
        {
            string snum = input.ToString();
            string stotal = string.Empty;
            if (snum == string.Empty) return "صفر";
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
                        stotal = stotal + getnum3(b) + " " + basex[L - i] + " و ";
                }
                stotal = stotal.Substring(0, stotal.Length - 3);
            }
            return stotal;
        }
        
        /// <summary>
        /// تبدیل عدد به حروف فارسی
        /// </summary>
        /// <param name="input">ورودی عدد</param>
        /// <returns></returns>
        public static string ToPersianCharacterString(this string input)
        {
            string snum = input.ToString();
            string stotal = string.Empty;
            if (snum == string.Empty) return "صفر";
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
                        stotal = stotal + getnum3(b) + " " + basex[L - i] + " و ";
                }
                stotal = stotal.Substring(0, stotal.Length - 3);
            }
            return stotal;
        }
        /// <summary>
        ///تبدیل میلادی به شمسی 
        /// </summary>
        /// <param name="input">تاریخ میلادی</param>
        /// <returns>مثال : 1398/1/1</returns>
        public static string ToShamsiString(this DateTime input)
        {
            PersianCalendar _PersianCalendar = new PersianCalendar();
            return $"{_PersianCalendar.GetYear(input)}/{_PersianCalendar.GetMonth(input)}/{_PersianCalendar.GetDayOfMonth(input)}";
        }
        public static string ToShamsiFarsiString(this DateTime input)
        {
            PersianCalendar _PersianCalendar = new PersianCalendar();
            return $"{input.ToShamsiDayName()} {_PersianCalendar.GetDayOfMonth(input)} {input.ToShamsiMonthNameString()}  {_PersianCalendar.GetYear(input)}";
        }
        /// <summary>
        /// تبدیل به ساعت رشته
        /// </summary>
        /// <param name="input">Date Time</param>
        /// <returns>مثال : 8:0</returns>
        public static string ToTimeString(this DateTime input)
        {
            return $"{input.Minute} : {input.Hour}";
        }
        public static string ToPersianTimeString(this DateTime input)
        {
            if (input.Minute == 0)
            {
                return $"ساعت {input.Hour}";
            }
            else
            {
                return $"ساعت {input.Hour} و {input.Minute} دقیقه";
            }
        }
        /// <summary>
        /// شمسی به میلادی 
        /// </summary>
        /// <param name="input">تاریخ شمسی</param>
        /// <returns>7/16/2019</returns>
        public static DateTime ToMiladiDateTime(this DateTime input)
        {
            PersianCalendar p = new PersianCalendar();
            return p.ToDateTime(input.Year, input.Month, input.Day, input.Hour, input.Minute, 0, 0);
        }
        /// <summary>
        /// شمسی به میلادی 
        /// </summary>
        /// <param name="input">تاریخ میلادی</param>
        /// <returns>7/16/2019</returns>
        public static string ToMiladiString(this DateTime input)
        {
            return $"{input.Year}/{input.Month}/{input.Day}";
        }
        /// <summary>
        /// تاریخ میلادی به تاریخ و زمان به زبان فارسی
        /// </summary>
        /// <param name="input">تاریخ میلادی</param>
        /// <returns>مثال 1395-11-9 11:20:34</returns>
        public static string ToPersianDateTimeString(this DateTime input)
        {
            PersianCalendar p = new PersianCalendar();
            int Year = p.GetYear(input);
            int Month = p.GetMonth(input);
            int Day = p.GetDayOfMonth(input);
            int Hour = p.GetHour(input);
            int Minute = p.GetMinute(input);
            int Second = p.GetSecond(input);
            double Milliseconds = p.GetMilliseconds(input);
            return $"{Year}/{Month}/{Day} {Hour}:{Minute}:{Second}.{Milliseconds}";
        }

        /// <summary>
        /// تاریخ میلادی به تاریخ و زمان به زبان فارسی
        /// فقط تاریخ
        /// </summary>
        /// <param name="input">تاریخ میلادی</param>
        /// <returns>مثال 1395-11-9 11:20:34</returns>
        public static string ToPersianDateString(this DateTime input)
        {
            PersianCalendar p = new PersianCalendar();
            int Year = p.GetYear(input);
            int Month = p.GetMonth(input);
            int Day = p.GetDayOfMonth(input);
            return $"{Year}/{Month}/{Day}";
        }
        /// <summary>
        /// تاریخ میلادی به تاریخ و زمان به زبان فارسی
        /// فقط زمان
        /// </summary>
        /// <param name="input">تاریخ میلادی</param>
        /// <returns>مثال 1395-11-9 11:20:34</returns>
        public static string ToPersianShortTimeString(this DateTime input)
        {
            PersianCalendar p = new PersianCalendar();
            int Hour = p.GetHour(input);
            int Minute = p.GetMinute(input);
            int Second = p.GetSecond(input);
            return $"{Hour}:{Minute}";
        }
        /// <summary>
        /// تبدیل دسیمال به واحد نمایشی پول
        /// </summary>
        /// <param name="input">decimal</param>
        /// <returns>example : 25,000</returns>
        public static string ToPriceStringFormat(this decimal input)
        {
            int _Price = decimal.ToInt32(input);
            if (_Price > 0)
                return _Price.ToString("0,#");
            else
                return "0";
        }
        public static string ToPriceStringFormat(this int input)
        {
            if (input > 0)
                return input.ToString("0,#");
            else if (input < 0)
                return $"{Math.Abs(input).ToString("0,#")}-";
            else
                return "0";
        }
        /// <summary>
        /// دریافت  نام ماه شمسی با گرفتن تاریخ میلادی
        /// </summary>
        /// <param name="input"></param>
        /// <returns>مثال : اردیبهشت</returns>
        public static string ToShamsiMonthNameString(this DateTime input)
        {
            try
            {
                PersianCalendar p = new PersianCalendar();
                int Month = p.GetMonth(input);
                string MonthName = string.Empty;
                switch (Month)
                {
                    case 1:
                        MonthName = "فروردین";
                        break;

                    case 2:
                        MonthName = "اردیبهشت";
                        break;

                    case 3:
                        MonthName = "خرداد";
                        break;

                    case 4:
                        MonthName = "تیر";
                        break;

                    case 5:
                        MonthName = "مرداد";
                        break;

                    case 6:
                        MonthName = "شهریور";
                        break;

                    case 7:
                        MonthName = "مهر";
                        break;

                    case 8:
                        MonthName = "آبان";
                        break;

                    case 9:
                        MonthName = "آذر";
                        break;

                    case 10:
                        MonthName = "دی";
                        break;

                    case 11:
                        MonthName = "بهمن";
                        break;

                    case 12:
                        MonthName = "اسفند";
                        break;

                    default:
                        break;
                }

                return $"{MonthName}";
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// دریافت  نام ماه شمسی با گرفتن تاریخ میلادی
        /// </summary>
        /// <param name="input"></param>
        /// <returns>مثال : اردیبهشت</returns>
        public static string ToShamsiMonthNameShortString(this DateTime input)
        {
            try
            {
                PersianCalendar p = new PersianCalendar();
                int Year = p.GetYear(input);
                int Month = p.GetMonth(input);
                int Day = p.GetDayOfMonth(input);
                string MonthName = string.Empty;
                switch (Month)
                {
                    case 1:
                        MonthName = "فروردین";
                        break;

                    case 2:
                        MonthName = "اردیبهشت";
                        break;

                    case 3:
                        MonthName = "خرداد";
                        break;

                    case 4:
                        MonthName = "تیر";
                        break;

                    case 5:
                        MonthName = "مرداد";
                        break;

                    case 6:
                        MonthName = "شهریور";
                        break;

                    case 7:
                        MonthName = "مهر";
                        break;

                    case 8:
                        MonthName = "آبان";
                        break;

                    case 9:
                        MonthName = "آذر";
                        break;

                    case 10:
                        MonthName = "دی";
                        break;

                    case 11:
                        MonthName = "بهمن";
                        break;

                    case 12:
                        MonthName = "اسفند";
                        break;

                    default:
                        break;
                }

                return $"{Year}-{MonthName}-{Day}";
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// نام روز هفته در تاریخ شمسی 
        /// </summary>
        /// <param name="input">تاریخ میلادی</param>
        /// <returns>مثال : شنبه</returns>
        public static string ToShamsiDayName(this DateTime input)
        {
            PersianCalendar p = new PersianCalendar();
            int Year = p.GetYear(input);
            int Month = p.GetMonth(input);
            int Day = p.GetDayOfMonth(input);
            int Hour = p.GetHour(input);
            int Minute = p.GetMinute(input);
            int Second = p.GetSecond(input);
            double Milliseconds = p.GetMilliseconds(input);
            string DayName = string.Empty;
            switch (input.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    DayName = "یکشنبه";
                    break;
                case DayOfWeek.Monday:
                    DayName = "دوشنبه";
                    break;
                case DayOfWeek.Tuesday:
                    DayName = "سه شنبه";
                    break;
                case DayOfWeek.Wednesday:
                    DayName = "چهارشنبه";
                    break;
                case DayOfWeek.Thursday:
                    DayName = "پنج شنبه";
                    break;
                case DayOfWeek.Friday:
                    DayName = "جمعه";
                    break;
                case DayOfWeek.Saturday:
                    DayName = "شنبه";
                    break;
                default:
                    break;
            }
            return DayName;
        }
        public static DateTime? ToPersianStringToMiladi(this string input)
        {
            try
            {
                int Year = int.Parse(input.Split('/')[0]);
                int Month = int.Parse(input.Split('/')[1]);
                int Day = int.Parse(input.Split('/')[2]);
                PersianCalendar _PersianCalendar = new PersianCalendar();
                return _PersianCalendar.ToDateTime(Year, Month, Day, 0, 0, 0, 0);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static DateTime? ToPersianStringToMiladi(this string input, int Hour, int Minute)
        {
            try
            {
                int Year = int.Parse(input.Split('/')[0]);
                int Month = int.Parse(input.Split('/')[1]);
                int Day = int.Parse(input.Split('/')[2]);
                PersianCalendar _PersianCalendar = new PersianCalendar();
                return _PersianCalendar.ToDateTime(Year, Month, Day, Hour, Minute, 0, 0);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static bool CompareDate(this DateTime input, DateTime OtherDate)
        {
            try
            {
                if (input.Year == OtherDate.Year && input.Month == OtherDate.Month && input.Day == OtherDate.Day && input.Hour == OtherDate.Hour && input.Minute == OtherDate.Minute)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool CompareDateDay(this DateTime input, DateTime OtherDate)
        {
            try
            {
                if (input.Year == OtherDate.Year && input.Month == OtherDate.Month && input.Day == OtherDate.Day)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
