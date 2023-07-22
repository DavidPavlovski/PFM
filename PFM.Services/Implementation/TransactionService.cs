using AutoMapper;
using LanguageExt.Common;
using Microsoft.AspNetCore.Http;
using PFM.DataAccess.Entities;
using PFM.DataAccess.Repositories.Abstraction;
using PFM.DataAccess.UnitOfWork;
using PFM.DataTransfer;
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

        public async Task<Result<ResponseModel>> ImportFromCSVAsync(IFormFile file)
        {


            var csvResponse = _csvParser.ParseCSV<Transaction, TransactionCSVMap>(file);
            if (csvResponse is null)
            {
                var error = new FileLoadException("Error occured while reading file , make sure the file you uploaded is a valid CSV file.");
                return new Result<ResponseModel>(error);
            }
            if (csvResponse.Errors.Any())
            {
                var exception = new CustomException("Error occured while reading CSV file")
                {
                    Description = "Problem occured while parsing CSV values , see errors below.",
                    StatusCode = HttpStatusCode.BadRequest,
                    Errors = csvResponse.Errors
                };
                return new Result<ResponseModel>(exception);
            }
            _transactionRepository.ImportTransactions(csvResponse.Items);


            try
            {
                await _unitOfWork.SaveChangesAsync();
                var res = new ResponseModel
                {
                    Message = "Transactions imported successfully."
                };
                return res;
            }
            catch
            {
                var exception = new Exception("Error occured while writing in database");
                return new Result<ResponseModel>(exception);
            }
        }
        public async Task<Result<ResponseModel>> CategorizeTransaction(string transactionId, TransactionCategorizeDto model)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction is null)
            {
                var notFoundException = new KeyNotFoundException("Transaction does not exist");
                return new Result<ResponseModel>(notFoundException);
            }
            if (transaction.TransactionSplits.Any())
            {
                var exception = new InvalidOperationException("Cannot categorize Split Transaction");
                return new Result<ResponseModel>(exception);
            }
            if (!await _categoryValidator.ExistsAsync(model.CatCode))
            {
                var notFoundException = new KeyNotFoundException("Category does not exist");
                return new Result<ResponseModel>(notFoundException);
            }
            transaction.CatCode = model.CatCode;
            _transactionRepository.Update(transaction);
            try
            {
                await _unitOfWork.SaveChangesAsync();
                var res = new ResponseModel
                {
                    Message = "Transaction categorized successfully."
                };
                return res;
            }
            catch
            {
                var exception = new Exception("Error occured while writing in database");
                return new Result<ResponseModel>(exception);
            }
        }

        public async Task<Result<ResponseModel>> SplitTransactionAsync(string transactionId, TransactionSplitDto model)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction is null)
            {
                var notFoundException = new KeyNotFoundException("Transaction does not exist");
                return new Result<ResponseModel>(notFoundException);
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
                return new Result<ResponseModel>(notFoundException);
            }

            var splits = new List<TransactionSplit>();
            foreach (var split in model.Splits)
            {
                if (!await _categoryValidator.ExistsAsync(split.CatCode))
                {
                    var notFoundException = new KeyNotFoundException("Category does not exist");
                    return new Result<ResponseModel>(notFoundException);
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
                var res = new ResponseModel
                {
                    Message = "Transaction split successfully."
                };
                return res;
            }
            catch
            {
                var exception = new Exception("Error occured while writing in database");
                return new Result<ResponseModel>(exception);
            }
        }
    }
}
