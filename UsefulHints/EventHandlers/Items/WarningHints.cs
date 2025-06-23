using CustomPlayerEffects;
using LabApi.Events.Arguments.PlayerEvents;

namespace UsefulHints.EventHandlers.Items
{
    public static class WarningHints
    {
        public static void RegisterEvents()
        {
            LabApi.Events.Handlers.PlayerEvents.PickingUpItem += OnPickingUpWarning;
        }
        public static void UnregisterEvents()
        {
            LabApi.Events.Handlers.PlayerEvents.PickingUpItem -= OnPickingUpWarning;
        }

        private static void OnPickingUpWarning(PlayerPickingUpItemEventArgs ev)
        {
            if (ev.Player == null)
                return;

            if (ev.Pickup.Type == ItemType.SCP207 || ev.Pickup.Type == ItemType.AntiSCP207 || ev.Pickup.Type == ItemType.SCP1853)
            {
                if (ev.Player.HasEffect<Scp207>() && ev.Pickup.Type != ItemType.SCP207)
                    ev.Player.SendHint(string.Format(UsefulHints.Instance.Config.Scp207Warning), 4);

                if (ev.Player.HasEffect<AntiScp207>() && ev.Pickup.Type != ItemType.AntiSCP207)
                    ev.Player.SendHint(string.Format(UsefulHints.Instance.Config.AntiScp207Warning), 4);

                if (ev.Player.HasEffect<Scp1853>() && ev.Pickup.Type != ItemType.SCP1853)
                    ev.Player.SendHint(string.Format(UsefulHints.Instance.Config.Scp1853Warning), 4);
            }
        }
    }
}