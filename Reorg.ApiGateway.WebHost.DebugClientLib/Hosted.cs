namespace Reorg.ApiGateway.WebHost.DebugClientLib;

internal sealed class Hosted(
	ILogger<Hosted> logger, 
	IOptions<AppSettings> options, 
	IConsole console,
	IMassCaller massCaller) : IHostedLifecycleService
{
	private AppSettings settings => options.Value;

	public Task StartingAsync(CancellationToken cancellationToken)
	{
		logger.LogDebug("starting");
		return Task.CompletedTask;
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		logger.LogDebug("start");
		while (true)
		{
			await massCaller.MakeCallsAsync(settings.Url, settings.NumberOfCalls, settings.WaitInOneBatchForSecond, cancellationToken);
			logger.LogCritical("Next round");
			//console.ReadKey();
		}
	}

	public Task StartedAsync(CancellationToken cancellationToken)
	{
		logger.LogDebug("started");
		return Task.CompletedTask;
	}

	public Task StoppingAsync(CancellationToken cancellationToken)
	{
		logger.LogDebug("stopping");
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		logger.LogDebug("stop");
		return Task.CompletedTask;
	}

	public Task StoppedAsync(CancellationToken cancellationToken)
	{
		logger.LogDebug("stopped");
		return Task.CompletedTask;
	}
}