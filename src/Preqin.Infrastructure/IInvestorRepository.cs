using Preqin.Domain;

namespace Preqin.Infrastructure
{
    public interface IInvestorRepository
    {
        Task<List<Investor>> GetInvestorsAsync();
        Task<Investor> GetInvestorDetailsAsync(int id, string assetClass);
    }
}
