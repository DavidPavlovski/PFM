using Microsoft.AspNetCore.Http;
using PFM.DataAccess.Entities;
using PFM.DataTransfer.Transaction;
using PFM.Helpers.PageSort;
using System.IO.Abstractions;

namespace PFM.Services.Abstraction
{
    public interface ITransactionService
    {
        Task<PagedSortedList<TransactionResponseDto>> GetTransactionsAsync(PagerSorter pagerSorter);
        Task<List<TransactionResponseDto>> ImportFromCSVAsync(IFormFile file);
        Task<TransactionResponseDto> CategorizeTransaction(string transactionId, TransactionCategorizeDto model);
        Task<TransactionResponseDto> SplitTransactionAsync(string transactionId, TransactionSplitDto model);
    }
}
