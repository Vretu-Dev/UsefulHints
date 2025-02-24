using Exiled.API.Features.Core.UserSettings;

namespace UsefulHints.Extensions
{
    public static class ServerSpecificSettings
    {
        public static TwoButtonsSetting ShowHintSetting { get; private set; }
        public static TwoButtonsSetting ShowTimersSetting { get; private set; }
        public static TwoButtonsSetting ShowWarningHintSetting { get; private set; }
        public static TwoButtonsSetting ShowFFWarningSetting { get; private set; }
        public static TwoButtonsSetting ShowKillCounterSetting { get; private set; }
        public static void RegisterSettings()
        {
            if (UsefulHints.Instance.Config.EnableHints)
            {
                ShowHintSetting = new TwoButtonsSetting(
                id: 777,
                label: UsefulHints.Instance.Translation.ShowHints,
                firstOption: UsefulHints.Instance.Translation.ButtonOn,
                secondOption: UsefulHints.Instance.Translation.ButtonOff,
                defaultIsSecond: false,
                hintDescription: UsefulHints.Instance.Translation.ShowHintsDescription,
                onChanged: (player, setting) =>
                {
                    var showHints = (setting as TwoButtonsSetting)?.IsFirst ?? true;
                    player.SessionVariables["ShowHints"] = showHints;
                });
                SettingBase.Register(new[] { ShowHintSetting });

                ShowTimersSetting = new TwoButtonsSetting(
                id: 776,
                label: UsefulHints.Instance.Translation.ShowTimers,
                firstOption: UsefulHints.Instance.Translation.ButtonOn,
                secondOption: UsefulHints.Instance.Translation.ButtonOff,
                defaultIsSecond: false,
                hintDescription: UsefulHints.Instance.Translation.ShowTimersDescription,
                onChanged: (player, setting) =>
                {
                    var showTimers = (setting as TwoButtonsSetting)?.IsFirst ?? true;
                    player.SessionVariables["ShowTimers"] = showTimers;
                });
                SettingBase.Register(new[] { ShowTimersSetting });
            }
            if (UsefulHints.Instance.Config.EnableWarnings)
            {
                ShowWarningHintSetting = new TwoButtonsSetting(
                id: 775,
                label: UsefulHints.Instance.Translation.ItemWarningHints,
                firstOption: UsefulHints.Instance.Translation.ButtonOn,
                secondOption: UsefulHints.Instance.Translation.ButtonOff,
                defaultIsSecond: false,
                hintDescription: UsefulHints.Instance.Translation.ItemWarningHintsDescription,
                onChanged: (player, setting) =>
                {
                    var showWarningHintSetting = (setting as TwoButtonsSetting)?.IsFirst ?? true;
                    player.SessionVariables["ShowWarningHints"] = showWarningHintSetting;
                });
                SettingBase.Register(new[] { ShowWarningHintSetting });
            }
            if (UsefulHints.Instance.Config.EnableFfWarning)
            {
                ShowFFWarningSetting = new TwoButtonsSetting(
                id: 774,
                label: UsefulHints.Instance.Translation.FriendlyFireWarning,
                firstOption: UsefulHints.Instance.Translation.ButtonOn,
                secondOption: UsefulHints.Instance.Translation.ButtonOff,
                defaultIsSecond: false,
                hintDescription: UsefulHints.Instance.Translation.FriendlyFireWarningDescription,
                onChanged: (player, setting) =>
                {
                    var showFFWarning = (setting as TwoButtonsSetting)?.IsFirst ?? true;
                    player.SessionVariables["ShowFFWarning"] = showFFWarning;
                });
                SettingBase.Register(new[] { ShowFFWarningSetting });
            }
            if (UsefulHints.Instance.Config.EnableKillCounter)
            {
                ShowKillCounterSetting = new TwoButtonsSetting(
                id: 773,
                label: UsefulHints.Instance.Translation.KillCounter,
                firstOption: UsefulHints.Instance.Translation.ButtonOn,
                secondOption: UsefulHints.Instance.Translation.ButtonOff,
                defaultIsSecond: false,
                hintDescription: UsefulHints.Instance.Translation.KillCounterDescription,
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
            if (UsefulHints.Instance.Config.EnableHints)
            {
                SettingBase.Unregister(settings: new[] { ShowHintSetting });
                SettingBase.Unregister(settings: new[] { ShowTimersSetting });
            }
            if (UsefulHints.Instance.Config.EnableWarnings)
                SettingBase.Unregister(settings: new[] { ShowWarningHintSetting });
            if (UsefulHints.Instance.Config.EnableFfWarning)
                SettingBase.Unregister(settings: new[] { ShowFFWarningSetting });
            if (UsefulHints.Instance.Config.EnableKillCounter)
                SettingBase.Unregister(settings: new[] { ShowKillCounterSetting });
        }
    }
}
