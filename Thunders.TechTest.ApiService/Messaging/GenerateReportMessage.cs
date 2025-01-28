using System.Diagnostics.CodeAnalysis;
using Thunders.TechTest.ApiService.Models.Reports;

namespace Thunders.TechTest.ApiService.Messaging;
[ExcludeFromCodeCoverage]
public class GenerateReportMessage
{
	public Guid ReportId { get; }
	public DateTime Date { get; }
	public ReportType ReportType { get; }

	public GenerateReportMessage(Guid reportId, DateTime date, ReportType reportType)
	{
		ReportId = reportId;
		Date = date;
		ReportType = reportType;
	}
}
