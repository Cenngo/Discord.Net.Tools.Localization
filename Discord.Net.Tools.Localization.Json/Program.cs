using Discord.Net.Tools.Localization;
using Discord.Net.Tools.Localization.Common;
using Discord.Net.Tools.Localization.Json;
using Discord.Rest;
using Newtonsoft.Json;
using Serilog;
using System.Text;

await Utils.RunAsync(args, ProcessCommands);

async Task ProcessCommands(IReadOnlyCollection<RestApplicationCommand> commands, Options options)
{
    var outputFilePath = Path.Combine(options.OutputDir, options.FileName + ".json");
    if (Path.GetExtension(outputFilePath) != ".json")
        throw new ArgumentException("Output file extesion must be '.json'");

    Log.Information($"Initializing, using {outputFilePath} as output.");

    using var sw = new StreamWriter(outputFilePath);
    var entries = commands.ToDictionary(x => x.Name, x => x.ToEntry(options.SkipNullOrEmpty));

    Log.Information($"Serializing JSON object with {entries.Count} entries.");

    var json = JsonConvert.SerializeObject(entries, Formatting.Indented);

    Log.Information($"Writing the JSON file.");

    await sw.WriteAsync(json);
}