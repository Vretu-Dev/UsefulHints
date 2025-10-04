using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using Exiled.Loader;
using MEC;
using System;
using System.Linq;

namespace UsefulHintsAddons
{
    public class UsefulHintsAddons : Plugin<Config>
    {
        public override string Name => "UsefulHints.Addons";
        public override string Author => "Vretu";
        public override string Prefix { get; } = "UH.Addons";
        public override Version Version => new Version(1, 0, 0);
        public override Version RequiredExiledVersion { get; } = new Version(9, 9, 0);
        public override PluginPriority Priority { get; } = PluginPriority.Lowest;
        public static UsefulHintsAddons Instance { get; private set; }

        public override void OnEnabled()
        {
            Instance = this;

            if (UsefulHints.UsefulHints.Instance == null && Loader.GetPlugin("UsefulHints") == null)
            {
                Log.Error("UsefulHints core plugin not found. Disabling Addons.");
                Instance = null;
                return;
            }

            if (Config.EnableTranslations)
                _ = Extensions.TranslationManager.ApplyAsync(Config.Language);

            if (Config.EnableAutoUpdate)
                Extensions.UpdateChecker.Register();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Extensions.UpdateChecker.Unregister();
            Extensions.TranslationManager.CancelPending();
            Instance = null;
            base.OnDisabled();
        }
    }
}