using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Preqin.Domain;
using Preqin.Infrastructure;
using Prequin.Service;
using Xunit;

namespace Preqin.Tests.Service
{
    public class InvestorServiceTests
    {
        private readonly Mock<IInvestorRepository> _mockInvestorRepository;
        private readonly InvestorService _investorService;

        public InvestorServiceTests()
        {
            _mockInvestorRepository = new Mock<IInvestorRepository>();
            _investorService = new InvestorService(_mockInvestorRepository.Object);
        }

        [Fact]
        public async Task GetInvestorsAsync_ShouldReturnListOfInvestors()
        {
            // Arrange
            var investors = new List<Investor>
        {
            new Investor { Id = 1, InvestorName = "John Doe", InvestorType = "PE", InvestorCountry = "USA" },
            new Investor { Id = 2, InvestorName = "Jane Smith", InvestorType = "RE", InvestorCountry = "UK" }
        };

            _mockInvestorRepository
                .Setup(repo => repo.GetInvestorsAsync())
                .ReturnsAsync(investors);

            // Act
            var result = await _investorService.GetInvestorsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("John Doe", result.First().InvestorName);
        }

        [Fact]
        public async Task GetInvestorDetailsAsync_ShouldReturnInvestor_WhenIdExists()
        {
            // Arrange
            var investor = new Investor
            {
                Id = 1,
                InvestorName = "John Doe",
                InvestorType = "PE",
                InvestorCountry = "USA",
                Commitments = new List<Commitment>
            {
                new Commitment { Id = 1, InvestorId = 1, AssetClass = "Private Equity", Amount = 100000, Currency = "USD" }
            }
            };

            _mockInvestorRepository
                .Setup(repo => repo.GetInvestorDetailsAsync(1, null))
                .ReturnsAsync(investor);

            // Act
            var result = await _investorService.GetInvestorDetailsAsync(1, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("John Doe", result.InvestorName);
            Assert.Single(result.Commitments);
        }

        [Fact]
        public async Task GetInvestorDetailsAsync_ShouldReturnNull_WhenInvestorDoesNotExist()
        {
            // Arrange
            _mockInvestorRepository
                .Setup(repo => repo.GetInvestorDetailsAsync(99, null))
                .ReturnsAsync((Investor)null);

            // Act
            var result = await _investorService.GetInvestorDetailsAsync(99, null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetInvestorDetailsAsync_ShouldFilterCommitments_ByAssetClass()
        {
            // Arrange
            var investor = new Investor
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
            };

            _mockInvestorRepository
                .Setup(repo => repo.GetInvestorDetailsAsync(1, "Private Equity"))
                .ReturnsAsync(new Investor
                {
                    Id = investor.Id,
                    InvestorName = investor.InvestorName,
                    InvestorType = investor.InvestorType,
                    InvestorCountry = investor.InvestorCountry,
                    Commitments = investor.Commitments.FindAll(c => c.AssetClass == "Private Equity")
                });

            // Act
            var result = await _investorService.GetInvestorDetailsAsync(1, "Private Equity");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Commitments);
            Assert.Equal("Private Equity", result.Commitments.First().AssetClass);
        }
    }
}
