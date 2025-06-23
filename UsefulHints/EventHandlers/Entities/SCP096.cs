using Exiled.Events.EventArgs.Scp096;
using HintServiceMeow.Core.Utilities;
using HintServiceMeow.Core.Models.Hints;
using MEC;
using System;
using UsefulHints.Extensions;
using HintServiceMeow.Core.Extension;

namespace UsefulHints.EventHandlers.Entities
{
    public static class SCP096
    {
        private static Config Config => UsefulHints.Instance.Config;
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
            if (!ev.IsAllowed || ev.Player == null || ev.Target == null || !ServerSettings.ShouldShowHints(ev.Target))
                return;

            var hint = HintService.Scp096LookHint(Config.Scp096LookMessage);

            ev.Target.Display().AddHint(hint);
            ev.Target.Display().RemoveAfter(hint, 5f);
        }
    }
}