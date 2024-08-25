namespace YaqraApi.Services.IServices
{
    public interface IBookProxyService
    {
        Task<decimal?> CalculateRate(int bookId);
        Task UpdateRate(int bookId, decimal newRate);
    }
}
