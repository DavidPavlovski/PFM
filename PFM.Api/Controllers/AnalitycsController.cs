using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PFM.Helpers.Analytics;
using PFM.Services.Abstraction;

namespace PFM.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalitycsController : ControllerBase
    {
        readonly IAnalyticsService _analyticsService;

        public AnalitycsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("spending-analytics")]
        public async Task<IActionResult> AnalyticsView([FromQuery] AnalyticsQuery analyticsQuery)
        {
            var res = await _analyticsService.GetTransactionAnalyticsAsync(analyticsQuery);
            return Ok(res);
        }
    }
}
