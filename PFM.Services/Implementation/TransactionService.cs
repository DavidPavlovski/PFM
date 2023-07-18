using AutoMapper;
using LanguageExt.Common;
using Microsoft.AspNetCore.Http;
using PFM.DataAccess.Entities;
using PFM.DataAccess.Repositories.Abstraction;
using PFM.DataAccess.UnitOfWork;
using PFM.DataTransfer.Transaction;
using PFM.Helpers.CSVParser;
using PFM.Helpers.PageSort;
using PFM.Mapping.CSVMapping;
using PFM.Services.Abstraction;
using PFM.Validations.Category;
using PFM.Validations.Split;

namespace PFM.Services.Implementation
{
    public class TransactionService : ITransactionService
    {
        readonly ITransactionRepository _transactionRepository;
        readonly IUnitOfWork _unitOfWork;
        readonly IMapper _mapper;
        readonly ICategoryValidator _categoryValidator;
        readonly ISplitValidator _splitValidator;
        readonly ITransactionSplitRepository _transactionSplitRepository;
        public TransactionService(ITransactionRepository transactionRepository, IUnitOfWork unitOfWork, IMapper mapper, ICategoryValidator categoryValidator, ISplitValidator splitValidator, ITransactionSplitRepository transactionSplitRepository)
        {
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _categoryValidator = categoryValidator;
            _splitValidator = splitValidator;
            _transactionSplitRepository = transactionSplitRepository;
        }

        public async Task<PagedSortedList<TransactionResponseDto>> GetTransactionsAsync(PagerSorter pagerSorter)
        {
            var res = await _transactionRepository.GetTransactionsAsync(pagerSorter);
            return _mapper.Map<PagedSortedList<TransactionResponseDto>>(res);
        }

        public async Task<Result<List<TransactionResponseDto>>> ImportFromCSVAsync(IFormFile file)
        {
            var transactions = CSVParser.ParseCSV<Transaction, TransactionCSVMap>(file);
            if (transactions is null)
            {
                var exception = new Exception("Error occured while reading CSV file");
                return new Result<List<TransactionResponseDto>>(exception);
            }

            try
            {
                await _unitOfWork.SaveChangesAsync();
                return _mapper.Map<List<TransactionResponseDto>>(transactions.Take(10).ToList());
            }
            catch
            {
                var exception = new Exception("Error occured while writing in database");
                return new Result<List<TransactionResponseDto>>(exception);
            }
        }
        public async Task<Result<TransactionResponseDto>> CategorizeTransaction(string transactionId, TransactionCategorizeDto model)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction is null)
            {
                var notFoundException = new KeyNotFoundException("Transaction does not exist");
                return new Result<TransactionResponseDto>(notFoundException);
            }
            if (transaction.TransactionSplits.Any())
            {
                var exception = new Exception("Cannot categorize Split Transaction");
                return new Result<TransactionResponseDto>(exception);
            }
            if (!await _categoryValidator.ExistsAsync(model.CatCode))
            {
                var notFoundException = new KeyNotFoundException("Category does not exist");
                return new Result<TransactionResponseDto>(notFoundException);
            }
            transaction.CatCode = model.CatCode;
            _transactionRepository.Update(transaction);
            try
            {
                await _unitOfWork.SaveChangesAsync();
                return _mapper.Map<TransactionResponseDto>(transaction);
            }
            catch
            {
                var exception = new Exception("Error occured while writing in database");
                return new Result<TransactionResponseDto>(exception);
            }
        }

        public async Task<Result<TransactionResponseDto>> SplitTransactionAsync(string transactionId, TransactionSplitDto model)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction is null)
            {
                var notFoundException = new KeyNotFoundException("Transaction does not exist");
                return new Result<TransactionResponseDto>(notFoundException);
            }

            transaction.CatCode = "Z";
            _transactionRepository.Update(transaction);

            if (transaction.TransactionSplits.Any())
            {
                _transactionSplitRepository.DeleteRange(transaction.TransactionSplits);
            }

            if (!_splitValidator.ValidateAmmount(transaction.Ammount, model.Splits.Sum(x => x.Ammount)))
            {
                var notFoundException = new KeyNotFoundException("The transaction split ammounts do not correspond with the total ammount of the transaction");
                return new Result<TransactionResponseDto>(notFoundException);
            }

            var splits = new List<TransactionSplit>();
            foreach (var split in model.Splits)
            {
                if (!await _categoryValidator.ExistsAsync(split.CatCode))
                {
                    var notFoundException = new KeyNotFoundException("Category does not exist");
                    return new Result<TransactionResponseDto>(notFoundException);
                }
                splits.Add(new TransactionSplit
                {
                    TransactionId = transactionId,
                    CatCode = split.CatCode,
                    Ammount = split.Ammount,
                });
            }
            _transactionSplitRepository.AddRange(splits);
            try
            {
                await _unitOfWork.SaveChangesAsync();
                return _mapper.Map<TransactionResponseDto>(transaction);
            }
            catch
            {
                var exception = new Exception("Error occured while writing in database");
                return new Result<TransactionResponseDto>(exception);
            }
        }
    }
}
