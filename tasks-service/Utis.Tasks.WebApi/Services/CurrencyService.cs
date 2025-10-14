using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Text.Json.Serialization;
using Utis.Tasks.Domain.Entities;
using Utis.Tasks.Domain.Interfaces.Services;
using Utis.Tasks.WebApi.Models;

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
				json = @"{
    ""Date"": ""2025-10-14T11:30:00+03:00"",
    ""PreviousDate"": ""2025-10-11T11:30:00+03:00"",
    ""PreviousURL"": ""\/\/www.cbr-xml-daily.ru\/archive\/2025\/10\/11\/daily_json.js"",
    ""Timestamp"": ""2025-10-13T20:00:00+03:00"",
    ""Valute"": {
        ""AUD"": {
            ""ID"": ""R01010"",
            ""NumCode"": ""036"",
            ""CharCode"": ""AUD"",
            ""Nominal"": 1,
            ""Name"": ""Австралийский доллар"",
            ""Value"": 52.7497,
            ""Previous"": 53.3255
        },
        ""AZN"": {
            ""ID"": ""R01020A"",
            ""NumCode"": ""944"",
            ""CharCode"": ""AZN"",
            ""Nominal"": 1,
            ""Name"": ""Азербайджанский манат"",
            ""Value"": 47.5616,
            ""Previous"": 47.7587
        },
        ""DZD"": {
            ""ID"": ""R01030"",
            ""NumCode"": ""012"",
            ""CharCode"": ""DZD"",
            ""Nominal"": 100,
            ""Name"": ""Алжирских динаров"",
            ""Value"": 62.1257,
            ""Previous"": 62.322
        },
        ""GBP"": {
            ""ID"": ""R01035"",
            ""NumCode"": ""826"",
            ""CharCode"": ""GBP"",
            ""Nominal"": 1,
            ""Name"": ""Фунт стерлингов"",
            ""Value"": 107.5531,
            ""Previous"": 108.2504
        },
        ""AMD"": {
            ""ID"": ""R01060"",
            ""NumCode"": ""051"",
            ""CharCode"": ""AMD"",
            ""Nominal"": 100,
            ""Name"": ""Армянских драмов"",
            ""Value"": 21.1346,
            ""Previous"": 21.2161
        },
        ""BHD"": {
            ""ID"": ""R01080"",
            ""NumCode"": ""048"",
            ""CharCode"": ""BHD"",
            ""Nominal"": 1,
            ""Name"": ""Бахрейнский динар"",
            ""Value"": 214.9929,
            ""Previous"": 215.8837
        },
        ""BYN"": {
            ""ID"": ""R01090B"",
            ""NumCode"": ""933"",
            ""CharCode"": ""BYN"",
            ""Nominal"": 1,
            ""Name"": ""Белорусский рубль"",
            ""Value"": 27.0698,
            ""Previous"": 27.1003
        },
        ""BGN"": {
            ""ID"": ""R01100"",
            ""NumCode"": ""975"",
            ""CharCode"": ""BGN"",
            ""Nominal"": 1,
            ""Name"": ""Болгарский лев"",
            ""Value"": 47.8227,
            ""Previous"": 48.1993
        },
        ""BOB"": {
            ""ID"": ""R01105"",
            ""NumCode"": ""068"",
            ""CharCode"": ""BOB"",
            ""Nominal"": 1,
            ""Name"": ""Боливиано"",
            ""Value"": 11.7011,
            ""Previous"": 11.7496
        },
        ""BRL"": {
            ""ID"": ""R01115"",
            ""NumCode"": ""986"",
            ""CharCode"": ""BRL"",
            ""Nominal"": 1,
            ""Name"": ""Бразильский реал"",
            ""Value"": 14.8513,
            ""Previous"": 15.1657
        },
        ""HUF"": {
            ""ID"": ""R01135"",
            ""NumCode"": ""348"",
            ""CharCode"": ""HUF"",
            ""Nominal"": 100,
            ""Name"": ""Форинтов"",
            ""Value"": 23.8947,
            ""Previous"": 24.0812
        },
        ""VND"": {
            ""ID"": ""R01150"",
            ""NumCode"": ""704"",
            ""CharCode"": ""VND"",
            ""Nominal"": 10000,
            ""Name"": ""Донгов"",
            ""Value"": 32.1977,
            ""Previous"": 32.3105
        },
        ""HKD"": {
            ""ID"": ""R01200"",
            ""NumCode"": ""344"",
            ""CharCode"": ""HKD"",
            ""Nominal"": 1,
            ""Name"": ""Гонконгский доллар"",
            ""Value"": 10.4154,
            ""Previous"": 10.4518
        },
        ""GEL"": {
            ""ID"": ""R01210"",
            ""NumCode"": ""981"",
            ""CharCode"": ""GEL"",
            ""Nominal"": 1,
            ""Name"": ""Лари"",
            ""Value"": 29.817,
            ""Previous"": 29.9317
        },
        ""DKK"": {
            ""ID"": ""R01215"",
            ""NumCode"": ""208"",
            ""CharCode"": ""DKK"",
            ""Nominal"": 1,
            ""Name"": ""Датская крона"",
            ""Value"": 12.5259,
            ""Previous"": 12.6251
        },
        ""AED"": {
            ""ID"": ""R01230"",
            ""NumCode"": ""784"",
            ""CharCode"": ""AED"",
            ""Nominal"": 1,
            ""Name"": ""Дирхам ОАЭ"",
            ""Value"": 22.0163,
            ""Previous"": 22.1075
        },
        ""USD"": {
            ""ID"": ""R01235"",
            ""NumCode"": ""840"",
            ""CharCode"": ""USD"",
            ""Nominal"": 1,
            ""Name"": ""Доллар США"",
            ""Value"": 80.8548,
            ""Previous"": 81.1898
        },
        ""EUR"": {
            ""ID"": ""R01239"",
            ""NumCode"": ""978"",
            ""CharCode"": ""EUR"",
            ""Nominal"": 1,
            ""Name"": ""Евро"",
            ""Value"": 93.9181,
            ""Previous"": 94.0491
        },
        ""EGP"": {
            ""ID"": ""R01240"",
            ""NumCode"": ""818"",
            ""CharCode"": ""EGP"",
            ""Nominal"": 10,
            ""Name"": ""Египетских фунтов"",
            ""Value"": 16.9492,
            ""Previous"": 17.0731
        },
        ""INR"": {
            ""ID"": ""R01270"",
            ""NumCode"": ""356"",
            ""CharCode"": ""INR"",
            ""Nominal"": 100,
            ""Name"": ""Индийских рупий"",
            ""Value"": 91.0735,
            ""Previous"": 91.5413
        },
        ""IDR"": {
            ""ID"": ""R01280"",
            ""NumCode"": ""360"",
            ""CharCode"": ""IDR"",
            ""Nominal"": 10000,
            ""Name"": ""Рупий"",
            ""Value"": 48.7518,
            ""Previous"": 49.1048
        },
        ""IRR"": {
            ""ID"": ""R01300"",
            ""NumCode"": ""364"",
            ""CharCode"": ""IRR"",
            ""Nominal"": 100000,
            ""Name"": ""Иранских риалов"",
            ""Value"": 13.7247,
            ""Previous"": 14.0584
        },
        ""KZT"": {
            ""ID"": ""R01335"",
            ""NumCode"": ""398"",
            ""CharCode"": ""KZT"",
            ""Nominal"": 100,
            ""Name"": ""Тенге"",
            ""Value"": 14.9992,
            ""Previous"": 14.9924
        },
        ""CAD"": {
            ""ID"": ""R01350"",
            ""NumCode"": ""124"",
            ""CharCode"": ""CAD"",
            ""Nominal"": 1,
            ""Name"": ""Канадский доллар"",
            ""Value"": 57.7534,
            ""Previous"": 57.9927
        },
        ""QAR"": {
            ""ID"": ""R01355"",
            ""NumCode"": ""634"",
            ""CharCode"": ""QAR"",
            ""Nominal"": 1,
            ""Name"": ""Катарский риал"",
            ""Value"": 22.2129,
            ""Previous"": 22.3049
        },
        ""KGS"": {
            ""ID"": ""R01370"",
            ""NumCode"": ""417"",
            ""CharCode"": ""KGS"",
            ""Nominal"": 100,
            ""Name"": ""Сомов"",
            ""Value"": 92.4583,
            ""Previous"": 92.8414
        },
        ""CNY"": {
            ""ID"": ""R01375"",
            ""NumCode"": ""156"",
            ""CharCode"": ""CNY"",
            ""Nominal"": 1,
            ""Name"": ""Юань"",
            ""Value"": 11.2816,
            ""Previous"": 11.3626
        },
        ""CUP"": {
            ""ID"": ""R01395"",
            ""NumCode"": ""192"",
            ""CharCode"": ""CUP"",
            ""Nominal"": 10,
            ""Name"": ""Кубинских песо"",
            ""Value"": 33.6895,
            ""Previous"": 33.8291
        },
        ""MDL"": {
            ""ID"": ""R01500"",
            ""NumCode"": ""498"",
            ""CharCode"": ""MDL"",
            ""Nominal"": 10,
            ""Name"": ""Молдавских леев"",
            ""Value"": 47.4812,
            ""Previous"": 47.855
        },
        ""MNT"": {
            ""ID"": ""R01503"",
            ""NumCode"": ""496"",
            ""CharCode"": ""MNT"",
            ""Nominal"": 1000,
            ""Name"": ""Тугриков"",
            ""Value"": 22.4941,
            ""Previous"": 22.5923
        },
        ""NGN"": {
            ""ID"": ""R01520"",
            ""NumCode"": ""566"",
            ""CharCode"": ""NGN"",
            ""Nominal"": 1000,
            ""Name"": ""Найр"",
            ""Value"": 55.5637,
            ""Previous"": 55.3572
        },
        ""NZD"": {
            ""ID"": ""R01530"",
            ""NumCode"": ""554"",
            ""CharCode"": ""NZD"",
            ""Nominal"": 1,
            ""Name"": ""Новозеландский доллар"",
            ""Value"": 46.3824,
            ""Previous"": 46.7207
        },
        ""NOK"": {
            ""ID"": ""R01535"",
            ""NumCode"": ""578"",
            ""CharCode"": ""NOK"",
            ""Nominal"": 10,
            ""Name"": ""Норвежских крон"",
            ""Value"": 80.0963,
            ""Previous"": 81.099
        },
        ""OMR"": {
            ""ID"": ""R01540"",
            ""NumCode"": ""512"",
            ""CharCode"": ""OMR"",
            ""Nominal"": 1,
            ""Name"": ""Оманский риал"",
            ""Value"": 210.2856,
            ""Previous"": 211.1568
        },
        ""PLN"": {
            ""ID"": ""R01565"",
            ""NumCode"": ""985"",
            ""CharCode"": ""PLN"",
            ""Nominal"": 1,
            ""Name"": ""Злотый"",
            ""Value"": 21.9935,
            ""Previous"": 22.0756
        },
        ""SAR"": {
            ""ID"": ""R01580"",
            ""NumCode"": ""682"",
            ""CharCode"": ""SAR"",
            ""Nominal"": 1,
            ""Name"": ""Саудовский риял"",
            ""Value"": 21.5613,
            ""Previous"": 21.6506
        },
        ""RON"": {
            ""ID"": ""R01585F"",
            ""NumCode"": ""946"",
            ""CharCode"": ""RON"",
            ""Nominal"": 1,
            ""Name"": ""Румынский лей"",
            ""Value"": 18.4159,
            ""Previous"": 18.4598
        },
        ""XDR"": {
            ""ID"": ""R01589"",
            ""NumCode"": ""960"",
            ""CharCode"": ""XDR"",
            ""Nominal"": 1,
            ""Name"": ""СДР (специальные права заимствования)"",
            ""Value"": 109.9876,
            ""Previous"": 110.6544
        },
        ""SGD"": {
            ""ID"": ""R01625"",
            ""NumCode"": ""702"",
            ""CharCode"": ""SGD"",
            ""Nominal"": 1,
            ""Name"": ""Сингапурский доллар"",
            ""Value"": 62.3302,
            ""Previous"": 62.5403
        },
        ""TJS"": {
            ""ID"": ""R01670"",
            ""NumCode"": ""972"",
            ""CharCode"": ""TJS"",
            ""Nominal"": 10,
            ""Name"": ""Сомони"",
            ""Value"": 87.2653,
            ""Previous"": 87.3798
        },
        ""THB"": {
            ""ID"": ""R01675"",
            ""NumCode"": ""764"",
            ""CharCode"": ""THB"",
            ""Nominal"": 10,
            ""Name"": ""Батов"",
            ""Value"": 24.6892,
            ""Previous"": 24.7915
        },
        ""BDT"": {
            ""ID"": ""R01685"",
            ""NumCode"": ""050"",
            ""CharCode"": ""BDT"",
            ""Nominal"": 100,
            ""Name"": ""Так"",
            ""Value"": 66.3657,
            ""Previous"": 66.5903
        },
        ""TRY"": {
            ""ID"": ""R01700J"",
            ""NumCode"": ""949"",
            ""CharCode"": ""TRY"",
            ""Nominal"": 10,
            ""Name"": ""Турецких лир"",
            ""Value"": 19.4007,
            ""Previous"": 19.4795
        },
        ""TMT"": {
            ""ID"": ""R01710A"",
            ""NumCode"": ""934"",
            ""CharCode"": ""TMT"",
            ""Nominal"": 1,
            ""Name"": ""Новый туркменский манат"",
            ""Value"": 23.1014,
            ""Previous"": 23.1971
        },
        ""UZS"": {
            ""ID"": ""R01717"",
            ""NumCode"": ""860"",
            ""CharCode"": ""UZS"",
            ""Nominal"": 10000,
            ""Name"": ""Узбекских сумов"",
            ""Value"": 66.597,
            ""Previous"": 66.9696
        },
        ""UAH"": {
            ""ID"": ""R01720"",
            ""NumCode"": ""980"",
            ""CharCode"": ""UAH"",
            ""Nominal"": 10,
            ""Name"": ""Гривен"",
            ""Value"": 19.4315,
            ""Previous"": 19.5155
        },
        ""CZK"": {
            ""ID"": ""R01760"",
            ""NumCode"": ""203"",
            ""CharCode"": ""CZK"",
            ""Nominal"": 10,
            ""Name"": ""Чешских крон"",
            ""Value"": 38.4181,
            ""Previous"": 38.6637
        },
        ""SEK"": {
            ""ID"": ""R01770"",
            ""NumCode"": ""752"",
            ""CharCode"": ""SEK"",
            ""Nominal"": 10,
            ""Name"": ""Шведских крон"",
            ""Value"": 84.968,
            ""Previous"": 85.8009
        },
        ""CHF"": {
            ""ID"": ""R01775"",
            ""NumCode"": ""756"",
            ""CharCode"": ""CHF"",
            ""Nominal"": 1,
            ""Name"": ""Швейцарский франк"",
            ""Value"": 100.5532,
            ""Previous"": 100.8318
        },
        ""ETB"": {
            ""ID"": ""R01800"",
            ""NumCode"": ""230"",
            ""CharCode"": ""ETB"",
            ""Nominal"": 100,
            ""Name"": ""Эфиопских быров"",
            ""Value"": 55.2214,
            ""Previous"": 55.5123
        },
        ""RSD"": {
            ""ID"": ""R01805F"",
            ""NumCode"": ""941"",
            ""CharCode"": ""RSD"",
            ""Nominal"": 100,
            ""Name"": ""Сербских динаров"",
            ""Value"": 80.2161,
            ""Previous"": 80.2071
        },
        ""ZAR"": {
            ""ID"": ""R01810"",
            ""NumCode"": ""710"",
            ""CharCode"": ""ZAR"",
            ""Nominal"": 10,
            ""Name"": ""Рэндов"",
            ""Value"": 46.6374,
            ""Previous"": 47.1362
        },
        ""KRW"": {
            ""ID"": ""R01815"",
            ""NumCode"": ""410"",
            ""CharCode"": ""KRW"",
            ""Nominal"": 1000,
            ""Name"": ""Вон"",
            ""Value"": 56.92,
            ""Previous"": 57.9141
        },
        ""JPY"": {
            ""ID"": ""R01820"",
            ""NumCode"": ""392"",
            ""CharCode"": ""JPY"",
            ""Nominal"": 100,
            ""Name"": ""Иен"",
            ""Value"": 52.8428,
            ""Previous"": 53.0618
        },
        ""MMK"": {
            ""ID"": ""R02005"",
            ""NumCode"": ""104"",
            ""CharCode"": ""MMK"",
            ""Nominal"": 1000,
            ""Name"": ""Кьятов"",
            ""Value"": 38.5023,
            ""Previous"": 38.6618
        }
    }
}";

				var currencyResponse = JsonSerializer.Deserialize<CurrencyRatesResponse>(json,
					new JsonSerializerOptions
					{
						PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
						NumberHandling = JsonNumberHandling.AllowReadingFromString

					});

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

		public async Task<CurrencyRate> GetCurrencyRateAsync(string charCode)
		{
			if (string.IsNullOrWhiteSpace(charCode))
				throw new ArgumentException("Currency code cannot be empty", nameof(charCode));

			var rates = await GetCurrentRatesAsync();
			var upperCharCode = charCode.ToUpperInvariant();

			if (rates.Valute.TryGetValue(upperCharCode, out var currency))
			{
				return currency.ToCurrencyRate();
			}

			_logger.LogWarning("Currency with code {CharCode} not found", charCode);
			throw new ArgumentException($"Currency with code {charCode} not found");
		}

		public async Task<List<CurrencyRate>> GetAllRatesAsync()
		{
			var rates = await GetCurrentRatesAsync();
			return rates.Valute.Values.Select(s => s.ToCurrencyRate()).ToList();
		}
	}
}
