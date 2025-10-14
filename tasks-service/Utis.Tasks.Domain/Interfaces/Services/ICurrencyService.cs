using Utis.Tasks.Domain.Entities;

namespace Utis.Tasks.Domain.Interfaces.Services
{
	public interface ICurrencyService
	{
		Task<List<CurrencyRate>> GetAllRatesAsync();
	}
}
