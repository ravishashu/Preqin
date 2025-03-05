using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Preqin.Domain;
using Preqin.Infrastructure.Data;
using Preqin.Infrastructure;
using Xunit;

namespace Preqin.Tests.Intrastructure
{
    public class InvestorRepositoryTests
    {
        private PreqinDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<PreqinDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
                .Options;
            return new PreqinDbContext(options);
        }

        [Fact]
        public async Task GetInvestorsAsync_ShouldReturnInvestors_WithCommitments()
        {
            // Arrange
            var dbContext = GetDbContext();
            dbContext.Investors.AddRange(new List<Investor>
        {
            new Investor { Id = 1, InvestorName = "John Doe", InvestorType = "PE", InvestorCountry = "USA",
                Commitments = new List<Commitment>
                {
                    new Commitment { Id = 1, InvestorId = 1, AssetClass = "Private Equity", Amount = 100000, Currency = "USD" }
                }
            },
            new Investor { Id = 2, InvestorName = "Jane Smith", InvestorType = "RE", InvestorCountry = "UK",
                Commitments = new List<Commitment>
                {
                    new Commitment { Id = 2, InvestorId = 2, AssetClass = "Real Estate", Amount = 200000, Currency = "GBP" }
                }
            }
        });
            await dbContext.SaveChangesAsync();

            var repository = new InvestorRepository(dbContext);

            // Act
            var result = await repository.GetInvestorsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, investor => Assert.NotEmpty(investor.Commitments));
        }

        [Fact]
        public async Task GetInvestorDetailsAsync_ShouldReturnInvestor_WhenIdExists()
        {
            // Arrange
            var dbContext = GetDbContext();
            dbContext.Investors.Add(new Investor
            {
                Id = 1,
                InvestorName = "John Doe",
                InvestorType = "PE",
                InvestorCountry = "USA",
                Commitments = new List<Commitment>
            {
                new Commitment { Id = 1, InvestorId = 1, AssetClass = "Private Equity", Amount = 100000, Currency = "USD" }
            }
            });
            await dbContext.SaveChangesAsync();

            var repository = new InvestorRepository(dbContext);

            // Act
            var investor = await repository.GetInvestorDetailsAsync(1, null);

            // Assert
            Assert.NotNull(investor);
            Assert.Equal(1, investor.Id);
            Assert.Equal("John Doe", investor.InvestorName);
            Assert.Single(investor.Commitments);
        }

        [Fact]
        public async Task GetInvestorDetailsAsync_ShouldReturnNull_WhenInvestorDoesNotExist()
        {
            // Arrange
            var dbContext = GetDbContext();
            var repository = new InvestorRepository(dbContext);

            // Act
            var investor = await repository.GetInvestorDetailsAsync(99, null);

            // Assert
            Assert.Null(investor);
        }

        [Fact]
        public async Task GetInvestorDetailsAsync_ShouldFilterCommitments_ByAssetClass()
        {
            // Arrange
            var dbContext = GetDbContext();
            dbContext.Investors.Add(new Investor
            {
                Id = 1,
                InvestorName = "John Doe",
                InvestorType = "PE",
                InvestorCountry = "USA",
                Commitments = new List<Commitment>
            {
                new Commitment { Id = 1, InvestorId = 1, AssetClass = "Private Equity", Amount = 100000, Currency = "USD" },
                new Commitment { Id = 2, InvestorId = 1, AssetClass = "Real Estate", Amount = 200000, Currency = "GBP" }
            }
            });
            await dbContext.SaveChangesAsync();

            var repository = new InvestorRepository(dbContext);

            // Act
            var investor = await repository.GetInvestorDetailsAsync(1, "Private Equity");

            // Assert
            Assert.NotNull(investor);
            Assert.Single(investor.Commitments);
            Assert.Equal("Private Equity", investor.Commitments.First().AssetClass);
        }
    }
}
