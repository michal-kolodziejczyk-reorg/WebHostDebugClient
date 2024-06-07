namespace Reorg.ApiGateway.WebHost.DebugClient;

file static class Program
{
	static async Task<int> Main(string[] args)
	{
		var builder = CreateBuilder(args);

		AddLogging(builder);
		AddServices(builder);
		AddConfiguration(args, builder);

		return await RunHostAsync(builder);
	}

	private static HostApplicationBuilder CreateBuilder(string[] args) => Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings
	{
		DisableDefaults = true,
		ApplicationName = "Sygenic.Template.Console",
		Args = args,
		EnvironmentName = "Development"
	});

	private static void AddLogging(HostApplicationBuilder builder)
	{
		builder.Logging.AddColorConsole();
		builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
	}

	private static void AddServices(HostApplicationBuilder builder)
	{
		builder.Services.AddSingleton(TimeProvider.System);
		builder.Services.TryAddColorFormatterHelper();
		builder.Services.TryAddEventHandling();
		builder.Services.DiscoverEventHandlers();
		builder.Services.TryAddHelp();
		builder.Services.TryAddReflection();
		builder.Services.TryAddSerializer();
		builder.Services.TryAddBridges();
		builder.Services.TryAddLibrary();
	}

	private static void AddConfiguration(string[] args, HostApplicationBuilder builder)
	{
		var tempBuilderEnvironment = new ServiceCollection().TryAddBridges().BuildServiceProvider().Get<IEnvironment>();

		builder.Services.AddOptions();
		builder.Services.AutoConfigure<AppSettings, ColorFormatterSettings, HelpSettings>(builder.Configuration);

		builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
		builder.Configuration.AddJsonFile($"appsettings.Machine.{tempBuilderEnvironment.MachineName}.json", optional: true, reloadOnChange: true);
		builder.Configuration.AddJsonFile($"appsettings.User.{tempBuilderEnvironment.UserName}.json", optional: true, reloadOnChange: true);
		builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true);
		builder.Configuration.AddEnvironmentVariables("SYGENIC_");
		builder.Configuration.AddCommandLine(args);

		builder.Services.TryAddSetableConfiguration(builder.Configuration); // programatic configuration overrides everything else
	}

	private static async Task<int> RunHostAsync(HostApplicationBuilder builder)
	{
		var host = builder.Build();
		var services = host.Services;
		var console = services.Get<IConsole>();
		try
		{
			services.Get<IHelp>().MaybeDisplayHelpFile();
			services.Get<IColorFormatterHelper>().MaybeDisplayAllEnabledLogLevels();
			await host.RunAsync(CancellationToken.None);
			return 0;
		}
		catch (TaskCanceledException)
		{
			console.Error.WriteLine($"TaskCanceledException (Ctrl+C maybe), no proper software finish");
			return 2;
		}
	}
}