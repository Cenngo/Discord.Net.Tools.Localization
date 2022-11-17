using Discord.Net.Tools.Localization;
using Discord.Net.Tools.Localization.Common;
using Discord.Rest;
using Serilog;
using System.Resources.NetStandard;

const string NameEntryKey = "name";
const string DescriptionEntryKey = "description";

await Utils.RunAsync(args, ProcessCommands);

Task ProcessCommands(IReadOnlyCollection<RestApplicationCommand> commands, Options options)
{
    var outputFilePath = Path.Combine(options.OutputDir, options.FileName + ".resx");
    if (Path.GetExtension(outputFilePath) != ".resx")
        throw new ArgumentException("Output file extesion must be '.resx'");

    Log.Information($"Initializing, using {outputFilePath} as output.");

    using var rw = new ResXResourceWriter(outputFilePath);

    foreach (var command in commands)
        WriteRestCommand(command, rw, options);

    return Task.CompletedTask;
}

void WriteRestCommand(RestApplicationCommand command, ResXResourceWriter rw, Options o)
{
    Log.Information($"Creating resources for '{command.Name}'.");

    if(!(string.IsNullOrEmpty(command.Name) && o.SkipNullOrEmpty))
        rw.AddResource(CreateNameKey(command.Name, string.Empty), command.Name);

    if(!(string.IsNullOrEmpty(command.Description) && o.SkipNullOrEmpty))
        rw.AddResource(CreateDescriptionKey(command.Name, string.Empty), command.Description);

    var commandPath = command.Name;

    foreach (var option in command.Options)
        WriteRestCommandOption(option, rw, commandPath, o);
}

void WriteRestCommandOption(RestApplicationCommandOption option, ResXResourceWriter rw, string path, Options o)
{
    if(!(string.IsNullOrEmpty(option.Name) && o.SkipNullOrEmpty))
        rw.AddResource(CreateNameKey(option.Name, path), option.Name);

    if(!(string.IsNullOrEmpty(option.Description) && o.SkipNullOrEmpty))
        rw.AddResource(CreateDescriptionKey(option.Name, path), option.Description);

    var optionPath = PathCombine(path, option.Name);

    if (option.Options.Count != 0)
        foreach (var subOption in option.Options)
            WriteRestCommandOption(subOption, rw, optionPath, o);

    if (option.Choices.Count != 0)
        foreach (var choice in option.Choices)
            WriteRestCommandOptionChoice(choice, rw, optionPath, o);
}

void WriteRestCommandOptionChoice(RestApplicationCommandChoice choice, ResXResourceWriter rw, string path, Options o)
{
    if(!(string.IsNullOrEmpty(choice.Name) && o.SkipNullOrEmpty))
        rw.AddResource(CreateNameKey(choice.Name, path), choice.Name);
}

string CreateNameKey(string name, string path) => string.Join(".", new List<string> { path, name, NameEntryKey }.Where(x => !string.IsNullOrEmpty(x)));

string CreateDescriptionKey(string name, string path) => string.Join(".", new List<string> { path, name, DescriptionEntryKey }.Where(x => !string.IsNullOrEmpty(x)));

string PathCombine(string name, string path) => string.IsNullOrEmpty(path) ? name : path + "." + name;