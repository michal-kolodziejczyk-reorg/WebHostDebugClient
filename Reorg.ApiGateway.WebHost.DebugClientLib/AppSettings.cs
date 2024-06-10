namespace Reorg.ApiGateway.WebHost.DebugClientLib;

public sealed record AppSettings
{
	public required string Url { get; init; }
	public required int NumberOfCalls { get; init; }
	public required int WaitInOneBatchForSecond { get; init; }
}