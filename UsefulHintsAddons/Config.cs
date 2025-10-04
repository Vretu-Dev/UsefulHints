using Exiled.API.Interfaces;
using System.ComponentModel;

namespace UsefulHintsAddons
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("Download & apply remote translations.")]
        public bool EnableTranslations { get; set; } = true;

        [Description("pl, en, de, fr, cs, sk, es, it, pt, ru, tr, zh, ko")]
        public string Language { get; set; } = "en";

        [Description("Check GitHub releases on WaitingForPlayers.")]
        public bool EnableAutoUpdate { get; set; } = true;

        [Description("Only notify about new version.")]
        public bool NotifyOnly { get; set; } = false;

        [Description("Create .backup before overwrite.")]
        public bool EnableBackup { get; set; } = false;

        [Description("Show logs from addons.")]
        public bool EnableLogging { get; set; } = true;

        [Description("After downloading a new version restart server after round.")]
        public bool RestartNextRound { get; set; } = true;
    }
}