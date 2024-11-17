using Gold.SharedKernel.Contract;
using Gold.SharedKernel.ExtentionMethods;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Drawing.Printing;
using System.IO;
using System.Threading.Tasks;

namespace Gold.Infrastracture.ExternalService
{
    public class FileService : IFileService
    {
        private readonly ILogManager _logManager;

        public FileService(ILogManager logManager)
        {
            this._logManager = logManager;
        }


        public async Task<bool> SaveFileAsync(IFormFile file, string fileName, string savePath)
        {
            try
            {
                string filePath = savePath + fileName;
                if (file.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                return true;
            }
            catch (Exception ex) { _logManager.RaiseLog(ex); return false; }

        }
        //public bool SaveFile(IFormFile file, string fileName, string savePath, ImageSize size)
        //{
        //    switch (size)
        //    {
        //        case ImageSize.AnySize:
        //            return SaveFile(file, fileName, savePath);
        //        case ImageSize.MainSlider:
        //            return SaveImageFile(file, 1140, 420, savePath, fileName);
        //        case ImageSize.DoctorSldier:
        //            return SaveImageFile(file, 960, 566, savePath, fileName);
        //        case ImageSize.HelthCenterSlider:
        //            return SaveImageFile(file, 960, 566, savePath, fileName);
        //        case ImageSize.DoctorImage:
        //            return SaveImageFile(file, 300, 300, savePath, fileName);
        //        case ImageSize.IconImage:
        //            return SaveImageFile(file, 200, 200, savePath, fileName);
        //        case ImageSize.PostThumb:
        //            return SaveImageFile(file, 284, 180, savePath, fileName);
        //        case ImageSize.GalleryThumb:
        //            int width = 0, height = 0, temp = 0;
        //            using (var image = System.Drawing. Image.FromStream(file.OpenReadStream()))
        //            {
        //                width = image.Width;
        //                height = image.Height;
        //                temp = (width + height) / 2 / 300;
        //                width = image.Width / temp;
        //                height = image.Height / temp;
        //            }
        //            return SaveImageFile(file, width, height, savePath, fileName);
        //        default:
        //            return SaveFile(file, fileName, savePath);
        //    }
        //}
        public bool SaveFile(IFormFile file, string fileName, string savePath)
        {
            try
            {
                string filePath = savePath + fileName;
                if (file.Length > 0)
                {
                    if (!Directory.Exists(savePath))
                        Directory.CreateDirectory(savePath);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
                return true;
            }
            catch (Exception ex) { _logManager.RaiseLog(ex); return false; }

        }
        /// <summary>
        /// ذخیره تصویر
        /// </summary>
        /// <param name="img">تصویر بایت شده</param>
        /// <param name="savePath">دایرکتوری</param>
        /// <param name="imageName">نام تصویر</param>
        /// <returns></returns>
        public bool SaveImageFile(byte[] img, string savePath, string imageName)
        {
            try
            {
                using (Image image = Image.Load(img))
                {
                    string filePath = savePath + imageName;
                    if (!Directory.Exists(savePath))
                        Directory.CreateDirectory(savePath);
                    image.Save(filePath);
                    return true;
                }
            }
            catch (Exception ex)
            {

                _logManager.RaiseLog(ex);
                return false;
            }
        }
        /// <summary>
        /// ذخیره تصویر
        /// </summary>
        /// <param name="img">تصویر بایت شده</param>
        /// <param name="width">طول</param>
        /// <param name="height">ارتفاع</param>
        /// <param name="savePath">دایرکتوری</param>
        /// <param name="imageName">نام تصویر</param>
        /// <returns></returns>
        public bool SaveImageFile(byte[] img, int width, int height, string savePath, string imageName)
        {
            try
            {
                using (Image image = Image.Load(img))
                {
                    string filePath = savePath + imageName;
                    if (!Directory.Exists(savePath))
                        Directory.CreateDirectory(savePath);
                    image.Mutate(x => x.Resize(width: width, height: height));
                    image.Save(filePath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return false;
            }
        }
        /// <summary>
        /// ذخیره تصویر
        /// </summary>
        /// <param name="img">فایل تصویر</param>
        /// <param name="savePath">دایرکتوری</param>
        /// <param name="imageName">نام تصویر</param>
        /// <returns></returns>
        public bool SaveImageFile(IFormFile img, string savePath, string imageName)
        {
            try
            {

                using (var image = Image.Load(img.OpenReadStream()))
                {
                    string filePath = savePath + imageName;
                    if (!Directory.Exists(savePath))
                        Directory.CreateDirectory(savePath);
                    image.Save(filePath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return false;
            }
        }
        /// <summary>
        /// ذخیره تصویر
        /// </summary>
        /// <param name="img">فایل تصویر</param>
        /// <param name="width">طول</param>
        /// <param name="height">ارتفاع</param>
        /// <param name="savePath">دایرکتوری</param>
        /// <param name="imageName">نام تصویر</param>
        /// <returns></returns>
        public bool SaveImageFile(IFormFile img, int width, int height, string savePath, string imageName)
        {
            try
            {
                string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, savePath));
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                using (var image = Image.Load(img.OpenReadStream()))
                {
                    path = Path.Combine(path, $"{imageName}{Path.GetExtension(img.FileName)}");
                    image.Mutate(x => x.Resize(width: width, height: height));
                    image.Save(path);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return false;
            }
        }  
      


        /// <summary>
        /// بروزرسانی فایل تصویر
        /// </summary>
        /// <param name="newImg">فایل جدید</param>
        /// <param name="width">طول</param>
        /// <param name="height">ارتفاع</param>
        /// <param name="path">دایرکتوری</param>
        /// <param name="NewimageName">نام جدید</param>
        /// <param name="oldImageName">نام قبلی</param>
        /// <returns></returns>
        public bool UpdateImageFile(IFormFile newImg, int width, int height, string path, string NewimageName, string oldImageName)
        {
            try
            {
                if (DeleteFile(oldImageName, path))
                    return SaveImageFile(newImg, width, height, path, NewimageName);
                return false;
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return false;
            }
        }

        public async Task<bool> UploadFileAsync(IFormFile file)
        {
            string path = "";
            try
            {
                if (file.Length > 0)
                {
                    path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "UploadedFiles"));
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    using (var fileStream = new FileStream(Path.Combine(path, file.FileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return false;
            }
        }
        public async Task<bool> UploadFileAsync(IFormFile file, string pathRoot)
        {
            string path = "";
            try
            {
                if (file.Length > 0)
                {
                    path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, pathRoot));
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    using (var fileStream = new FileStream(Path.Combine(path, file.FileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return false;
            }
        }
        public async Task<bool> UploadFileAsync(IFormFile file, string pathRoot, string fileName)
        {
            string path = "";
            try
            {
                if (file.Length > 0)
                {
                    path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, pathRoot));
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    using (var fileStream = new FileStream(Path.Combine(path, $"{fileName}{Path.GetExtension(file.FileName)}"), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return false;
            }
        }

        public bool IsExist(IFormFile file, string pathRoot)
        {
            string path = string.Empty, newFilePath = string.Empty;
            try
            {
                if (file.Length > 0)
                {
                    path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, pathRoot));
                    newFilePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, pathRoot, file.FileName));
                    var files = Directory.GetFiles(path);
                    if (files.Any(x => x == newFilePath))
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return false;
            }
        }
        public bool IsExist(IFormFile file, string pathRoot, string exeptionFileName)
        {
            string path = string.Empty, newFilePath = string.Empty, exeptionFileNamePath = string.Empty;
            try
            {
                if (file.Length > 0)
                {
                    path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, pathRoot));
                    newFilePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, pathRoot, file.FileName));
                    exeptionFileNamePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, pathRoot, exeptionFileName));
                    var files = Directory.GetFiles(path);
                    if (files.Any(x => x == newFilePath & x != exeptionFileNamePath))
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return false;
            }
        }
        public bool IsExist(string fileName, string pathRoot)
        {
            string path = string.Empty, newFilePath = string.Empty;
            try
            {
                if (fileName.Length > 0)
                {
                    path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, pathRoot));
                    newFilePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, pathRoot, fileName));
                    var files = Directory.GetFiles(path);
                    if (files.Any(x => x == newFilePath))
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return false;
            }
        }

