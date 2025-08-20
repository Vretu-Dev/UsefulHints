using Events.Scp2176;
using HarmonyLib;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;

namespace Scp2176Patch.Patches
{
    [HarmonyPatch(typeof(Scp2176Projectile), nameof(Scp2176Projectile.ServerShatter))]
    public static class Scp2176Projectile_ServerShatter_Patch
    {
        static void Prefix(Scp2176Projectile __instance)
        {
            if (!NetworkServer.active)
                return;

            float duration = __instance.LockdownDuration;
            Scp2176Events.RaiseUsedScp2176(__instance, ref duration);
            __instance.LockdownDuration = duration;
        }
    }
}