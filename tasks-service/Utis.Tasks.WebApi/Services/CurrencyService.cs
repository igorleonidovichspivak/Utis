using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Text.Json.Serialization;
using Utis.Tasks.Domain.Interfaces.Services;
using Utis.Tasks.Domain.Models;
using Utis.Tasks.WebApi.Models;
using Utis.Tasks.WebApi.Models.Converters;

namespace Utis.Tasks.WebApi.Services
{
	public class CurrencyService : ICurrencyService
	{
		private readonly HttpClient _httpClient;
		private readonly ILogger<CurrencyService> _logger;
		private readonly IMemoryCache _cache;
		private readonly MemoryCacheEntryOptions _cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
		private const string CbrApiUrl = "https://www.cbr-xml-daily.ru/daily_json.js";
		private const string ResponseRatesCacheKey = "cached_rates";
		private static JsonSerializerOptions JsonSerializerOptions => new()
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			NumberHandling = JsonNumberHandling.AllowReadingFromString
		};

		public CurrencyService(HttpClient httpClient, ILogger<CurrencyService> logger, IMemoryCache cache)
		{
			_httpClient = httpClient;
			_logger = logger;
			_cache = cache;

		}



		public async Task<CurrencyRatesResponse> GetCurrentRatesAsync()
		{
			if (_cache.TryGetValue(ResponseRatesCacheKey, out CurrencyRatesResponse cachedResponse))
			{
				_logger.LogInformation("Get result from cache");
				return cachedResponse;
			}

			try
			{
				_logger.LogInformation("Fetching currency rates from CBR");

				var response = await _httpClient.GetAsync(CbrApiUrl);
				response.EnsureSuccessStatusCode();

				var json = await response.Content.ReadAsStringAsync();

				var currencyResponse = JsonSerializer.Deserialize<CurrencyRatesResponse>(json, JsonSerializerOptions);

				_logger.LogInformation("Successfully fetched currency rates for {Date}", currencyResponse?.Date);

				_cache.Set(ResponseRatesCacheKey, currencyResponse, _cacheOptions);

				return currencyResponse;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching currency rates from CBR");
				throw;
			}
		}

	
		public async Task<List<CurrencyRate>> GetAllRatesAsync()
		{
			var rates = await GetCurrentRatesAsync();
			return rates.Valute.Values.Select(s => s.ToCurrencyRate()).ToList();
		}
	}
}
