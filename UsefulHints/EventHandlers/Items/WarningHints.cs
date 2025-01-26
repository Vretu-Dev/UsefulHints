using CustomPlayerEffects;
using Exiled.API.Features.Core.UserSettings;
using Exiled.Events.EventArgs.Player;

namespace UsefulHints.EventHandlers.Items
{
    public static class WarningHints
    {
        public static TwoButtonsSetting ShowWarningHintSetting { get; private set; }
        public static void RegisterEvents()
        {
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpWarning;

            ShowWarningHintSetting = new TwoButtonsSetting(
            id: 775,
            label: "Item Warning Hints",
            firstOption: "ON",
            secondOption: "OFF",
            defaultIsSecond: false,
            hintDescription: "Prevent use SCP-207, Anti SCP-207 and SCP-1853 together.",
            onChanged: (player, setting) =>
            {
                var showWarningHintSetting = (setting as TwoButtonsSetting)?.IsFirst ?? true;
                player.SessionVariables["ShowWarningHints"] = showWarningHintSetting;
            });
            SettingBase.Register(new[] { ShowWarningHintSetting });
        }
        public static void UnregisterEvents()
        {
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpWarning;
            SettingBase.Unregister(settings: new[] { ShowWarningHintSetting });
        }
        private static void OnPickingUpWarning(PickingUpItemEventArgs ev)
        {
            if (ev.Player.SessionVariables.TryGetValue("ShowWarningHints", out var showWarningHintSetting) && !(bool)showWarningHintSetting)
                return;

            if (ev.Pickup.Type == ItemType.SCP207 || ev.Pickup.Type == ItemType.AntiSCP207 || ev.Pickup.Type == ItemType.SCP1853)
            {
                if (ev.Player.IsEffectActive<Scp207>() && ev.Pickup.Type != ItemType.SCP207)
                    ev.Player.ShowHint(string.Format(UsefulHints.Instance.Config.Scp207Warning), 4);

                if (ev.Player.IsEffectActive<AntiScp207>() && ev.Pickup.Type != ItemType.AntiSCP207)
                    ev.Player.ShowHint(string.Format(UsefulHints.Instance.Config.AntiScp207Warning), 4);

                if (ev.Player.IsEffectActive<Scp1853>() && ev.Pickup.Type != ItemType.SCP1853)
                    ev.Player.ShowHint(string.Format(UsefulHints.Instance.Config.Scp1853Warning), 4);
            }
        }
    }
}