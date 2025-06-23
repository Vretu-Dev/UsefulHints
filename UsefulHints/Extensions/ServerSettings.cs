using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;

namespace UsefulHints.Extensions
{
    public static class ServerSettings
    {
        public static TwoButtonsSetting ShowHintSetting { get; private set; }
        public static TwoButtonsSetting ShowTimersSetting { get; private set; }
        public static TwoButtonsSetting ShowWarningHintSetting { get; private set; }
        public static TwoButtonsSetting ShowFFWarningSetting { get; private set; }
        public static TwoButtonsSetting ShowKillCounterSetting { get; private set; }
        private static Config Config => UsefulHints.Instance.Config;
        private static Translations Translation => UsefulHints.Instance.Translation;

        public static void RegisterSettings()
        {

            if (Config.EnableHints)
            {
                ShowHintSetting = new TwoButtonsSetting(
                id: 777,
                label: Translation.ShowHints,
                firstOption: Translation.ButtonOn,
                secondOption: Translation.ButtonOff,
                defaultIsSecond: false,
                hintDescription: Translation.ShowHintsDescription,
                onChanged: (player, setting) =>
                {
                    var showHints = (setting as TwoButtonsSetting)?.IsFirst ?? true;
                    player.SessionVariables["ShowHints"] = showHints;
                });

                SettingBase.Register(new[] { ShowHintSetting });

                ShowTimersSetting = new TwoButtonsSetting(
                id: 776,
                label: Translation.ShowTimers,
                firstOption: Translation.ButtonOn,
                secondOption: Translation.ButtonOff,
                defaultIsSecond: false,
                hintDescription: Translation.ShowTimersDescription,
                onChanged: (player, setting) =>
                {
                    var showTimers = (setting as TwoButtonsSetting)?.IsFirst ?? true;
                    player.SessionVariables["ShowTimers"] = showTimers;
                });

                SettingBase.Register(new[] { ShowTimersSetting });
            }

            if (Config.EnableWarnings)
            {
                ShowWarningHintSetting = new TwoButtonsSetting(
                id: 775,
                label: Translation.ItemWarningHints,
                firstOption: Translation.ButtonOn,
                secondOption: Translation.ButtonOff,
                defaultIsSecond: false,
                hintDescription: Translation.ItemWarningHintsDescription,
                onChanged: (player, setting) =>
                {
                    var showWarningHintSetting = (setting as TwoButtonsSetting)?.IsFirst ?? true;
                    player.SessionVariables["ShowWarningHints"] = showWarningHintSetting;
                });

                SettingBase.Register(new[] { ShowWarningHintSetting });
            }

            if (Config.EnableFfWarning)
            {
                ShowFFWarningSetting = new TwoButtonsSetting(
                id: 774,
                label: Translation.FriendlyFireWarning,
                firstOption: Translation.ButtonOn,
                secondOption: Translation.ButtonOff,
                defaultIsSecond: false,
                hintDescription: Translation.FriendlyFireWarningDescription,
                onChanged: (player, setting) =>
                {
                    var showFFWarning = (setting as TwoButtonsSetting)?.IsFirst ?? true;
                    player.SessionVariables["ShowFFWarning"] = showFFWarning;
                });

                SettingBase.Register(new[] { ShowFFWarningSetting });
            }

            if (Config.EnableKillCounter)
            {
                ShowKillCounterSetting = new TwoButtonsSetting(
                id: 773,
                label: Translation.KillCounter,
                firstOption: Translation.ButtonOn,
                secondOption: Translation.ButtonOff,
                defaultIsSecond: false,
                hintDescription: Translation.KillCounterDescription,
                onChanged: (player, setting) =>
                {
                    var showKillCount = (setting as TwoButtonsSetting)?.IsFirst ?? true;
                    player.SessionVariables["ShowKillCount"] = showKillCount;
                });

                SettingBase.Register(new[] { ShowKillCounterSetting });
            }
        }

        public static void UnregisterSettings()
        {
            if (ShowHintSetting != null)
                SettingBase.Unregister(settings: new[] { ShowHintSetting });

            if (ShowTimersSetting != null)
                SettingBase.Unregister(settings: new[] { ShowTimersSetting });

            if (ShowWarningHintSetting != null)
                SettingBase.Unregister(settings: new[] { ShowWarningHintSetting });

            if (ShowFFWarningSetting != null)
                SettingBase.Unregister(settings: new[] { ShowFFWarningSetting });

            if (ShowKillCounterSetting != null)
                SettingBase.Unregister(settings: new[] { ShowKillCounterSetting });
        }

        public static bool ShouldShowHints(Player player) => !(player.SessionVariables.TryGetValue("ShowHints", out var value) && value is bool enabled && !enabled);
        public static bool ShouldShowTimers(Player player) => !(player.SessionVariables.TryGetValue("ShowTimers", out var value) && value is bool enabled && !enabled);
        public static bool ShouldShowWarningHints(Player player) => !(player.SessionVariables.TryGetValue("ShowWarningHints", out var value) && value is bool enabled && !enabled);
        public static bool ShouldShowFFWarning(Player player) => !(player.SessionVariables.TryGetValue("ShowFFWarning", out var value) && value is bool enabled && !enabled);
        public static bool ShouldShowKillCount(Player player) => !(player.SessionVariables.TryGetValue("ShowKillCount", out var value) && value is bool enabled && !enabled);
    }
}