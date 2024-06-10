namespace Reorg.ApiGateway.WebHost.DebugClientLib.MassCallers;

internal sealed class MassCaller(IHttpClientFactory httpClientFactory, ILogger<MassCaller> logger) : IMassCaller
{
	public async Task MakeCallsAsync(string url, int numberOfCalls, int secondsToWait, CancellationToken cancellationToken)
	{
		logger.LogTrace("Starting {numberOfCalls} calls to url {url}", numberOfCalls, url);
		var tasks = new Task[numberOfCalls];
		for (int index = 0; index < numberOfCalls; index++)
		{
			var hitUrl = url.Replace("{param}", index.ToString());
			var task = MakeTwoCallsAsync(hitUrl, secondsToWait, cancellationToken);
			tasks[index] = task;
		}
		await Task.WhenAll(tasks);
		logger.LogDebug("Finished {numberOfCalls} double calls to url {url}", numberOfCalls, url);
	}

	private async Task MakeTwoCallsAsync(string hitUrl, int secondsToWait, CancellationToken cancellationToken)
	{
		await MakeOneCallAsync(hitUrl, cancellationToken);
		logger.LogInformation("Waiting {seconds} seconds to call again same url {url}", secondsToWait, hitUrl);
		await SafeTask.Delay(secondsToWait * 1_000, cancellationToken);
		await MakeOneCallAsync(hitUrl, cancellationToken);
	}

	private async Task MakeOneCallAsync(string url, CancellationToken cancellationToken)
	{
		using var httpClient = httpClientFactory.CreateClient();
		var message = CreateMessage(url);
		var timestamp = TimeProvider.System.GetTimestamp();
		try
		{
			var response = await httpClient.SendAsync(message, cancellationToken);
			var content = await response.Content.ReadAsStringAsync(cancellationToken);
			var time = TimeProvider.System.GetElapsedTime(timestamp);
			var statusCode = response.StatusCode;
			logger.LogInformation("Call to {url} status code {statusCode} content {content} time {time}", url, statusCode, content, time);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Error on call {url}", url);
		}
	}

	private static HttpRequestMessage CreateMessage(string url)
	{
		var message = new HttpRequestMessage()
		{
			Method = HttpMethod.Get,
			RequestUri = new Uri(url)
		};

		message.Headers.Add("Authorization", "hardcoded same client id");

		return message;
	}
}