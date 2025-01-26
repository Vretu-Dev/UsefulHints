using Exiled.API.Features.Core.UserSettings;
using Exiled.Events.EventArgs.Scp096;

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
            if (ev.Target.SessionVariables.TryGetValue("ShowHints", out var showHints) && !(bool)showHints)
                return;

            ev.Target.ShowHint(UsefulHints.Instance.Config.Scp096LookMessage, 5);
        }
    }
}