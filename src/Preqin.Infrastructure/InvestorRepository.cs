using Microsoft.EntityFrameworkCore;
using Preqin.Domain;
using Preqin.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Preqin.Infrastructure
{
    public class InvestorRepository : IInvestorRepository
    {
        PreqinDbContext preqinDbContext;

        public InvestorRepository(PreqinDbContext dbContext) 
        { 
            preqinDbContext = dbContext;
        }

        private List<Investor> _investors = new List<Investor>();
        
        public async Task<List<Investor>> GetInvestorsAsync()
        {
            _investors = await preqinDbContext.Investors.Include(i => i.Commitments).ToListAsync();
            return _investors;
        }

        public async Task<Investor?> GetInvestorDetailsAsync(int id, string assetClass)
        {
            var investor = await preqinDbContext.Investors
                .Include(i => i.Commitments) // Ensure Commitments are loaded if needed
                .FirstOrDefaultAsync(i => i.Id == id);

            if (investor == null) return null;

            if (!string.IsNullOrEmpty(assetClass) && assetClass != "ALL")
            {
                investor.Commitments = investor.Commitments
                    .Where(c => c.AssetClass == assetClass)
                    .ToList();
            }

            return investor;
        }

    }
}
