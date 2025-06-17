using System.ComponentModel.DataAnnotations;

namespace Thunders.TechTest.ApiService.Models.Reports
{
	public class VehicleTypeReport
	{
		[Key]
		public Guid Id { get; set; }
		public Guid ReportId { get; set; }
		public string Plaza { get; set; } = string.Empty;
		public string City { get; set; } = string.Empty;
		public string State { get; set; } = string.Empty;
		public DateTime Date { get; set; }
		public VehicleType VehicleType { get; set; }
		public int VehicleCount { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
