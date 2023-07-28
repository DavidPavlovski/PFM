using AutoMapper;
using LanguageExt.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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
using System.Net;
using System.Web;

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
        readonly ICategoryRepository _categoryRepository;
        readonly IConfiguration _configuration;

        const int DEFAULT_BATCH_SIZE = 1000;
        const int DEFAULT_PAGE_SIZE = 10;
        public TransactionService(ITransactionRepository transactionRepository, IUnitOfWork unitOfWork, IMapper mapper, ICategoryValidator categoryValidator, ISplitValidator splitValidator, ITransactionSplitRepository transactionSplitRepository, ICSVParser csvParser, ICategoryRepository categoryRepository, IConfiguration configuration)
        {
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _categoryValidator = categoryValidator;
            _splitValidator = splitValidator;
            _transactionSplitRepository = transactionSplitRepository;
            _csvParser = csvParser;
            _categoryRepository = categoryRepository;
            _configuration = configuration;
        }

        public async Task<PagedSortedList<TransactionResponseDto>> GetTransactionsAsync(PagerSorter pagerSorter)
        {
            var itemsForPagination = _configuration.GetValue<int?>("variables:MaxItemsForPagination") ?? DEFAULT_PAGE_SIZE;
            if (pagerSorter.PageSize <= 0)
            {
                pagerSorter.PageSize = itemsForPagination;
            }
            pagerSorter.PageSize = Math.Min(pagerSorter.PageSize, itemsForPagination);
            if (pagerSorter.Page < 1)
            {
                pagerSorter.Page = 1;
            }
            if (string.IsNullOrEmpty(pagerSorter.SortBy))
            {
                pagerSorter.SortBy = "date";
            }
            var res = await _transactionRepository.GetTransactionsAsync(pagerSorter);
            return _mapper.Map<PagedSortedList<TransactionResponseDto>>(res);
        }

        public async Task<Result<ResponseModel>> ImportFromCSVAsync(IFormFile file)
        {
            try
            {
                var csvResponse = _csvParser.ParseCSV<Transaction, TransactionCSVMap>(file);
                if (csvResponse is null)
                {
                    var error = new CustomFileLoadException($"Error occured while reading file: '{file.FileName}', make sure the file you uploaded is a valid CSV file and you have all the required headers.")
                    {
                        CsvHeaders = "id, beneficiary-name, date, direction, amount, description, currency, mcc, kind"
                    };
                    return new Result<ResponseModel>(error);
                }
                if (csvResponse.Errors.Any())
                {
                    var exception = new CustomException("One or more validation errors occured while reading file")
                    {
                        Description = "Problem occured while parsing CSV values , see errors below.",
                        StatusCode = HttpStatusCode.BadRequest,
                        Errors = csvResponse.Errors
                    };
                    return new Result<ResponseModel>(exception);
                }
                var batchSize = _configuration.GetValue<int?>("variables:BatchSize") ?? DEFAULT_BATCH_SIZE;

                await _transactionRepository.ImportTransactions(csvResponse.Items, batchSize);

                var res = new ResponseModel
                {
                    Message = "Transactions imported successfully."
                };
                return res;
            }
            catch (Exception ex)
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
            if (model.Splits.Count <= 1)
            {
                var exception = new ArgumentException("Transaction split count must be greater than 1");
                return new Result<ResponseModel>(exception);
            }
            if (model.Splits.Any(x => x.Ammount <= 0))
            {
                var exception = new ArgumentException("Transaction split ammount cannot be 0 or lower.");
                return new Result<ResponseModel>(exception);
            }
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
                var notFoundException = new ArgumentException($"The transaction split ammounts do not correspond with the total ammount of the transaction : {transaction.Ammount}");
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

        public async Task<Result<ResponseModel>> AutoCategorize()
        {
            try
            {
                var transactions = _transactionRepository.GetAll();
                var catCodes = await _categoryRepository.GetCatCodesAsync();
                if (catCodes.Count == 0)
                {
                    var err = new ResponseModel
                    {
                        Message = "No categories were found. Cannot continue with operation."
                    };
                    return err;
                }
                List<Rule> rules = new();
                using (StreamReader r = new("rules.json"))
                {
                    string json = r.ReadToEnd();
                    rules = JsonConvert.DeserializeObject<List<Rule>>(json).ToList();
                }

                foreach (var transaction in transactions)
                {
                    if (!string.IsNullOrEmpty(transaction.CatCode))
                    {
                        continue;
                    }
                    foreach (var rule in rules)
                    {
                        if (!catCodes.Contains(rule.CatCode))
                        {
                            continue;
                        }
                        if (rule.Mcc.Any(x => x == transaction.Mcc))
                        {
                            transaction.CatCode = rule.CatCode;
                        }
                        else if (rule.Keywords.Any(x => transaction.BeneficiaryName.ToLower().Contains(x) || transaction.Description.ToLower().Contains(x)))
                        {
                            transaction.CatCode = rule.CatCode;
                        }
                    }
                }
                await _transactionRepository.UpdateRange(transactions.ToList());

                var res = new ResponseModel
                {
                    Message = "Transactions categorized successfully."
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
