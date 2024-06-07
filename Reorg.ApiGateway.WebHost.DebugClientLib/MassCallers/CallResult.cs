namespace Reorg.ApiGateway.WebHost.DebugClientLib.MassCallers;

internal sealed class CallResult
{
	public HttpStatusCode StatusCode { get; set; }
	public string Content { get; set; } = "";
	public TimeSpan Time { get; set; } = TimeSpan.MaxValue;
}