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
                    return new NotFoundObjectResult(kex);
                }
                if (error is Exception ex)
                {
                    return new BadRequestObjectResult(ex);
                }
                return new StatusCodeResult(500);
            });
        }
    }
}
