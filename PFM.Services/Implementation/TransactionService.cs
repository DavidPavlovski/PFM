using AutoMapper;
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

        public async Task<List<TransactionResponseDto>> ImportFromCSVAsync(IFormFile file)
        {
            var transactions = CSVParser.ParseCSV<Transaction, TransactionCSVMap>(file);
            _transactionRepository.InsertTransactions(transactions);
            try
            {
                await _unitOfWork.SaveChangesAsync();
                return _mapper.Map<List<TransactionResponseDto>>(transactions.Take(10).ToList());
            }
            catch
            {
                return null;
            }
        }
        public async Task<TransactionResponseDto> CategorizeTransaction(string transactionId, TransactionCategorizeDto model)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction == null)
            {
                return null;
            }
            if (!await _categoryValidator.ExistsAsync(model.CatCode))
            {
                return null;
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
                return null;
            }
        }

        public async Task<TransactionResponseDto> SplitTransactionAsync(string transactionId, TransactionSplitDto model)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction == null)
            {
                return null;
            }
            transaction.CatCode = "Z";
            _transactionRepository.Update(transaction);
            if (transaction.TransactionSplits.Any())
            {
                _transactionSplitRepository.DeleteRange(transaction.TransactionSplits);
            }
            if (!_splitValidator.ValidateAmmount(transaction.Ammount, model.Splits.Sum(x => x.Ammount)))
            {
                return null;
            }
            var splits = new List<TransactionSplit>();
            foreach (var split in model.Splits)
            {
                if (!await _categoryValidator.ExistsAsync(split.CatCode))
                {
                    return null;
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
                return null;
            }
        }
    }
}
