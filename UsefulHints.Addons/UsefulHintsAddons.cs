using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Loader;
using UsefulHintsAddons.Integrations.RueI;

namespace UsefulHintsAddons
{
    public class UsefulHintsAddons : Plugin<Config>
    {
        public override string Name => "UsefulHints.Addons";
        public override string Author => "Vretu";
        public override string Prefix { get; } = "UH.Addons";
        public override Version Version => new Version(1, 4, 0);
        public override Version RequiredExiledVersion { get; } = new Version(9, 14, 2);
        public override PluginPriority Priority { get; } = PluginPriority.Lowest;
        public static UsefulHintsAddons Instance { get; private set; }
        private static UsefulHints.Config Core => UsefulHints.UsefulHints.Instance?.Config;

        public override void OnEnabled()
        {
            Instance = this;

            if (UsefulHints.UsefulHints.Instance == null)
            {
                var corePlugin = Loader.GetPlugin("UsefulHints");

                if (corePlugin == null)
                {
                    Log.Error("[UH-Addons] UsefulHints core plugin not found. Disabling Addons.");
                    Instance = null;
                    return;
                }
            }

            if (Config.EnableTranslations)
                _ = Extensions.TranslationManager.ApplyAsync(Config.Language);

            if (Config.EnableAutoUpdate)
                Extensions.UpdateChecker.Register();

            if (Config.EnableRueIIntegration)
            {
                InitializeRueI();
            }

            base.OnEnabled();
        }

        private void InitializeRueI()
        {
            // Use nameof(RueI) and PluginLoader.Plugins for checking RueI plugin presence
            if (!LabApi.Loader.PluginLoader.Plugins.Any(d => d.Key.Name == nameof(RueI)))
            {
                Log.Warn("[RueI] RueI not detected in dependencies. Skipping RueI integration.");
                return;
            }

            try
            {
                if (Core == null)
                {
                    Log.Warn("[RueI] Core Config not ready. Cannot disable core events.");
                }
                else
                {
                    if (Core.EnableHints)
                    {
                        UsefulHints.EventHandlers.Entities.SCP096.UnregisterEvents();
                        UsefulHints.EventHandlers.Items.Hints.UnregisterEvents();
                    }

                    if (Core.EnableWarnings)
                        UsefulHints.EventHandlers.Items.WarningHints.UnregisterEvents();

                    if (Core.EnableFfWarning)
                        UsefulHints.EventHandlers.Modules.FFWarning.UnregisterEvents();

                    if (Core.EnableKillCounter)
                        UsefulHints.EventHandlers.Modules.KillCounter.UnregisterEvents();

                    if (Core.EnableTeammates)
                        UsefulHints.EventHandlers.Modules.Teammates.UnregisterEvents();
                }

                RueIHandlers.Register();
                Log.Info("[RueI] Support Enabled (Core events overridden).");
            }
            catch (Exception ex)
            {
                Log.Error($"[RueI] Initialization failed: {ex.Message}");
                if (Config.Debug) Log.Debug(ex);
            }
        }
        public override void OnDisabled()
        {
            Extensions.UpdateChecker.Unregister();
            Extensions.TranslationManager.CancelPending();

            if (Config.EnableRueIIntegration)
            {
                try
                {
                    RueIHandlers.Unregister();
                }
                catch (Exception ex)
                {
                    Log.Warn("[RueI] Unregister failed: " + ex.Message);
                }
            }

            Instance = null;
            base.OnDisabled();
        }
    }
}