using Utis.Tasks.Domain.Entities;

namespace Utis.Tasks.WebApi.Models
{
	public static class CurrencyItemResponseExtensions
	{
		public static CurrencyRate ToCurrencyRate(this CurrencyItemResponse currency)
		{
			if (currency == null) return null;

			var change = currency.Value - currency.Previous;
			var changeDirection = change switch
			{
				> 0 => "up",
				< 0 => "down",
				_ => "unchanged"
			};

			return new CurrencyRate
			{
				CharCode = currency.CharCode,
				Name = currency.Name,
				Value = currency.Value,
				Previous = currency.Previous,
				Change = change,
				ChangeDirection = changeDirection
			};
		}
	}
}
