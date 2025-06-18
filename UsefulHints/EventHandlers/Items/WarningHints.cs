using CustomPlayerEffects;
using Exiled.Events.EventArgs.Player;
using UsefulHints.Extensions;

namespace UsefulHints.EventHandlers.Items
{
    public static class WarningHints
    {
        private static Config Config => UsefulHints.Instance.Config;

        public static void RegisterEvents()
        {
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpWarning;
        }
        public static void UnregisterEvents()
        {
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpWarning;
        }
        private static void OnPickingUpWarning(PickingUpItemEventArgs ev)
        {
            if (!ServerSettings.ShouldShowWarningHints(ev.Player) || ev.Player == null)
                return;

            if (ev.Pickup.Type == ItemType.SCP207 || ev.Pickup.Type == ItemType.AntiSCP207 || ev.Pickup.Type == ItemType.SCP1853)
            {
                if (ev.Player.IsEffectActive<Scp207>() && ev.Pickup.Type != ItemType.SCP207)
                    ev.Player.ShowHint(string.Format(Config.Scp207Warning), 4);

                if (ev.Player.IsEffectActive<AntiScp207>() && ev.Pickup.Type != ItemType.AntiSCP207)
                    ev.Player.ShowHint(string.Format(Config.AntiScp207Warning), 4);

                if (ev.Player.IsEffectActive<Scp1853>() && ev.Pickup.Type != ItemType.SCP1853)
                    ev.Player.ShowHint(string.Format(Config.Scp1853Warning), 4);
            }
        }
    }
}