using System.Runtime.CompilerServices;

namespace PFM.Helpers.Extensions
{
    public class ExceptionResponse
    {
        public string Message { get; set; }
    }
    public static class ExceptionExtension
    {
        public static ExceptionResponse ToExceptionResponse(this Exception ex)
        {
            return new ExceptionResponse
            {
                Message = ex.Message,
            };
        }
    }
}