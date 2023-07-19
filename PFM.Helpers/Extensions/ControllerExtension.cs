using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;

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
                    return new NotFoundObjectResult(kex.ToExceptionResponse());
                }
                if (error is ArgumentException aEx)
                {
                    return new BadRequestObjectResult(aEx.ToExceptionResponse());
                }
                if (error is InvalidOperationException ioEx)
                {
                    return new BadRequestObjectResult(ioEx.ToExceptionResponse());
                }
                if (error is FileLoadException fEx)
                {
                    return new BadRequestObjectResult(fEx.ToExceptionResponse());
                }
                return new StatusCodeResult(500);
            });
        }
    }
}
