using System.ComponentModel.DataAnnotations;

namespace Thunders.TechTest.ApiService.Models.Reports
{
	public class MonthlyPlazaReport
	{
		[Key]
		public Guid Id { get; set; }
		public Guid ReportId { get; set; }
		public string Plaza { get; set; } = string.Empty;
		public string City { get; set; } = string.Empty;
		public string State { get; set; } = string.Empty;
		public int Year { get; set; }
		public int Month { get; set; }
		public decimal TotalAmount { get; set; }
		public int Rank { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
