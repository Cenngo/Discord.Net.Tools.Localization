using CommandLine;
using Discord.Rest;
using System.Resources.NetStandard;

const string NameEntryKey = "name";
const string DescriptionEntryKey = "description";

await Parser.Default.ParseArguments<Options>(args)
    .WithParsedAsync<Options>(async options =>
    {
        var outputFilePath = Path.Combine(options.OutputDir, options.FileName + ".resx");
        if (Path.GetExtension(outputFilePath) != ".resx")
            throw new ArgumentException("Output file extesion must be '.resx'");

        Console.WriteLine($"Initializing, using {outputFilePath} as output.");

        using var rw = new ResXResourceWriter(outputFilePath);

        Console.WriteLine("Creating REST client.");

        using var restClient = new DiscordRestClient();

        Console.WriteLine("Logging in to Discord.");

        await restClient.LoginAsync(Discord.TokenType.Bot, options.Token);

        Console.WriteLine($"Fetching commands of {restClient.CurrentUser.Username} from {(options.GuildId.HasValue ? options.GuildId.Value.ToString() : "global scope")}.");

        IReadOnlyCollection<RestApplicationCommand> commands = options.GuildId.HasValue ? await restClient.GetGuildApplicationCommands(options.GuildId.Value) :
            await restClient.GetGlobalApplicationCommands();

        Console.WriteLine($"Found {commands.Count} application commands.");

        foreach (var command in commands)
            WriteRestCommand(command, rw);

        Console.WriteLine("Done!\nLogging out...");
        await restClient.LogoutAsync();
    });

void WriteRestCommand(RestApplicationCommand command, ResXResourceWriter rw)
{
    Console.WriteLine($"Creating resources for '{command.Name}'.");

    rw.AddResource(CreateNameKey(command.Name, string.Empty), command.Name);
    rw.AddResource(CreateDescriptionKey(command.Name, string.Empty), command.Description);

    var commandPath = command.Name;

    foreach (var option in command.Options)
        WriteRestCommandOption(option, rw, commandPath);
}

void WriteRestCommandOption(RestApplicationCommandOption option, ResXResourceWriter rw, string path)
{
    rw.AddResource(CreateNameKey(option.Name, path), option.Name);
    rw.AddResource(CreateDescriptionKey(option.Name, path), option.Description);

    var optionPath = PathCombine(path, option.Name);

    if (option.Options.Count != 0)
        foreach (var subOption in option.Options)
            WriteRestCommandOption(subOption, rw, optionPath);

    if (option.Choices.Count != 0)
        foreach (var choice in option.Choices)
            WriteRestCommandOptionChoice(choice, rw, optionPath);
}

void WriteRestCommandOptionChoice(RestApplicationCommandChoice choice, ResXResourceWriter rw, string path)
{
    rw.AddResource(CreateNameKey(choice.Name, path), choice.Name);
}

string CreateNameKey(string name, string path) => string.Join(".", new List<string> { path, name, NameEntryKey }.Where(x => !string.IsNullOrEmpty(x)));

string CreateDescriptionKey(string name, string path) => string.Join(".", new List<string> { path, name, DescriptionEntryKey }.Where(x => !string.IsNullOrEmpty(x)));

string PathCombine(string name, string path) => string.IsNullOrEmpty(path) ? name : path + "." + name;

public class Options
{
    [Option('g', "guild", Required = false, HelpText = "Guild to fetch the commands from. Fetches global commnands if no guild is specified.")]
    public ulong? GuildId { get; set; }

    [Option('o', "output", Required = false, HelpText = "Output directory.")]
    public string OutputDir { get; set; } = "./";

    [Option('f', "file", Required = false, HelpText = "Name of the output file.")]
    public string FileName { get; set; } = "localizations";

    [Option('t', "token", Required = true, HelpText = "Token of the bot.")]
    public string Token { get; set; }
}