using Preqin.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prequin.Service.Interfaces
{
    public interface IInvestorService
    {
        Task<IEnumerable<Investor>> GetInvestorsAsync();
        Task<Investor> GetInvestorDetailsAsync(int id, string assetClass);
    }
}
