using System;
using Exiled.API.Features;

namespace UsefulHints
{
    public class UsefulHints : Plugin<Config>
    {
        public override string Name => "Useful Hints";
        public override string Author => "Vretu";
        public override string Prefix { get; } = "UH";
        public override Version Version => new Version(1, 4, 0);
        public override Version RequiredExiledVersion { get; } = new Version(8, 9, 8);
        public static UsefulHints Instance { get; private set; }
        public override void OnEnabled()
        {
            Instance = this;
            if(Config.EnableHints){ EventHandlers.Entities.SCP096.RegisterEvents(); }
            if(Config.EnableHints){ EventHandlers.Items.Hints.RegisterEvents(); }
            EventHandlers.Modules.JailbirdPatchHandler.RegisterEvents();
            EventHandlers.Modules.KillCounter.RegisterEvents();
            EventHandlers.Modules.LastHumanBroadcast.RegisterEvents();
            EventHandlers.Modules.RoundSummary.RegisterEvents();
            EventHandlers.Modules.Teammates.RegisterEvents();
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            Instance = null;
            if(Config.EnableHints){ EventHandlers.Entities.SCP096.UnregisterEvents(); }
            if(Config.EnableHints){ EventHandlers.Items.Hints.UnregisterEvents(); }
            EventHandlers.Modules.JailbirdPatchHandler.UnregisterEvents();
            EventHandlers.Modules.KillCounter.UnregisterEvents();
            EventHandlers.Modules.LastHumanBroadcast.UnregisterEvents();
            EventHandlers.Modules.RoundSummary.UnregisterEvents();
            EventHandlers.Modules.Teammates.UnregisterEvents();
            base.OnDisabled();
        }
    }
}