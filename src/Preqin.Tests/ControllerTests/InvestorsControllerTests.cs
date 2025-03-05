using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Preqin.Domain;
using Preqin.WebAPI.Controllers;
using Prequin.Service.Interfaces;
using Xunit;

namespace Preqin.Tests.ControllerTests
{
    public class InvestorsControllerTests
    {
        private readonly Mock<IInvestorService> _mockInvestorService;
        private readonly Mock<ILogger<InvestorsController>> _mockLogger;
        private readonly InvestorsController _controller;

        public InvestorsControllerTests()
        {
            _mockInvestorService = new Mock<IInvestorService>();
            _mockLogger = new Mock<ILogger<InvestorsController>>();
            _controller = new InvestorsController(_mockLogger.Object, _mockInvestorService.Object);
        }

        [Fact]
        public async Task GetInvestors_ShouldReturnOk_WithListOfInvestors()
        {
            // Arrange
            var investors = new List<Investor>
        {
            new Investor { Id = 1, InvestorName = "John Doe", InvestorType = "PE", InvestorCountry = "USA" },
            new Investor { Id = 2, InvestorName = "Jane Smith", InvestorType = "RE", InvestorCountry = "UK" }
        };

            _mockInvestorService.Setup(service => service.GetInvestorsAsync()).ReturnsAsync(investors);

            // Act
            var result = await _controller.GetInvestors();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<Investor>>(okResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async Task GetInvestorDetails_ShouldReturnOk_WhenInvestorExists()
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

            _mockInvestorService.Setup(service => service.GetInvestorDetailsAsync(1, null)).ReturnsAsync(investor);

            // Act
            var result = await _controller.GetInvestorDetails(1, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Investor>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("John Doe", returnValue.InvestorName);
        }

        [Fact]
        public async Task GetInvestorDetails_ShouldReturnNotFound_WhenInvestorDoesNotExist()
        {
            // Arrange
            _mockInvestorService.Setup(service => service.GetInvestorDetailsAsync(99, null)).ReturnsAsync((Investor)null);

            // Act
            var result = await _controller.GetInvestorDetails(99, null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
