using System.Linq;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using MEC;

namespace UsefulHints.EventHandlers.Items
{
    public static class WarningHints
    {
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
            if (ev.Pickup.Type == ItemType.SCP207)
            {
                if (ev.Player.IsEffectActive<AntiScp207>())
                {
                    ev.Player.ShowHint(string.Format(UsefulHints.Instance.Config.AntiScp207Warning), 4);
                }
                if (ev.Player.IsEffectActive<Scp1853>())
                {
                    ev.Player.ShowHint(string.Format(UsefulHints.Instance.Config.Scp1853Warning), 4);
                }
            }
            if (ev.Pickup.Type == ItemType.AntiSCP207)
            {
                if (ev.Player.IsEffectActive<Scp207>())
                {
                    ev.Player.ShowHint(string.Format(UsefulHints.Instance.Config.Scp207Warning), 4);
                }
                if (ev.Player.IsEffectActive<Scp1853>())
                {
                    ev.Player.ShowHint(string.Format(UsefulHints.Instance.Config.Scp1853Warning), 4);
                }
            }
            if (ev.Pickup.Type == ItemType.SCP1853)
            {
                if (ev.Player.IsEffectActive<Scp207>())
                {
                    ev.Player.ShowHint(string.Format(UsefulHints.Instance.Config.Scp207Warning), 4);
                }
                if (ev.Player.IsEffectActive<AntiScp207>())
                {
                    ev.Player.ShowHint(string.Format(UsefulHints.Instance.Config.AntiScp207Warning), 4);
                }
            }
        }
    }
}