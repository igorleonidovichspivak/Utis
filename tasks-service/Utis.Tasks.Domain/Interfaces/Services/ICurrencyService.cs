using Utis.Tasks.Domain.Models;

namespace Utis.Tasks.Domain.Interfaces.Services
{
	public interface ICurrencyService
	{
		Task<List<CurrencyRate>> GetAllRatesAsync();
	}
}
