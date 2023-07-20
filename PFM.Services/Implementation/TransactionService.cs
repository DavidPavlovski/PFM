using AutoMapper;
using LanguageExt.Common;
using Microsoft.AspNetCore.Http;
using PFM.DataAccess.Entities;
using PFM.DataAccess.Repositories.Abstraction;
using PFM.DataAccess.UnitOfWork;
using PFM.DataTransfer.Transaction;
using PFM.Exceptions;
using PFM.Helpers.CSVParser;
using PFM.Helpers.PageSort;
using PFM.Mapping.CSVMapping;
using PFM.Services.Abstraction;
using PFM.Validations.Category;
using PFM.Validations.Split;
using System;
using System.Net;

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
        readonly ICSVParser _csvParser;
        public TransactionService(ITransactionRepository transactionRepository, IUnitOfWork unitOfWork, IMapper mapper, ICategoryValidator categoryValidator, ISplitValidator splitValidator, ITransactionSplitRepository transactionSplitRepository, ICSVParser csvParser)
        {
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _categoryValidator = categoryValidator;
            _splitValidator = splitValidator;
            _transactionSplitRepository = transactionSplitRepository;
            _csvParser = csvParser;
        }

        public async Task<PagedSortedList<TransactionResponseDto>> GetTransactionsAsync(PagerSorter pagerSorter)
        {
            var res = await _transactionRepository.GetTransactionsAsync(pagerSorter);
            return _mapper.Map<PagedSortedList<TransactionResponseDto>>(res);
        }

        public async Task<Result<List<TransactionResponseDto>>> ImportFromCSVAsync(IFormFile file)
        {
            CSVReponse<Transaction> csvResponse;
            try
            {
                csvResponse = _csvParser.ParseCSV<Transaction, TransactionCSVMap>(file);
                if (csvResponse.Errors.Any())
                {
                    var exception = new CustomException("Error occured while reading CSV file")
                    {
                        Description = "Problem occured while parsing CSV values , see errors below.",
                        StatusCode = HttpStatusCode.BadRequest,
                        Errors = csvResponse.Errors
                    };
                    return new Result<List<TransactionResponseDto>>(exception);
                }
                _transactionRepository.ImportTransactions(csvResponse.Items);
            }
            catch (ArgumentException aex)
            {
                return new Result<List<TransactionResponseDto>>(aex);
            }

            try
            {
                await _unitOfWork.SaveChangesAsync();
                return _mapper.Map<List<TransactionResponseDto>>(csvResponse.Items.Take(10).ToList());
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
                var exception = new InvalidOperationException("Cannot categorize Split Transaction");
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
                var notFoundException = new ArgumentException("The transaction split ammounts do not correspond with the total ammount of the transaction");
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
