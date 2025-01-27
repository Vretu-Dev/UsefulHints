using CommandSystem;
using Exiled.Permissions.Extensions;
using System;

namespace UsefulHints.EventHandlers.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ChangeLanguageCommand : ICommand
    {
        public string Command => "usefulhints_language";
        public string[] Aliases => new string[] { "uhl" };
        public string Description => "Changes the language of UsefulHints.";
        private static readonly string[] SupportedLanguages = { "pl", "en", "de", "fr", "cs", "sk", "es", "it", "pt", "ru", "tr", "zh", "ko" };
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("uh.changelanguage"))
            {
                response = "Missing permission uh.changelanguage";
                return false;
            }
            if (arguments.Count != 1)
            {
                response = "Usage: uhl <language>";
                return false;
            }

            string newLanguage = arguments.At(0).ToLower();

            if (!SupportedLanguages.Contains(newLanguage))
            {
                response = $"Unsupported language: {newLanguage}. Supported languages: {string.Join(", ", SupportedLanguages)}.";
                return false;
            }

            UsefulHints.Instance.Config.Language = newLanguage;
            _ = TranslationManager.RegisterEvents();

            response = $"The language has been changed to: {newLanguage}";
            return true;
        }
    }
}