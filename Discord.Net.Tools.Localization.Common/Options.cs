using CommandLine;

namespace Discord.Net.Tools.Localization
{
    public class Options
    {
        [Option('g', "guild", Required = false, HelpText = "Guild to fetch the commands from. Fetches global commnands if no guild is specified.")]
        public ulong? GuildId { get; set; }

        [Option('o', "output", Required = false, HelpText = "Output directory.")]
        public string OutputDir { get; set; } = "./";

        [Option('f', "file", Required = false, HelpText = "Name of the output file.")]
        public string FileName { get; set; } = "localizations";

        [Option('t', "token", SetName = "Token as Argument", Required = true, HelpText = $"Token of the bot. (Argument mutually exclusive with {nameof(TokenPath)})")]
        public string Token { get; set; }

        [Option('T', "tokenPath", SetName = "Token as File", Required = true, HelpText = $"Path of the .txt file that contains the bot token. (Argument mutually exclusive with {nameof(Token)})")]
        public string TokenPath { get; set; } = "token.txt";

        [Option('s', "skipEmpty", Required =  false, 
            HelpText = "Empty or null string values are excluded from the final localization file. Such as description fields of Context Commands.")]
        public bool SkipNullOrEmpty { get; set; }
    }
}