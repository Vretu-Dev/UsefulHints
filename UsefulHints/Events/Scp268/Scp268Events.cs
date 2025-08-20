using System;
using System.Linq;
using Exiled.API.Features;
using InventorySystem.Items.Usables;

namespace Events.Scp268Item
{
    public static class Scp268Events
    {
        public static event Action<UsedScp268EventArgs> UsedScp268;

        internal static void RaiseUsedScp268(Scp268 item, ref float cooldown, ref float invisibilityTime)
        {
            var handler = UsedScp268;
            if (handler is null)
                return;

            var player = Player.Get(item.Owner);
            var args = new UsedScp268EventArgs(player, item, cooldown, invisibilityTime);

            foreach (var d in handler.GetInvocationList().Cast<Action<UsedScp268EventArgs>>())
            {
                try { d(args); }
                catch (Exception ex)
                {
                    Log.Warn($"Scp268 Used handler exception: {ex}");
                }
            }

            if (args.Cooldown < 0f) args.Cooldown = 0f;
            if (args.InvisibilityTime < 0f) args.InvisibilityTime = 0f;

            cooldown = args.Cooldown;
            invisibilityTime = args.InvisibilityTime;
        }
    }
}