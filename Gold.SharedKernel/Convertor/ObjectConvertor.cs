using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Gold.SharedKernel.Convertor
{
    public static class ObjectConvertor
    {
        /// <summary>
        ///تبدیل یک آبجکت به جیسان
        /// </summary>
        /// <param name="obj">آبجکت شما</param>
        /// <returns>مقدار خروجی: رشته جی سان آبجکت ورودی</returns>
        public static string? ConvertObjectToJson(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// تبدیل یک جیسان به آبجکت متناظر
        /// </summary>
        /// <typeparam name="T">نوع آبجکت را مشخص کنید</typeparam>
        /// <param name="json">رشته جیسان</param>
        /// <returns>خروجی: تبدیل به نوع متناظر جیسان ورودی</returns>
        public static T? ConvertJsonToOject<T>(this string json)
        {
            if (string.IsNullOrEmpty(json))
                return default(T);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
