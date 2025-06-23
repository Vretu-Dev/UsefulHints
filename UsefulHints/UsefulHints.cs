using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using System;

namespace UsefulHints
{
    public class UsefulHints : Plugin<Config>
    {
        public override string Name => "UsefulHints";
        public override string Description => "Display extra hint like a timers and notifications."; 
        public override string Author => "Vretu";
        public override Version Version => new Version(1, 3, 0);
        public override Version RequiredApiVersion { get; } = new Version(LabApiProperties.CompiledVersion);
        public static UsefulHints Instance { get; private set; }
        public override void Enable()
        {
            Instance = this;
            if(Config.EnableHints){ EventHandlers.Entities.SCP096.RegisterEvents(); }
            if(Config.EnableHints){ EventHandlers.Items.Hints.RegisterEvents(); }
            if(Config.EnableWarnings){ EventHandlers.Items.WarningHints.RegisterEvents(); }
            if(Config.EnableFfWarning){ EventHandlers.Modules.FFWarning.RegisterEvents(); }
            if(Config.EnableKillCounter){ EventHandlers.Modules.KillCounter.RegisterEvents(); }
            if(Config.EnableLastHumanBroadcast){EventHandlers.Modules.LastHumanBroadcast.RegisterEvents(); }
            if(Config.EnableMapBroadcast) {EventHandlers.Modules.Maps.RegisterEvents(); }
            if(Config.EnableRoundSummary){ EventHandlers.Modules.RoundSummary.RegisterEvents(); }
            if(Config.EnableTeammates){ EventHandlers.Modules.Teammates.RegisterEvents(); }
        }
        public override void Disable()
        {
            Instance = null;
            if(Config.EnableHints){ EventHandlers.Entities.SCP096.UnregisterEvents(); }
            if(Config.EnableHints){ EventHandlers.Items.Hints.UnregisterEvents(); }
            if(Config.EnableWarnings){ EventHandlers.Items.WarningHints.UnregisterEvents(); }
            if(Config.EnableFfWarning){ EventHandlers.Modules.FFWarning.UnregisterEvents(); }
            if(Config.EnableKillCounter){ EventHandlers.Modules.KillCounter.UnregisterEvents(); }
            if(Config.EnableLastHumanBroadcast){ EventHandlers.Modules.LastHumanBroadcast.UnregisterEvents(); }
            if(Config.EnableMapBroadcast) { EventHandlers.Modules.Maps.UnregisterEvents(); }
            if(Config.EnableRoundSummary){ EventHandlers.Modules.RoundSummary.UnregisterEvents(); }
            if(Config.EnableTeammates){ EventHandlers.Modules.Teammates.UnregisterEvents(); }
        }
    }
}