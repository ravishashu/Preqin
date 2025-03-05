using Preqin.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Preqin.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Seed(PreqinDbContext context)
        {
            if (!context.Investors.Any())
            {
                var investors = new List<Investor>();

               
                string filePath = @"C://temp/data.csv";
                var lines = File.ReadAllLines(filePath);
                List<Commitment> commitment = new List<Commitment>();
                int id = 1;

                foreach (var line in lines.Skip(1)) // Skip header
                {
                    var values = line.Split(',');
                    string investorName = values[0];
                    string investorType = values[1];
                    string InvestorCountry = values[2];
                    
                    var investor = investors.FirstOrDefault(r => r.InvestorType == investorType
                    && r.InvestorName == investorName && r.InvestorCountry == InvestorCountry);
                    if (investor == null)
                    {
                        investor = new Investor
                        {
                            Id = id++,
                            InvestorName = values[0],
                            InvestorType = values[1],
                            InvestorCountry = values[2],
                            InvestorDateAdded = DateTime.ParseExact(values[3], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                            InvestorLastUpdated = DateTime.ParseExact(values[4], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                            //Commitments = commitment.Where(r => r.InvestorName == investorName && r.AssetClass == assetClass && r.Currency == currency).ToList()
                        };

                        investors.Add(investor);
                    }
                }
                id = 1;
                foreach (var line in lines.Skip(1)) // Skip header
                {
                    var values = line.Split(',');
                    string investorName = values[0];
                    string investorType = values[1];
                    string InvestorCountry = values[2];
                    string assetClass = values[5];
                    decimal amount = decimal.Parse(values[6]);
                    string currency = values[7];

                    var investor = investors.FirstOrDefault(r => r.InvestorName == values[0] && r.InvestorType == values[1] && r.InvestorCountry==values[2]);
                    if (investor != null)
                    {
                        commitment.Add(new Commitment()
                        {
                            Id = id++,
                            InvestorId = investor.Id,
                            AssetClass = assetClass,
                            Amount = amount,
                            Currency = currency,
                            Investor = investor,
                        });
                    }
                }

                foreach(var investor in investors)
                {
                    investor.Commitments = commitment.Where(r => r.InvestorId == investor.Id).ToList();    
                }

                context.Investors.AddRange(investors);
                context.Commitments.AddRange(commitment);
                context.SaveChanges();
            }
        }
    }

}

