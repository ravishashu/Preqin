using Preqin.Domain;
using Preqin.Infrastructure;
using Prequin.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prequin.Service
{
    public class InvestorService : IInvestorService
    {
        private readonly IInvestorRepository _investorRepository;

        public InvestorService(IInvestorRepository investorRepository)
        {
            _investorRepository = investorRepository;
        }

        public async Task<IEnumerable<Investor>> GetInvestorsAsync()
        {
            return await _investorRepository.GetInvestorsAsync();
        }

        public async Task<Investor> GetInvestorDetailsAsync(int id, string assetClass)
        {
            return await _investorRepository.GetInvestorDetailsAsync(id, assetClass);
        }
    }
}
