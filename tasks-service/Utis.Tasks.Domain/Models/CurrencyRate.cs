namespace Utis.Tasks.Domain.Models
{
	public class CurrencyRate
	{
		public string CharCode { get; set; }
		public string Name { get; set; }
		public decimal Value { get; set; }
		public decimal Previous { get; set; }
		public decimal Change { get; set; }
		public string ChangeDirection { get; set; }
	}
}
