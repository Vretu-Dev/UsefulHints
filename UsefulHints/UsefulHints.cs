using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using HarmonyLib;
using System;

namespace UsefulHints
{
    public class UsefulHints : Plugin<Config, Translations>
    {
        public override string Name => "UsefulHints";
        public override string Author => "Vretu";
        public override string Prefix { get; } = "UH";
        public override Version Version => new Version(10, 0, 0);
        public override Version RequiredExiledVersion { get; } = new Version(9, 8, 0);
        public override PluginPriority Priority { get; } = PluginPriority.Low;
        public static UsefulHints Instance { get; private set; }
        public HeaderSetting SettingsHeader { get; set; } = new HeaderSetting(772, "Useful Hints");

        private Harmony _harmony;

        public override void OnEnabled()
        {
            Instance = this;

            _harmony = new Harmony("com.vretu");
            _harmony.PatchAll();

            if (Config.EnableServerSettings) SettingBase.Register(new[] { SettingsHeader });
            if (Config.AutoUpdate) Extensions.UpdateChecker.RegisterEvents();
            if (Config.Translations) _ = Extensions.TranslationManager.RegisterEvents();
            if (Config.EnableServerSettings) Extensions.ServerSettings.RegisterSettings();
            if (Config.EnableHints) EventHandlers.Entities.SCP096.RegisterEvents();
            if (Config.EnableHints) EventHandlers.Items.Hints.RegisterEvents();
            if (Config.EnableWarnings) EventHandlers.Items.WarningHints.RegisterEvents();
            if (Config.EnableFfWarning) EventHandlers.Modules.FFWarning.RegisterEvents();
            if (Config.EnableKillCounter) EventHandlers.Modules.KillCounter.RegisterEvents();
            if (Config.EnableLastHumanBroadcast) EventHandlers.Modules.LastHumanBroadcast.RegisterEvents();
            if (Config.EnableMapBroadcast) EventHandlers.Modules.Maps.RegisterEvents();
            if (Config.EnableRoundSummary) EventHandlers.Modules.RoundSummary.RegisterEvents();
            if (Config.EnableTeammates) EventHandlers.Modules.Teammates.RegisterEvents();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Instance = null;

            _harmony?.UnpatchAll(_harmony.Id);
            _harmony = null;

            if (Config.EnableServerSettings) SettingBase.Unregister(settings: new[] { SettingsHeader });
            if (Config.AutoUpdate) Extensions.UpdateChecker.UnregisterEvents();
            if (Config.EnableServerSettings) Extensions.ServerSettings.UnregisterSettings();
            if (Config.EnableHints) EventHandlers.Entities.SCP096.UnregisterEvents();
            if (Config.EnableHints) EventHandlers.Items.Hints.UnregisterEvents();
            if (Config.EnableWarnings) EventHandlers.Items.WarningHints.UnregisterEvents();
            if (Config.EnableFfWarning) EventHandlers.Modules.FFWarning.UnregisterEvents();
            if (Config.EnableKillCounter) EventHandlers.Modules.KillCounter.UnregisterEvents();
            if (Config.EnableLastHumanBroadcast) EventHandlers.Modules.LastHumanBroadcast.UnregisterEvents();
            if (Config.EnableMapBroadcast) EventHandlers.Modules.Maps.UnregisterEvents();
            if (Config.EnableRoundSummary) EventHandlers.Modules.RoundSummary.UnregisterEvents();
            if (Config.EnableTeammates) EventHandlers.Modules.Teammates.UnregisterEvents();
            base.OnDisabled();
        }
    }
}