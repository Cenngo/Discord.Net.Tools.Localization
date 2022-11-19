using Discord.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Discord.Net.Tools.Localization.Json
{
    [JsonObject(MissingMemberHandling = MissingMemberHandling.Ignore, ItemNullValueHandling = NullValueHandling.Ignore)]
    public record Entry
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonExtensionData(WriteData = true)]
        public IDictionary<string, object> Entries { get; set; }
    }

    public static class EntryExtensions
    {
        public static Entry ToEntry(this RestApplicationCommand command, bool skipEmpty)
        {
            return new()
            {
                Name = command.Name,
                Description = string.IsNullOrEmpty(command.Description) && skipEmpty ? null : command.Description,
                Entries = command.Options.ToDictionary(x => x.Name, x => x.ToEntry() as object)
            };
        }

        public static Entry ToEntry(this RestApplicationCommandOption option)
        {
            IDictionary<string, object>? subEntries = option.Type switch
            {
                ApplicationCommandOptionType.SubCommand or ApplicationCommandOptionType.SubCommandGroup =>
                    option.Options?.ToDictionary(x => x.Name, x => x.ToEntry() as object),
                _ => option.Choices?.ToDictionary(x => x.Name, x => x.ToEntry() as object)
            };

            return new()
            {
                Name = option.Name,
                Description = option.Description,
                Entries = subEntries!
            };
        }

        public static Entry ToEntry(this RestApplicationCommandChoice choice) =>
            new Entry
            {
                Name = choice.Name
            };
    }
}
