﻿using CustomPlayerEffects;
using Exiled.Events.EventArgs.Player;
using HintServiceMeow.Core.Utilities;
using HintServiceMeow.Core.Models.Hints;
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
            PlayerDisplay playerDisplay = PlayerDisplay.Get(ev.Player);

            var SCP207Hint = new Hint
            {
                Text = string.Format(UsefulHints.Instance.Config.AntiScp207Warning),
                YCoordinate = 700,
                FontSize = 32,
            };
            var SCP1853Hint = new Hint
            {
                Text = string.Format(UsefulHints.Instance.Config.Scp1853Warning),
                YCoordinate = 700,
                FontSize = 32,
            };
            var AntiSCP207Hint = new Hint
            {
                Text = string.Format(UsefulHints.Instance.Config.AntiScp207Warning),
                YCoordinate = 700,
                FontSize = 32,
            };

            if (ev.Pickup.Type == ItemType.SCP207)
            {
                if (ev.Player.IsEffectActive<AntiScp207>())
                {
                    playerDisplay.AddHint(AntiSCP207Hint);
                    Timing.CallDelayed(4f, () => { playerDisplay.RemoveHint(AntiSCP207Hint); });
                }
                if (ev.Player.IsEffectActive<Scp1853>())
                {
                    playerDisplay.AddHint(SCP1853Hint);
                    Timing.CallDelayed(4f, () => { playerDisplay.RemoveHint(SCP1853Hint); });
                }
            }
            if (ev.Pickup.Type == ItemType.AntiSCP207)
            {
                if (ev.Player.IsEffectActive<Scp207>())
                {
                    playerDisplay.AddHint(SCP207Hint);
                    Timing.CallDelayed(4f, () => { playerDisplay.RemoveHint(SCP207Hint); });
                }
                if (ev.Player.IsEffectActive<Scp1853>())
                {
                    playerDisplay.AddHint(SCP1853Hint);

                    Timing.CallDelayed(4f, () => {
                        playerDisplay.RemoveHint(SCP1853Hint);
                    });
                }
            }
            if (ev.Pickup.Type == ItemType.SCP1853)
            {
                if (ev.Player.IsEffectActive<Scp207>())
                {
                    playerDisplay.AddHint(SCP207Hint);
                    Timing.CallDelayed(4f, () => { playerDisplay.RemoveHint(SCP207Hint); });
                }
                if (ev.Player.IsEffectActive<AntiScp207>())
                {
                    playerDisplay.AddHint(AntiSCP207Hint);
                    Timing.CallDelayed(4f, () => { playerDisplay.RemoveHint(AntiSCP207Hint); });
                }
            }
        }
    }
}