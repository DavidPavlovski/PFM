using LanguageExt.Common;
using Microsoft.AspNetCore.Http;
using PFM.DataTransfer.Transaction;
using PFM.Helpers.PageSort;

namespace PFM.Services.Abstraction
{
    public interface ITransactionService
    {
        Task<PagedSortedList<TransactionResponseDto>> GetTransactionsAsync(PagerSorter pagerSorter);
        Task<Result<List<TransactionResponseDto>>> ImportFromCSVAsync(IFormFile file);
        Task<Result<TransactionResponseDto>> CategorizeTransaction(string transactionId, TransactionCategorizeDto model);
        Task<Result<TransactionResponseDto>> SplitTransactionAsync(string transactionId, TransactionSplitDto model);
    }
}
