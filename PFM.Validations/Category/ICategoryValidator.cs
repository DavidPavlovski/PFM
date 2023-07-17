namespace PFM.Validations.Category
{
    public interface ICategoryValidator
    {
        Task<bool> ExistsAsync(string categoryCode);
    }
}
