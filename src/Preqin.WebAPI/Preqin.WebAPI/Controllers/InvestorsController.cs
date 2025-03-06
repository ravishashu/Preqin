using Microsoft.AspNetCore.Mvc;
using Preqin.WebAPI.Filters;
using Prequin.Service.Interfaces;

namespace Preqin.WebAPI.Controllers
{
    [ApiController]
    [ServiceFilter(typeof(LoggingActionFilter))]
    [ServiceFilter(typeof(ExceptionHandlingFilter))]
    [Route("api/investors")]
    public class InvestorsController : ControllerBase
    {
        private readonly ILogger<InvestorsController> _logger;
        private readonly IInvestorService _investorService;

        public InvestorsController(ILogger<InvestorsController> logger, IInvestorService investorService)
        {
            _logger = logger;
            _investorService = investorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetInvestors()
        {
            var investors = await _investorService.GetInvestorsAsync();
            return Ok(investors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvestorDetails(int id, [FromQuery] string assetClass)
        {
            var investorDetails = await _investorService.GetInvestorDetailsAsync(id, assetClass);
            if (investorDetails == null) return NotFound();

            return Ok(investorDetails);
        }

    }
    
}
