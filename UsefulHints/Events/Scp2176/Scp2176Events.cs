using Exiled.API.Features;
using InventorySystem.Items.ThrowableProjectiles;
using System;
using System.Linq;

namespace Events.Scp2176
{
    public static class Scp2176Events
    {
        // Event: wywoływany przy wybuchu, przed zaaplikowaniem lockdownu.
        // Pozwala zmodyfikować LockdownDuration.
        public static event Action<UsedScp2176EventArgs> UsedScp2176;

        internal static void RaiseUsedScp2176(Scp2176Projectile proj, ref float lockdownDuration)
        {
            var handler = UsedScp2176;
            if (handler is null)
                return;

            // PreviousOwner.Hub jest używany w stockowym ServerFuseEnd – spróbujmy uzyskać Exiled.Player
            var hub = proj.PreviousOwner.Hub; // może być null
            var player = hub != null ? Player.Get(hub) : null;

            var args = new UsedScp2176EventArgs(player, proj, lockdownDuration, proj.transform.position);

            foreach (var d in handler.GetInvocationList().Cast<Action<UsedScp2176EventArgs>>())
            {
                try { d(args); }
                catch (Exception ex)
                {
                    Log.Warn($"[Scp2176] Used handler exception: {ex}");
                }
            }

            if (args.LockdownDuration < 0f)
                args.LockdownDuration = 0f;

            lockdownDuration = args.LockdownDuration;
        }
    }
}