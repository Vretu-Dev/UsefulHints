using CustomPlayerEffects;
using Exiled.Events.EventArgs.Player;
using HintServiceMeow.Core.Extension;
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
                {
                    var hint = HintService.Scp207WarningHint(Config.Scp207Warning);
                    ev.Player.Display().AddHint(hint);
                    ev.Player.Display().RemoveAfter(hint, 4f);
                }

                if (ev.Player.IsEffectActive<AntiScp207>() && ev.Pickup.Type != ItemType.AntiSCP207)
                {
                    var hint = HintService.AntiScp207WarningHint(Config.AntiScp207Warning);
                    ev.Player.Display().AddHint(hint);
                    ev.Player.Display().RemoveAfter(hint, 4f);
                }

                if (ev.Player.IsEffectActive<Scp1853>() && ev.Pickup.Type != ItemType.SCP1853)
                {
                    var hint = HintService.Scp1853WarningHint(Config.Scp1853Warning);
                    ev.Player.Display().AddHint(hint);
                    ev.Player.Display().RemoveAfter(hint, 4f);
                }
            }
        }
    }
}