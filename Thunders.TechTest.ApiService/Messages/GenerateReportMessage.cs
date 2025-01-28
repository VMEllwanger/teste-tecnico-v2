using System.Diagnostics.CodeAnalysis;

namespace Thunders.TechTest.ApiService.Messages
{
	[ExcludeFromCodeCoverage]
	public class GenerateReportMessage
	{
		public int Year { get; }
		public int Month { get; }

		public GenerateReportMessage(int year, int month)
		{
			Year = year;
			Month = month;
		}
	}
}
