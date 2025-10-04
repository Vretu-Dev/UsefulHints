using System;
using CommandSystem;
using Exiled.Permissions.Extensions;
using UsefulHintsAddons.Extensions;

namespace UsefulHintsAddons.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ChangeLanguage : ICommand
    {
        public string Command => "usefulhints_language";
        public string[] Aliases => new[] { "uhl" };
        public string Description => "Changes or reloads UsefulHints language (Addons). Usage: uhl <lang|reload|list>";

        private static readonly string[] Supported =
        {
            "pl","en","de","fr","cs","sk","es","it","pt","ru","tr","zh","ko"
        };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("uh.changelanguage"))
            {
                response = "You don't have permission: uh.changelanguage";
                return false;
            }

            if (arguments.Count == 0)
            {
                response = GetHelp();
                return false;
            }

            string arg = arguments.At(0).ToLowerInvariant();

            if (arg is "list" or "langs" or "languages")
            {
                response = "Supported languages: " + string.Join(", ", Supported);
                return true;
            }

            if (arg is "reload" or "refresh")
            {
                var current = UsefulHintsAddons.Instance.Config.Language;
                _ = TranslationManager.ApplyAsync(current);
                response = $"Reloading language: {current}";
                return true;
            }

            if (!Supported.Contains(arg))
            {
                response = $"Unsupported language: {arg}. Use: uhl list";
                return false;
            }

            UsefulHintsAddons.Instance.Config.Language = arg;
            _ = TranslationManager.ApplyAsync(arg);
            response = $"Language changed to: {arg}";
            return true;
        }

        private string GetHelp() =>
            "Usage:\n" +
            "  uhl <lang>    - Change language (e.g. uhl pl)\n" +
            "  uhl reload    - Force re-download current language\n" +
            "  uhl list      - Show supported languages\n" +
            $"Supported: {string.Join(", ", Supported)}";
    }
}