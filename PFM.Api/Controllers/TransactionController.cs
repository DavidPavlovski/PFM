using Microsoft.AspNetCore.Mvc;
using PFM.Services.Abstraction;
using PFM.Helpers.PageSort;
using PFM.DataTransfer.Transaction;

namespace PFM.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactions([FromQuery] PagerSorter pagerSorter)
        {
            var res = await _transactionService.GetTransactionsAsync(pagerSorter);
            return Ok(res);
        }

        [HttpPost("Import")]
        public async Task<IActionResult> ImportTransactions(IFormFile file)
        {
            var res = await _transactionService.ImportFromCSVAsync(file);
            if (res == null)
            {
                return BadRequest("Something went wrong while processing your request");
            }
            return Ok(res);
        }

        [HttpPost("{transactionId}/Split")]
        public async Task<IActionResult> SplitTransaction([FromRoute] string transactionId, [FromBody] TransactionSplitDto model)
        {
            var res = await _transactionService.SplitTransactionAsync(transactionId, model);
            return Ok(res);
        }

        [HttpPost("{transactionId}/Categorize")]
        public async Task<IActionResult> CategorizeTransaction([FromRoute] string transactionId, [FromBody] TransactionCategorizeDto model)
        {

            var res = await _transactionService.CategorizeTransaction(transactionId, model);
            if (res == null)
            {
                return BadRequest("Something went wrong while processing your request");
            }
            return Ok(res);
        }
        [HttpPost("AutoCategorize")]
        public async Task<IActionResult> AutoCategorize()
        {
            return Ok();
        }
    }
}