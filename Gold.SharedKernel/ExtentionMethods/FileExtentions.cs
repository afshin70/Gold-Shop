using Microsoft.AspNetCore.Http;


namespace Gold.SharedKernel.ExtentionMethods
{
    public static class FileExtentions
    {
        /// <summary>
        /// چک کردن فرمت عکس بصورت jpeg
        /// </summary>
        /// <param name="file">فایل عکس</param>
        /// <returns></returns>
        public static bool CheckJPEGContentType(this IFormFile file)
        {
            try
            {
                string content_type = file.ContentType.ToLower();
                string extention = Path.GetExtension(file.FileName).ToLower();
                if (content_type == "image/jpeg")
                {
                    if (extention == ".jpeg" || extention == ".jpg")
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// چک کردن نوع و محتوای فایل ارسال شده از سمت کابر
        /// </summary>
        /// <param name="file">فایل  آپلود شده از سمت کاربر</param>
        /// <param name="contenttype">نوع محتوا بر حسب نیاز</param>
        /// <param name="file_extention"> نوع اکستنشن و پسوند فایل برحسب نیاز و اختیاری</param>
        /// <returns></returns>
        public static bool CheckFileContentType(this IFormFile file, string contenttype, string[] file_extention)
        {
            string content_type = file.ContentType.ToLower();
            string extention = Path.GetExtension(file.FileName).ToLower();
            if (content_type == contenttype.ToLower())
            {
                if (file_extention.Any())
                {
                    if (file_extention.Any(fe => fe.ToLower() == extention))
                        return true;
                    else
                        return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// چک کردن محتوای فایل تصویر
        /// </summary>
        /// <param name="formFile">فایل</param>
        /// <returns></returns>
        public static bool IsImageValid(this IFormFile formFile)
        {
            try
            {
                //Image image = Image.FromStream(formFile.OpenReadStream());
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        /// <summary>
        /// دریافت تصادفی نام تصویر
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetNewImageName(this string input)
        {
            return Guid.NewGuid().ToString().Replace("-", "") + ".jpg";
        }
        /// <summary>
        /// دریاقت تصادفی نام تصویر مرتب شده بر اساس تاریخ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetNewImageNameByDateTime(this string input)
        {
            return DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + Guid.NewGuid().ToString().Replace("-", "") + ".jpg";
        }
        /// <summary>
        /// ای دی با guid
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetNewGuId(this string input)
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
