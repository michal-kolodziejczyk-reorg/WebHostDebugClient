
namespace Reorg.ApiGateway.WebHost.DebugClientLib.MassCallers
{
	internal interface IMassCaller
	{
		Task MakeCallsAsync(string url, int numberOfCalls, CancellationToken cancellationToken);
	}
}