using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Thunders.TechTest.ApiService.Logging;
[ExcludeFromCodeCoverage]
public static class ActivitySourceProvider
{
	public static readonly ActivitySource Source = new("Thunders.TechTest.ApiService", "1.0.0");
}
