using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Thunders.TechTest.ApiService.Logging;
[ExcludeFromCodeCoverage]
public static class StructuredLogging
{
	public static void LogOperation(
		this ILogger logger,
		string operationName,
		LogLevel logLevel,
		string message,
		Exception? exception = null,
		IDictionary<string, object>? additionalData = null)
	{
		var scope = new Dictionary<string, object>
		{
			["OperationName"] = operationName,
			["TraceId"] = Activity.Current?.TraceId.ToString() ?? "N/A",
			["SpanId"] = Activity.Current?.SpanId.ToString() ?? "N/A"
		};

		if (additionalData != null)
		{
			foreach (var (key, value) in additionalData)
			{
				scope[key] = value;
			}
		}

		using (logger.BeginScope(scope))
		{
			if (exception != null)
			{
				logger.Log(logLevel, exception, message);
			}
			else
			{
				logger.Log(logLevel, message);
			}
		}
	}

	public static void LogOperationStart(
		this ILogger logger,
		string operationName,
		string message,
		IDictionary<string, object>? additionalData = null)
	{
		logger.LogOperation(operationName, LogLevel.Information, $"Iniciando: {message}", additionalData: additionalData);
	}

	public static void LogOperationEnd(
		this ILogger logger,
		string operationName,
		string message,
		IDictionary<string, object>? additionalData = null)
	{
		logger.LogOperation(operationName, LogLevel.Information, $"Finalizando: {message}", additionalData: additionalData);
	}

	public static void LogOperationError(
		this ILogger logger,
		string operationName,
		string message,
		Exception exception,
		IDictionary<string, object>? additionalData = null)
	{
		logger.LogOperation(operationName, LogLevel.Error, message, exception, additionalData);
	}
}