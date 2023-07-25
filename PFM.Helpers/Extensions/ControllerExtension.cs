using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;
using PFM.Exceptions;

namespace PFM.Helpers.Extensions
{
    public static class ControllerExtension
    {
        public static IActionResult ToOk<TResult>(this Result<TResult> result)
        {
            return result.Match<IActionResult>(success =>
            {
                return new OkObjectResult(success);
            }, error =>
            {
                if (error is KeyNotFoundException kex)
                {
                    var errorResponse = new ErrorResponse()
                    {
                        StatusCode = 404,
                        Message = kex.Message
                    };
                    return new NotFoundObjectResult(errorResponse);
                }
                if (error is ArgumentException aEx)
                {
                    var errorResponse = new ErrorResponse()
                    {
                        StatusCode = 400,
                        Message = aEx.Message
                    };
                    return new BadRequestObjectResult(errorResponse);
                }
                if (error is InvalidOperationException ioEx)
                {
                    var errorResponse = new ErrorResponse()
                    {
                        StatusCode = 400,
                        Message = ioEx.Message
                    };
                    return new BadRequestObjectResult(errorResponse);
                }
                if (error is FileLoadException fEx)
                {
                    var errorResponse = new ErrorResponse()
                    {
                        StatusCode = 400,
                        Message = fEx.Message
                    };
                    return new BadRequestObjectResult(errorResponse);
                }
                if (error is CustomException cex)
                {
                    var errorResponse = new CSVErrorListResponse()
                    {
                        Message = cex.Message,
                        StatusCode = 400,
                        Errors = cex.Errors
                    };
                    return new BadRequestObjectResult(errorResponse);
                }
                return new StatusCodeResult(500);
            });
        }
    }
}
