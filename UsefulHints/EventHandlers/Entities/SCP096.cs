using LabApi.Events.Arguments.Scp096Events;

namespace UsefulHints.EventHandlers.Entities
{
    public static class SCP096
    {
        public static void RegisterEvents()
        {
            LabApi.Events.Handlers.Scp096Events.AddingTarget += OnScp096AddingTarget;
        }
        public static void UnregisterEvents()
        {
            LabApi.Events.Handlers.Scp096Events.AddingTarget -= OnScp096AddingTarget;
        }
        private static void OnScp096AddingTarget(Scp096AddingTargetEventArgs ev)
        {
            ev.Target.SendHint(UsefulHints.Instance.Config.Scp096LookMessage, 5);
        }
    }
}