using Exiled.API.Features.Core.UserSettings;
using Exiled.Events.EventArgs.Scp096;
using UsefulHints.Extensions;

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

            ev.Target.ShowHint(Config.Scp096LookMessage, 5);
        }
    }
}