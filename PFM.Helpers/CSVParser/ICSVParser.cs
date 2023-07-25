using Microsoft.AspNetCore.Http;

namespace PFM.Helpers.CSVParser
{
    public interface ICSVParser
    {
        CSVReponse<TEntity> ParseCSV<TEntity, TMap>(IFormFile file) where TMap : CustomClassMap<TEntity>;
    }
}