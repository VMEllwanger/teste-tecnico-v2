using System.ComponentModel.DataAnnotations;

namespace Thunders.TechTest.ApiService.Models.Reports;

public class HourlyCityReport
{
	[Key]
	public Guid Id { get; set; }
	public Guid ReportId { get; set; }
	public int Hour { get; set; }
	public string City { get; set; } = string.Empty;
	public string State { get; set; } = string.Empty;
	public DateTime Date { get; set; }
	public decimal TotalAmount { get; set; }
	public int TotalVehicles { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
