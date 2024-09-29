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
            ev.Target.ShowHint(UsefulHints.Instance.Config.Scp096LookMessage, 5);
        }
    }
}