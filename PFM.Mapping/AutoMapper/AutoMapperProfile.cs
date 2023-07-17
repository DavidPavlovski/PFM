using AutoMapper;
using PFM.DataAccess.Entities;
using PFM.DataTransfer.Category;
using PFM.DataTransfer.Transaction;
using PFM.Helpers.PageSort;

namespace PFM.Mapping.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Transaction, TransactionResponseDto>();
            CreateMap<Category, CategoryResponseDto>();
            CreateMap<PagedSortedList<Transaction>, PagedSortedList<TransactionResponseDto>>();
        }
    }
}
