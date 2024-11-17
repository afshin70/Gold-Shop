using Gold.SharedKernel.DTO.Captcha;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.SharedKernel.Contract
{
    public interface ICaptchaResolver
    {
        /// <summary>
        /// generate captcha as FileContentResult
        /// </summary>
        /// <param name="pageName">name of page for use captcha - must be is uniq page</param>
        /// <param name="haveNoisy"></param>
        /// <returns>
        /// save  CaptchaSessionValue in session
        /// pass CaptchaSessionValue to IsValid method for validate
        /// </returns>
        CaptchaAsFileContentResultModel GenerateCaptchaAsFileContentResult(string pageName, bool haveNoisy = true);

        /// <summary>
        /// generate captcha
        /// </summary>
        /// <param name="pageName">name of page for use captcha - must be is uniq page</param>
        /// <param name="haveNoisy"></param>
        /// <returns>
        /// save  CaptchaSessionValue in session
        /// pass CaptchaSessionValue to IsValid method for validate
        /// </returns>
        CaptchaAsBase64ContentModel GenerateCaptchaAsBase64Content(string pageName, bool haveNoisy = true);
        /// <summary>
        /// validate captcha
        /// </summary>
        /// <param name="sessionValue">value is saved in session</param>
        /// <param name="userValue">value is filled by user in page</param>
        /// <returns>return true if userValue is valid ok</returns>
        bool IsValid(string? sessionValue, string userValue);
    }
}
