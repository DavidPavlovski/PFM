using LanguageExt.Common;
using Microsoft.AspNetCore.Http;
using PFM.DataTransfer;
using PFM.DataTransfer.Transaction;
using PFM.Helpers.PageSort;

namespace PFM.Services.Abstraction
{
    public interface ITransactionService
    {
        Task<PagedSortedList<TransactionResponseDto>> GetTransactionsAsync(PagerSorter pagerSorter);
        Task<Result<ResponseModel>> ImportFromCSVAsync(IFormFile file);
        Task<Result<ResponseModel>> CategorizeTransaction(string transactionId, TransactionCategorizeDto model);
        Task<Result<ResponseModel>> SplitTransactionAsync(string transactionId, TransactionSplitDto model);
    }
}
