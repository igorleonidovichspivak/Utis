namespace Utis.Tasks.Domain.Models
{
	/// <summary>
	/// Статусы задач
	/// </summary>
	public enum TaskState
	{
		/// <summary>
		/// Новая
		/// </summary>
		New = 0,
		/// <summary>
		/// В работе
		/// </summary>
		InProgress,
		/// <summary>
		/// Завершена
		/// </summary>
		Completed,
		/// <summary>
		/// Просрочена
		/// </summary>
		Overdue
	}
}
