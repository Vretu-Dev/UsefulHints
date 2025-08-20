using Exiled.API.Features;
using InventorySystem.Items.Usables.Scp1576;
using System;
using System.Linq;

namespace Events.Scp1576
{
    public static class Scp1576Events
    {
        public static event Action<StoppingTransmissionEventArgs> StoppingTransmission;

        internal static void RaiseStoppingTransmission(Scp1576Item item, ref float cooldown)
        {
            var handler = StoppingTransmission;
            if (handler is null)
                return;

            var player = Player.Get(item.Owner);
            var args = new StoppingTransmissionEventArgs(player, item, cooldown);

            foreach (var d in handler.GetInvocationList().Cast<Action<StoppingTransmissionEventArgs>>())
            {
                try { d(args); }
                catch (Exception ex)
                {
                    Log.Warn($"Scp1576 StoppingTransmission handler exception: {ex}");
                }
            }

            cooldown = args.Cooldown;
        }
    }
}