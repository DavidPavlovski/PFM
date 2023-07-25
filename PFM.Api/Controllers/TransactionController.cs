using Microsoft.AspNetCore.Mvc;
using PFM.DataTransfer.Transaction;
using PFM.Helpers.Extensions;
using PFM.Helpers.PageSort;
using PFM.Services.Abstraction;

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
            return res.ToOk();
        }

        [HttpPost("{transactionId}/Split")]
        public async Task<IActionResult> SplitTransaction([FromRoute] string transactionId, [FromBody] TransactionSplitDto model)
        {
            var res = await _transactionService.SplitTransactionAsync(transactionId, model);
            return res.ToOk();
        }

        [HttpPost("{transactionId}/Categorize")]
        public async Task<IActionResult> CategorizeTransaction([FromRoute] string transactionId, [FromBody] TransactionCategorizeDto model)
        {
            var res = await _transactionService.CategorizeTransaction(transactionId, model);
            return res.ToOk();
        }
        [HttpPost("AutoCategorize")]
        public async Task<IActionResult> AutoCategorize()
        {
            var res = await _transactionService.AutoCategorize();
            return res.ToOk();
        }
    }
}