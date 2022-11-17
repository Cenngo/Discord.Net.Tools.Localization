using CommandLine;
using Discord.Rest;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Discord.Net.Tools.Localization.Common
{
    public static class Utils
    {
        public delegate Task ProcessCommand(IReadOnlyCollection<RestApplicationCommand> commands, Options options);

        public static async Task RunAsync(string[] args, ProcessCommand callback)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            await Parser.Default.ParseArguments<Options>(args)
                .WithParsedAsync(options => RunInternalAsync(options, callback));
        }

        private static async Task RunInternalAsync(Options options, ProcessCommand callback)
        {
            Log.Information("Creating REST client.");

            using var restClient = new DiscordRestClient();

            Log.Information("Logging in to Discord.");

            var token = await GetTokenAsync(options);

            await restClient.LoginAsync(TokenType.Bot, token);

            Log.Information($"Fetching commands of {restClient.CurrentUser.Username} from {(options.GuildId.HasValue ? options.GuildId.Value.ToString() : "global scope")}.");

            IReadOnlyCollection<RestApplicationCommand> commands = options.GuildId.HasValue ? await restClient.GetGuildApplicationCommands(options.GuildId.Value) :
                await restClient.GetGlobalApplicationCommands();

            Log.Information($"Found {commands.Count} application commands.");

            Log.Information("Done! Logging out...");
            await restClient.LogoutAsync();

            await callback(commands, options);
        }

        private static async ValueTask<string> GetTokenAsync(Options options)
        {
            if (!string.IsNullOrEmpty(options.Token))
                return options.Token;

            if (string.IsNullOrEmpty(options.TokenPath))
                throw new ArgumentNullException(nameof(Options.TokenPath), "Must point to a valid .txt file.");

            var txt = await File.ReadAllTextAsync(options.TokenPath);

            if (string.IsNullOrEmpty(txt))
                throw new InvalidOperationException(".txt file containing the bot token is empty.");

            return txt;
        }
    }
}
