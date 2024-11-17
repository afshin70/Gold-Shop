using Gold.SharedKernel.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Gold.SharedKernel.ExtentionMethods;
using System.Net.Http;
using Gold.SharedKernel.DTO.Captcha;

namespace Gold.Infrastracture.ExternalService
{
    public class CaptchaResolver: ICaptchaResolver
    {
        /// <summary>
        /// validate captcha
        /// </summary>
        /// <param name="sessionValue">value is saved in session</param>
        /// <param name="userValue">value is filled by user in page</param>
        /// <returns>return true if userValue is valid ok</returns>
        public bool IsValid(string? sessionValue,string userValue)
        {
            sessionValue ??= string.Empty;
			if (sessionValue.IsEmptyOrNull()|| userValue.IsEmptyOrNull())
               return false;
            return sessionValue.ToEnglishNumbers() == userValue.ToEnglishNumbers();
        }

        /// <summary>
        /// generate captcha as Base64
        /// </summary>
        /// <param name="pageName">name of page for use captcha - must be is uniq page</param>
        /// <param name="haveNoisy"></param>
        /// <returns>
        /// save  CaptchaSessionValue in session
        /// pass CaptchaSessionValue to IsValid method for validate
        /// </returns>
        public CaptchaAsBase64ContentModel GenerateCaptchaAsBase64Content(string pageName,bool haveNoisy=true)
        {
            var result = new CaptchaAsBase64ContentModel();
            var rand = new Random((int)DateTime.Now.Ticks);
            int number = rand.Next(100000, 999999);
            var captcha = string.Format("    {0}", number);
            result.CaptchaSessionName = pageName;
            result.CaptchaSessionValue = number.ToString();
            using (var mem = new MemoryStream())

            using (var bmp = new Bitmap(120, 40))
            using (var gfx = Graphics.FromImage((Image)bmp))
            {
                gfx.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                gfx.SmoothingMode = SmoothingMode.AntiAlias;
                gfx.FillRectangle(Brushes.White, new Rectangle(0, 0, bmp.Width, bmp.Height));
                if (haveNoisy)
                {
                    int i, r, x, y;
                    var pen = new Pen(Color.Yellow);
                    for (i = 1; i < 10; i++)
                    {
                        pen.Color = Color.FromArgb(
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)));
                        r = rand.Next(0, (120 / 3));
                        x = rand.Next(0, 120);
                        y = rand.Next(0, 40);
                        gfx.DrawLine(pen, (float)x, (float)y, (float)r, (float)r);
                    }
                }
                gfx.DrawString(number.ToString().ToFaNumber(), new Font("Tahoma", 22), Brushes.Gray, 2, 3);
                bmp.Save(mem, System.Drawing.Imaging.ImageFormat.Jpeg);
                FileContentResult img =  new FileContentResult(mem.GetBuffer(), "image/Jpeg");
               result.Content= "data:image/jpeg;base64," + Convert.ToBase64String(img.FileContents);
            }
            return result;
        }

        /// <summary>
        /// generate captcha as FileContentResult
        /// </summary>
        /// <param name="pageName">name of page for use captcha - must be is uniq page</param>
        /// <param name="haveNoisy"></param>
        /// <returns>
        /// save  CaptchaSessionValue in session
        /// pass CaptchaSessionValue to IsValid method for validate
        /// </returns>
        public CaptchaAsFileContentResultModel GenerateCaptchaAsFileContentResult(string pageName, bool haveNoisy = true)
        {
            var result = new CaptchaAsFileContentResultModel();
            var rand = new Random((int)DateTime.Now.Ticks);
            int number = rand.Next(100000, 999999);
            var captcha = string.Format("    {0}", number);
            result.CaptchaSessionName = pageName;
            result.CaptchaSessionValue = number.ToString();
            using (var mem = new MemoryStream())

            using (var bmp = new Bitmap(120, 40))
            using (var gfx = Graphics.FromImage((Image)bmp))
            {
                gfx.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                gfx.SmoothingMode = SmoothingMode.AntiAlias;
                gfx.FillRectangle(Brushes.White, new Rectangle(0, 0, bmp.Width, bmp.Height));
                if (haveNoisy)
                {
                    int i, r, x, y;
                    var pen = new Pen(Color.Yellow);
                    for (i = 1; i < 10; i++)
                    {
                        pen.Color = Color.FromArgb(
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)));
                        r = rand.Next(0, (120 / 3));
                        x = rand.Next(0, 120);
                        y = rand.Next(0, 40);
                        gfx.DrawLine(pen, (float)x, (float)y, (float)r, (float)r);
                    }
                }
                gfx.DrawString(number.ToString().ToFaNumber(), new Font("Tahoma", 22), Brushes.Gray, 2, 3);
                bmp.Save(mem, System.Drawing.Imaging.ImageFormat.Jpeg);
                result.Content = new FileContentResult(mem.GetBuffer(), "image/Jpeg");
            }
            return result;
        }
        //private FileContentResult PrepareImage(string prefix, bool noisy)
        //{
        //    var rand = new Random((int)DateTime.Now.Ticks);

        //    int number = rand.Next(100000, 999999);
        //    // string rndStr = RandomString(6);
        //    //int b = rand.Next(0, 9);
        //    var captcha = string.Format("    {0}", number);
        //    //store answer

        //    Session["Captcha" + prefix] = number;// + b;
        //    //image stream
        //    FileContentResult img = null;

        //    using (var mem = new MemoryStream())

        //    using (var bmp = new Bitmap(120, 40))
        //    using (var gfx = Graphics.FromImage((Image)bmp))
        //    {
        //        gfx.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        //        gfx.SmoothingMode = SmoothingMode.AntiAlias;
        //        gfx.FillRectangle(Brushes.White, new Rectangle(0, 0, bmp.Width, bmp.Height));

        //        //add noise
        //        if (noisy)
        //        {
        //            int i, r, x, y;
        //            var pen = new Pen( Color.Yellow);
        //            for (i = 1; i < 10; i++)
        //            {
        //                pen.Color = Color.FromArgb(
        //                (rand.Next(0, 255)),
        //                (rand.Next(0, 255)),
        //                (rand.Next(0, 255)));

        //                r = rand.Next(0, (120 / 3));
        //                x = rand.Next(0, 120);
        //                y = rand.Next(0, 40);

        //                gfx.DrawLine(pen, (float)x, (float)y, (float)r, (float)r);
        //            }
        //        }

        //        //add question
        //        gfx.DrawString(number.ToString().ToFaNumber(), new Font("Tahoma", 22), Brushes.Gray, 2, 3);

        //        //render as Jpeg
        //        bmp.Save(mem, System.Drawing.Imaging.ImageFormat.Jpeg);
        //        //img =this.File(mem.GetBuffer(), "image/Jpeg");
        //        img = new FileContentResult(mem.GetBuffer(), "image/Jpeg");
        //    }

        //    return img;
        //}
    }
}
