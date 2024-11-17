using System;
using System.IO;
using System.Linq;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.FileAddress;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Gold.EndPoints.Presentation.Areas.Dashboard.Controllers
{
    public class FileController : Controller
    {
        
         private readonly IWebHostEnvironment _env;
        private readonly IFileService _fileService;
        private readonly FilePathAddress _filePathAddress;
        public FileController(IWebHostEnvironment env, IFileService fileService, IOptions<FilePathAddress> filePathAddressOptions)
        {
            _env = env;
            _fileService = fileService;
            _filePathAddress = filePathAddressOptions.Value;
        }


        [HttpPost]
        public async Task<ActionResult> UploadImage(IFormFile upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            string vImagePath = String.Empty;
            string vMessage = String.Empty;
            string vFilePath = String.Empty;
            string vOutput = String.Empty;
            var guid = Guid.NewGuid().ToString();
            guid = guid.Substring(0, 4);
            bool isValidExtention = false;
            var validExtentions = new[] { ".jpg", ".bmp", ".png", ".jpeg", ".gif", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt" };
            try
            {
              
                if (upload != null)
                {
                    //var fileName = Guid.NewGuid() + Path.GetExtension(upload.FileName).ToLower();
                    string imageName =$"{Guid.NewGuid().ToString()}";
                    var extension = Path.GetExtension(upload.FileName);
                    if (extension != null)
                    {
                        var fileExt = extension.ToLower();

                        if (validExtentions.Any(x => x == fileExt))
                            isValidExtention = true;
                        else
                            vMessage = "نوع فایل مجاز نمی باشد";
                    }
                    if (isValidExtention && upload.Length > 0)
                    {
                        if (await _fileService.UploadFileAsync(upload,_filePathAddress.EditorImage, imageName))
                        {
                            vMessage = string.Format("تصویر مربوطه با موفقیت آپلود شد", $"{imageName}{extension}");
                        }

                        //var path = Path.Combine(
                        //    Directory.GetCurrentDirectory(), "wwwroot/EditorImage",
                        //    fileName);
                        vImagePath =Url.Content($"{_filePathAddress.EditorImageUrl}{imageName}{extension}") ;

                        //using (var stream = new FileStream(path, FileMode.Create))
                        //{
                        //    upload.CopyTo(stream);

                        //}
                        //if (System.IO.File.Exists(Path.Combine("wwwroot/EditorImage/", fileName)))
                        //{
                        //    vMessage = string.Format("تصویر مربوطه با موفقیت آپلود شد",upload.FileName);
                        //}
                      
                        
                    }
                }
            }
            catch
            {
                vMessage = "There was an issue uploading";
            }
            vOutput = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + vImagePath + "\", \"" + vMessage + "\");</script></body></html>";
            return new ContentResult
            {
                ContentType = "text/html",
                Content = vOutput
            };
        } 
        [HttpPost]
        public ActionResult UploadFile(IFormFile upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
           
          //  string vImagePath = String.Empty;
            string vMessage = String.Empty;
            string vFilePath = String.Empty;
            string vOutput = String.Empty;
            var guid = Guid.NewGuid().ToString();
            guid = guid.Substring(0, 4);
            bool isValidExtention = false;
            var validExtentions = new[] { ".jpg", ".bmp", ".png", ".jpeg", ".gif", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt" };
            try
            {
              
                if (upload != null)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(upload.FileName).ToLower();
                    var extension = Path.GetExtension(upload.FileName);
                    if (extension != null)
                    {
                        var fileExt = extension.ToLower();

                        if (validExtentions.Any(x => x == fileExt))
                            isValidExtention = true;
                        else
                            vMessage = "نوع فایل مجاز نمی باشد";
                    }
                    if (isValidExtention && upload.Length > 0)
                    {
                        
                        var path = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot/EditorImage",
                            fileName);
                        vFilePath =Url.Content("/EditorImage/" + fileName) ;
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            upload.CopyTo(stream);

                        }
                        if (System.IO.File.Exists(Path.Combine("wwwroot/EditorImage/", fileName)))
                        {
                            vMessage = string.Format("فایل مربوطه با موفقیت آپلود شد",upload.FileName);
                        }
                      
                       
                    }
                }
            }
            catch
            {
                vMessage = "There was an issue uploading";
            }
            vOutput = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + vFilePath + "\", \"" + vMessage + "\");</script></body></html>";
            return new ContentResult
            {
                ContentType = "text/html",
                Content = vOutput
            };
        }
    }
}