        public bool IsExist(string fileName, string exeptionFileName, string pathRoot)
        {
            string path = string.Empty, newFilePath = string.Empty, exeptionFileNamePath = string.Empty;
            try
            {
                if (fileName.Length > 0)
                {
                    path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, pathRoot));
                    newFilePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, pathRoot, fileName));
                    exeptionFileNamePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, pathRoot, exeptionFileName));
                    var files = Directory.GetFiles(path);
                    if (files.Any(x => x == newFilePath & x != exeptionFileNamePath))
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return false;
            }
        }

        /// <summary>
        /// حذف فایل
        /// </summary>
        /// <param name="fileName">نام فایل</param>
        /// <param name="savePath">دایرکتوری</param>
        /// <returns></returns>
        public bool DeleteFile(string? fileName, string pathRoot)
        {
            try
            {
                if (fileName.IsEmptyOrNull())
                    return true;
                string filePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, pathRoot, fileName));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                return true;
            }
            catch (Exception ex) { _logManager.RaiseLog(ex); return false; }

        }

        public bool UpdateFile(IFormFile newFile, string newFileName, string oldFileName, string savePath)
        {
            try
            {
                DeleteFile(oldFileName, savePath);
                SaveFile(newFile, newFileName, savePath);
                return true;
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return false;
            }

        }

        public async Task<bool> UpdateFileAsync(IFormFile newFile, string oldFileName, string updatePath)
        {
            try
            {
                if (DeleteFile(oldFileName, updatePath))
                {
                    return await UploadFileAsync(newFile, updatePath);
                }
                return false;
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return false;
            }

        }
        public async Task<bool> UpdateFileAsync(IFormFile newFile, string newFileName, string oldFileName, string updatePath)
        {
            try
            {
                if (DeleteFile(oldFileName, updatePath))
                {
                    return await UploadFileAsync(newFile, updatePath, newFileName);
                }
                return false;
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return false;
            }

        }

        public async Task<byte[]> GetFileBytesAsync(string filePath)
        {
            try
            {
                return await System.IO.File.ReadAllBytesAsync(filePath);
            }
            catch (Exception ex)
            {

                _logManager.RaiseLog(ex);
                return null;
            }
        }

        public byte[] GetFileBytes(string filePath)
        {
            try
            {
                return System.IO.File.ReadAllBytes(filePath);
            }
            catch (Exception ex)
            {

                _logManager.RaiseLog(ex);
                return null;
            }
        }

        public string GetMimeTypeForFileExtension(string filePath)
        {
            const string DefaultContentType = "application/octet-stream";

            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(filePath, out string contentType))
            {
                contentType = DefaultContentType;
            }

            return contentType;
        }

        public bool CopyFile(string sourceFilePath, string destinationFilePath, bool overWrite = true)
        {
            try
            {
                sourceFilePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, sourceFilePath));
                destinationFilePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, destinationFilePath));
                File.Copy(sourceFilePath, destinationFilePath, overWrite);
                return true;
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return false;
            }
        }


        public byte[] CreateExcelFileFromDataTable(System.Data.DataTable SearchedList, double[] ColumnWidth, string titleName, bool RightToLeft = false)
        {
            string[] Alphabet = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z".Split(',');
            var LastColumnExcelInHeadersName = Alphabet[SearchedList.Columns.Count - 1];
            byte[] file;
            using (var stream = new MemoryStream())
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                using (var excel = new ExcelPackage(stream))
                {
                    // اضافه کردن یک ورک شیت جدید
                    OfficeOpenXml.ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add(titleName);
                    worksheet.HeaderFooter.FirstHeader.CenteredText = titleName;
                    worksheet.HeaderFooter.AlignWithMargins = true;
                    worksheet.PrinterSettings.FitToPage = true;
                    worksheet.PrinterSettings.PaperSize = ePaperSize.A4Transverse;

                    //worksheet.Column(5).Style.WrapText = true;
                    //worksheet.Column(6).Style.WrapText = true;
                    //worksheet.Column(9).Style.WrapText = true;

                    for (int j = 0; j <= SearchedList.Rows.Count; j++)
                    {
                        for (int k = 1; k <= SearchedList.Columns.Count; k++)
                        {
                            worksheet.Cells[j + 1, k].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            worksheet.Cells[j + 1, k].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[j + 1, k].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells[j + 1, k].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[j + 1, k].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[j + 1, k].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[j + 1, k].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[j + 1, k].Style.Border.Right.Color.SetColor(System.Drawing.Color.Gray);
                            worksheet.Cells[j + 1, k].Style.Border.Left.Color.SetColor(System.Drawing.Color.Gray);
                            worksheet.Cells[j + 1, k].Style.Border.Top.Color.SetColor(System.Drawing.Color.Gray);
                            worksheet.Cells[j + 1, k].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Gray);
                            worksheet.Cells[j + 1, k].Style.Font.Name = "IRANSansWeb";
                            worksheet.Cells[j + 1, k].Style.Font.Size = 10;
                            if (j % 2 == 0)
                            {
                                worksheet.Cells[j + 1, k].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(242, 242, 242));
                            }
                            else
                            {
                                worksheet.Cells[j + 1, k].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                            }

                            if (k == 1)
                            {
                                worksheet.Cells[j + 1, k].Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                                worksheet.Cells[j + 1, k].Style.Border.Left.Style = ExcelBorderStyle.Thick;
                            }
                        }
                    }

                    worksheet.Cells["A1:" + LastColumnExcelInHeadersName + "1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A1:" + LastColumnExcelInHeadersName + "1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //worksheet.Cells["A1:G1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    worksheet.Cells["A1:" + LastColumnExcelInHeadersName + "1"].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    worksheet.Cells["A1:" + LastColumnExcelInHeadersName + "1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells["A1:" + LastColumnExcelInHeadersName + "1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(191, 191, 191));
                    worksheet.Cells["A1:" + LastColumnExcelInHeadersName + "1"].Style.Font.Bold = true;
                    //worksheet.Cells["A1:G1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A1:" + LastColumnExcelInHeadersName + "1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A1:" + LastColumnExcelInHeadersName + "1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A1:" + LastColumnExcelInHeadersName + "1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A1:" + LastColumnExcelInHeadersName + "1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A1:" + LastColumnExcelInHeadersName + "1"].Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    worksheet.Cells["A1:" + LastColumnExcelInHeadersName + "1"].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    //worksheet.Cells["G7"].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    //worksheet.Cells["G7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //worksheet.Cells["G7"].Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    worksheet.Cells["A1:" + LastColumnExcelInHeadersName + "1"].Style.Font.Name = "IRANSansWeb";
                    worksheet.Cells["A1:" + LastColumnExcelInHeadersName + "1"].Style.Font.Size = 10;
                    worksheet.Row(1).Height = 30;
                    for (int k = 0; k <= SearchedList.Columns.Count - 1; k++)
                    {
                        var Width = ColumnWidth[k];
                        worksheet.Column(k + 1).Width = Width;
                    }

                    //اضافه کردن یک جدول جدید از دیتاتیبل دریافتی
                    worksheet.Cells["A1"].LoadFromDataTable(SearchedList, true/*, TableStyles.Medium2*/);
                    worksheet.Cells.LoadFromDataTable(SearchedList, true);
                    worksheet.View.PageLayoutView = false;
                    worksheet.View.RightToLeft = RightToLeft;
                    excel.Save();
                    file = stream.ToArray();
                }
            }
            return file;
        }
    }
}
