using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;

namespace UsefulHints
{
    public class UsefulHints : Plugin<Config>
    {
        public override string Name => "UsefulHints";
        public override string Author => "Vretu";
        public override string Prefix { get; } = "UH";
        public override Version Version => new Version(2, 2, 0);
        public override Version RequiredExiledVersion { get; } = new Version(9, 4, 0);
        public override PluginPriority Priority { get; } = PluginPriority.Low;
        public static UsefulHints Instance { get; private set; }
        public HeaderSetting SettingsHeader { get; set; } = new HeaderSetting("Useful Hints");
        public override void OnEnabled()
        {
            Instance = this;
            SettingBase.Register(new[] { SettingsHeader });
            if(Config.AutoUpdate){ UpdateChecker.RegisterEvents(); }
            if(Config.Translations){ _ = TranslationManager.RegisterEvents(); }
            if(Config.EnableHints){ EventHandlers.Entities.SCP096.RegisterEvents(); }
            if(Config.EnableHints){ EventHandlers.Items.Hints.RegisterEvents(); }
            if(Config.EnableWarnings){ EventHandlers.Items.WarningHints.RegisterEvents(); }
            if(Config.EnableFfWarning){ EventHandlers.Modules.FFWarning.RegisterEvents(); }
            if(Config.EnableKillCounter){ EventHandlers.Modules.KillCounter.RegisterEvents(); }
            if(Config.EnableLastHumanBroadcast){EventHandlers.Modules.LastHumanBroadcast.RegisterEvents(); }
            if(Config.EnableMapBroadcast) {EventHandlers.Modules.Maps.RegisterEvents(); }
            if(Config.EnableRoundSummary){ EventHandlers.Modules.RoundSummary.RegisterEvents(); }
            if(Config.EnableTeammates){ EventHandlers.Modules.Teammates.RegisterEvents(); }
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            Instance = null;
            SettingBase.Unregister(settings: new[] { SettingsHeader });
            if(Config.AutoUpdate){ UpdateChecker.UnregisterEvents(); }
            if(Config.EnableHints){ EventHandlers.Entities.SCP096.UnregisterEvents(); }
            if(Config.EnableHints){ EventHandlers.Items.Hints.UnregisterEvents(); }
            if(Config.EnableWarnings){ EventHandlers.Items.WarningHints.UnregisterEvents(); }
            if(Config.EnableFfWarning){ EventHandlers.Modules.FFWarning.UnregisterEvents(); }
            if(Config.EnableKillCounter){ EventHandlers.Modules.KillCounter.UnregisterEvents(); }
            if(Config.EnableLastHumanBroadcast){ EventHandlers.Modules.LastHumanBroadcast.UnregisterEvents(); }
            if(Config.EnableMapBroadcast) { EventHandlers.Modules.Maps.UnregisterEvents(); }
            if(Config.EnableRoundSummary){ EventHandlers.Modules.RoundSummary.UnregisterEvents(); }
            if(Config.EnableTeammates){ EventHandlers.Modules.Teammates.UnregisterEvents(); }
            base.OnDisabled();
        }
    }
}