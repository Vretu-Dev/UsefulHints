using Exiled.Events.EventArgs.Scp096;
using HintServiceMeow.Core.Utilities;
using HintServiceMeow.Core.Models.Hints;
using MEC;
using System;

namespace UsefulHints.EventHandlers.Entities
{
    public static class SCP096
    {
        public static void RegisterEvents()
        {
            Exiled.Events.Handlers.Scp096.AddingTarget += OnScp096AddingTarget;
        }
        public static void UnregisterEvents()
        {
            Exiled.Events.Handlers.Scp096.AddingTarget -= OnScp096AddingTarget;
        }
        private static void OnScp096AddingTarget(AddingTargetEventArgs ev)
        {
            if (!ev.IsAllowed || ev.Player == null || ev.Target == null)
                return;

            var hint = new DynamicHint
            {
                Text = UsefulHints.Instance.Config.Scp096LookMessage,
                TargetY = 600,
                FontSize = 32,
            };

            PlayerDisplay playerDisplay = PlayerDisplay.Get(ev.Target);
            playerDisplay.AddHint(hint);
            Timing.CallDelayed(5f, () => { playerDisplay.RemoveHint(hint); });
        }
    }
}