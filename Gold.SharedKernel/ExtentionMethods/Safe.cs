namespace Gold.SharedKernel.ExtentionMethods
{
    public static class Safe
    {
        public static string GetEnglishNumber(this string persianNumber)
        {
            string englishNumber = "";
            foreach (char ch in persianNumber)
            {
                englishNumber += char.GetNumericValue(ch);
            }
            return englishNumber;
        }
       
        public static string ToSafeString(this object input)
        {
            if (input is null)
                return string.Empty;
            else
                return input.ToString();
        }
       
		public static int ToSafeInt(this int? input)
        {
            if (!input.HasValue)
                return 0;
            else
                return input.Value;
        }
        /// <summary>
        /// چک کردن ایمیل
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>
        ///   <c>true</c> if [is valid email] [the specified email]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidEmail(this string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
