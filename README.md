# Overview
Discord Slash Commands support name/description localization. Localization is available for names and descriptions of Slash Command Groups [(GroupAttribute)](https://discordnet.dev/api/Discord.Interactions.GroupAttribute.html), Slash Commands [(SlashCommandAttribute)](https://discordnet.dev/api/Discord.Interactions.SlashCommandAttribute.html), Slash Command parameters and Slash Command Parameter Choices. 

# Installation
**Resx Templates:** `dotnet tool install --global Discord.Net.Tools.Localization.Resx`

**Json Templates:** `dotnet tool install --global Discord.Net.Tools.Localization.Json`

# Updating the Tools
**Resx Templates:** `dotnet tool update --global Discord.Net.Tools.Localization.Resx`

**Json Templates:** `dotnet tool update --global Discord.Net.Tools.Localization.Json`

# Usage
This tool can generate Resx and Json file templates which can be used for localization. By default this tool will generate a file for every command and description in your application and pre-fill values with your current values. You can then modify these files as needed or translate them to other langauges.

### ResXLocalizationManager
The ResxLocalizationManager uses `.` delimited key names to traverse the resource files and get the localized strings (`group1.group2.command.parameter.name`). A ResxLocalizationManager instance must be initialized with a base resource name, a target assembly and a collection of `CultureInfo`s. Every key path must end with either `.name` or `.description`, including parameter choice strings. This tool can be used to create a resx localization file template using the following command: `Resx-Template -t <TOKEN>` where you replace `<TOKEN>` with your application's token.

### JsonLocalizationManager
The JsonLocaliationManager uses a nested data structure similar to Discord's Application Commands schema. You can get the Json schema [here](https://gist.github.com/Cenngo/d46a881de24823302f66c3c7e2f7b254). JsonLocalizationManager accepts a base path and a base file name and automatically discovers every resource file ( \basePath\fileName.locale.json ). A Json resource file can be generated with this tool by running the following command: `Json-Template -t <TOKEN>` where you replace `<TOKEN>` with your application's token.

### Using The Generated Files in Discord.Net
The Discord.Net Interaction Service can be initialized with an ILocalizationManager instance in its config which is used to create the necessary localization dictionaries on command registration. Interaction Service has two built-in ILocalizationManager implementations: ResxLocalizationManager and JsonLocalizationManager.

*Insert Usage Example Code Here*

# Command Parameters
| Parameter | Description |
| --- | --- |
| --help | Display a list of all parameters. |
| --version | Display currnet version information. |
| -t | **Required.** The token of the application you want to generate the template for. |
| -g | Guild to fetch the commands from. Fetches global commnands if no guild is specified. |
| -o | The output directory to use. Defaults to current directory if not specified. |
| -f | Name of the output file. Defaults to localizations is not specified. |
