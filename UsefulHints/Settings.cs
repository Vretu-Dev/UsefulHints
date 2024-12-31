using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using Exiled.Events.EventArgs.Player;
using System.Collections.Generic;
using UserSettings.ServerSpecific;
using static PlayerArms;


namespace UsefulHints
{
    public static class ServerSettings
    {
        private static readonly string[] SupportedLanguages = { "en", "pl", "de", "fr", "cs", "sk", "es", "it", "pt", "ru", "tr", "zh" };
        public static DropdownSetting LanguageDropdown { get; private set; }
        private static readonly Dictionary<Player, string> PlayerLanguages = new Dictionary<Player, string>();

        public static void RegisterSettings()
        {
            HeaderSetting header = new HeaderSetting("Useful Hints");
            IEnumerable<SettingBase> settings = new SettingBase[]
            {
            header,
            new DropdownSetting(
                id: 1155,
                label: "Language Selection",
                options: SupportedLanguages,
                defaultOptionIndex: 0,
                hintDescription: "Select the language for UsefulHints",
                onChanged: OnLanguageChanged)
            };

            SettingBase.Register(settings);
            SettingBase.SendToAll();

            Exiled.Events.Handlers.Player.Verified += OnPlayerVerified;
        }
        private static void OnLanguageChanged(Player player, SettingBase setting)
        {
            if (setting is DropdownSetting dropdown)
            {
                string selectedLanguage = dropdown.SelectedOption;
                UsefulHints.Instance.Config.Language = selectedLanguage;
                _ = TranslationManager.RegisterEvents();
                Log.Info($"Language changed to {selectedLanguage} by {player?.Nickname ?? "server"}.");
            }
        }

        private static void OnPlayerVerified(VerifiedEventArgs ev)
        {
            ServerSpecificSettingsSync.SendToPlayer(ev.Player.ReferenceHub);
            Log.Debug("Player joined, sending settings.");
        }
    }
}
