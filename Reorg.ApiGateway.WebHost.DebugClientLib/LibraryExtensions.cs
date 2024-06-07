namespace Reorg.ApiGateway.WebHost.DebugClientLib;

public static class LibraryExtensions
{
	public static IServiceCollection TryAddLibrary(this IServiceCollection services)
	{
		services.AddHostedService<Hosted>();
		services.TryAddTransient<IMassCaller, MassCaller>();
		services.AddHttpClient();
		return services;
	}
}