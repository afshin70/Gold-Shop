using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Gold.SharedKernel.Attributes.Validation
{
    public class FileSizeChecker : ValidationAttribute
    {
        private uint _minFileSize;
        private uint _maxFileSize;
        public FileSizeChecker(uint minFileSize = 0, uint maxFileSize = 0)
        {
            _minFileSize = minFileSize;
            _maxFileSize = maxFileSize;
        }
        public override bool IsValid(object? value)
        {
            if (value != null)
            {
                IFormFile file = (IFormFile)value;
                long fileSize = file.Length / 1024;

                if (fileSize < _minFileSize & fileSize > _maxFileSize)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
