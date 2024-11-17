using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Gold.SharedKernel.Attributes.Validation
{
    public class FileContentChecker : ValidationAttribute
    {
        private string _acceptContentTypes;
        public FileContentChecker(string acceptContentTypes)
        {
            _acceptContentTypes = acceptContentTypes.ToLower();
        }
        public override bool IsValid(object? value)
        {
            if (value != null)
            {
                IFormFile file = (IFormFile)value;
                if (!_acceptContentTypes.Split('|').Any(x => x == file.ContentType))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
