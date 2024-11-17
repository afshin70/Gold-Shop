using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.SharedKernel.Tools
{
    public class UploadFileTools
    {
        private string _path = string.Empty;
        private string _fileType = string.Empty;
        private string _filename = string.Empty;
        private IFormFile _file = null;

        public string path
        {
            get { return _path; }
            set { _path = value; }
        }
        public string FileType
        {
            get { return _fileType; }
            set { _fileType = value; }
        }
        public IFormFile file
        {
            get { return file; }
            set { _file = value; }
        }
        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }

        public bool Upload()
        {
            try
            {
                string filePath = string.Format("{0}{1}{2}", _path, _filename, _fileType);
                if (_file.Length > 0)
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        _file.CopyTo(fileStream);
                        return true;
                    }
                }
                else
                    throw new Exception("فایل مشکل دارد");

            }
            catch
            {
                throw new Exception("خطا!!! فایل شما آپلود نشد");
            }
        }

        public void RemoveFile(string path = null)
        {
            string serverPath = path;
            if (string.IsNullOrEmpty(serverPath))
                serverPath = string.Format("{0}{1}{2}", _path, _filename, _fileType);
            if (File.Exists(serverPath))
            {
                File.Delete(serverPath);
            }

        }
    }

}
