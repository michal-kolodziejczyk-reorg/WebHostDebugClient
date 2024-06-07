namespace Reorg.ApiGateway.WebHost.DebugClientLib.MassCallers;

internal sealed class MassCaller(IHttpClientFactory httpClientFactory, ILogger<MassCaller> logger) : IMassCaller
{
	public async Task MakeCallsAsync(string url, int numberOfCalls, CancellationToken cancellationToken)
	{
		logger.LogDebug("Starting {numberOfCalls} calls to url {url}", numberOfCalls, url);
		var tasks = new Task[numberOfCalls];
		var callResults = new CallResult[numberOfCalls];
		for (int index = 0; index < numberOfCalls; index++)
		{
			var hitUrl = url.Replace("{param}", index.ToString());
			callResults[index] = new();
			var task = MakeOneCallAsync(hitUrl, callResults[index], cancellationToken);
			tasks[index] = task;
		}
		await Task.WhenAll(tasks);
		logger.LogInformation("Finished {numberOfCalls} calls to url {url}", numberOfCalls, url);
		for (int index = 0; index < callResults.Length; index++)
		{
			var result = callResults[index];
			logger.LogInformation("Call result {index} Time {time} StatusCode {statusCode} Content {content}",
				index, result.Time, result.StatusCode, result.Content);
		}
		var max = callResults.Max(x => x.Time);
		var min = callResults.Min(x => x.Time);
		long avgTicks = (long)callResults.Average(x => x.Time.Ticks);
		var avg = TimeSpan.FromTicks(avgTicks);
		logger.LogInformation("Time: min {min} max {max} avg {avg}", min, max, avg);
	}

	private async Task MakeOneCallAsync(string url, CallResult result, CancellationToken cancellationToken)
	{
		using var httpClient = httpClientFactory.CreateClient();
		var message = CreateMessage(url);
		var timestamp = TimeProvider.System.GetTimestamp();
		var response = await httpClient.SendAsync(message, cancellationToken);
		result.Content = await response.Content.ReadAsStringAsync(cancellationToken);
		result.Time = TimeProvider.System.GetElapsedTime(timestamp);
		result.StatusCode = response.StatusCode;
	}

	private static HttpRequestMessage CreateMessage(string url)
	{
		var message = new HttpRequestMessage()
		{
			Method = HttpMethod.Get,
			RequestUri = new Uri(url)
		};

		message.Headers.Add("ClientId", "same");

		return message;
	}
}