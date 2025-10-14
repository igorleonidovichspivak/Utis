using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Utis.Tasks.Domain.Entities;
using Utis.Tasks.Domain.Interfaces.Services;

namespace Utis.Tasks.WebApi.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class CurrencyController : ControllerBase
	{
		private readonly ILogger<CurrencyController> _logger;
		private readonly ICurrencyService _currencyService;


		public CurrencyController(ILogger<CurrencyController> logger, ICurrencyService currencyService)
		{
			_logger = logger;
			_currencyService = currencyService;
		}


		[HttpGet]
		[ProducesResponseType(typeof(List<CurrencyRate>), 200)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> GetAllRates()
		{
			try
			{
				_logger.LogInformation("Getting all currency rates");
				var rates = await _currencyService.GetAllRatesAsync();
				return Ok(rates);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting all currency rates");
				return StatusCode(500, "Internal server error");
			}
		}

	}
}
