using Events.Scp268Item;
using Exiled.API.Features;
using HarmonyLib;
using InventorySystem.Items.Usables;
using System;
using System.Reflection;

namespace Scp268Patch.Patches
{
    [HarmonyPatch(typeof(Scp268), nameof(Scp268.ServerOnUsingCompleted))]
    public static class Scp268_ServerOnUsingCompleted_Patch
    {
        // protected void ServerSetPersonalCooldown(float)
        private static readonly MethodInfo ServerSetPersonalCooldownMI =
            AccessTools.Method(typeof(UsableItem), "ServerSetPersonalCooldown", new[] { typeof(float) });

        static void Postfix(Scp268 __instance)
        {
            float cooldown = 120f;
            float invis = 15f;

            Scp268Events.RaiseUsedScp268(__instance, ref cooldown, ref invis);

            Scp268Runtime.SetDuration(__instance, invis);

            if (Math.Abs(cooldown - 120f) > 0.001f && ServerSetPersonalCooldownMI != null)
            {
                try { ServerSetPersonalCooldownMI.Invoke(__instance, new object[] { cooldown }); }
                catch (Exception ex) { Log.Warn($"Scp268 personal cooldown set failed: {ex}"); }
            }

            Log.Debug($"[Scp268] Used -> invis={invis:0.###}s, cd={cooldown:0.###}s, item={__instance.ItemSerial}");
        }
    }
}