using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Gold.EndPoints.Presentation.Models
{
    public class ToastrResult
    {
        public ToastrResult()
        {

        }
        public ToastrResult(bool isSuccess, string message, string title, ToastrType type)
        {
            IsSuccess = isSuccess;
            Message = message;
            Title = title;
            Type = type;
        }

        public bool IsSuccess { get; set; }
        
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public ToastrType Type { get; set; }
        public static ToastrResult Error(string title, string message)
        {
            return new ToastrResult
            {
                IsSuccess = false,
                Message=message,
                Title=title,
                Type = ToastrType.Error,
            };
        }
        public static ToastrResult Warning(string title, string message)
        {
            return new ToastrResult
            {
                IsSuccess = false,
                Message = message,
                Title = title,
                Type = ToastrType.Warning,
            };
        }
        public static ToastrResult Info(string title, string message)
        {
            return new ToastrResult
            {
                IsSuccess = true,
                Message = message,
                Title = title,
                Type = ToastrType.Info,
            };
        }
        public static ToastrResult Success(string title, string message)
        {
            return new ToastrResult
            {
                IsSuccess = true,
                Message = message,
                Title = title,
                Type = ToastrType.Success,
            };
        }
    }
    public class ToastrResult<TData>: ToastrResult
    {
        public ToastrResult()
        {

        }
        public ToastrResult(TData data, bool isSuccess, string message, string title, ToastrType type)
        {
            Data = data;
            IsSuccess = isSuccess;
            Message = message;
            Title = title;
            Type = type;
        }

        public TData Data { get; set; }
        public static ToastrResult<TData> Error(string title, string message, TData data)
        {
            return new ToastrResult<TData>
            {
                IsSuccess = false,
                Message = message,
                Title = title,
                Type = ToastrType.Error,
                Data=data
            };
        }
        public static ToastrResult<TData> Warning(string title, string message, TData data)
        {
            return new ToastrResult<TData>
            {
                IsSuccess = false,
                Message = message,
                Title = title,
                Type = ToastrType.Warning,
                Data = data
            };
        }
        public static ToastrResult<TData> Info(string title, string message, TData data)
        {
            return new ToastrResult<TData>
            {
                IsSuccess = true,
                Message = message,
                Title = title,
                Type = ToastrType.Info,
                Data = data
            };
        }
        public static ToastrResult<TData> Success(string title, string message, TData data)
        {
            return new ToastrResult<TData>
            {
                IsSuccess = true,
                Message = message,
                Title = title,
                Type = ToastrType.Success,
                Data = data
            };
        }

    }
    //public static class ToastrType
    //{
    //    public const string Success = "success";
    //    public const string Info = "info";
    //    public const string Warning = "warning";
    //    public const string Error = "error";
    //}
    public enum ToastrType
    {
        Success=0,
        Error=1,
        Warning=2,
        Info=3
    }
    public static class ToastrPosition
    {
        public const string TopRight = "toast-top-right";
        public const string BottomRight = "toast-bottom-right";
        public const string BottomLeft = "toast-bottom-left";
        public const string TopLeft = "toast-top-left";
        public const string TopFullWidth = "toast-top-full-width";
        public const string BottomFullWidth = "toast-bottom-full-width";
        public const string TopCenter = "toast-top-center";
        public const string BottomCenter = "toast-bottom-center";
    }
}
