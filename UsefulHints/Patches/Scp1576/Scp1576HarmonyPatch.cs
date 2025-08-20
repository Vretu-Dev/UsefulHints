using Events.Scp1576;
using Exiled.API.Features;
using HarmonyLib;
using InventorySystem.Items.Usables;
using InventorySystem.Items.Usables.Scp1576;
using Mirror;
using System;
using System.Diagnostics;
using System.Reflection;
using Utils.Networking;

namespace Patches.Scp1576StopTransmittingPatch
{
    [HarmonyPatch(typeof(Scp1576Item), nameof(Scp1576Item.ServerStopTransmitting))]
    public static class Scp1576Item_ServerStopTransmitting_Patch
    {
        // protected void ServerSetGlobalItemCooldown(float)
        private static readonly MethodInfo ServerSetGlobalItemCooldownMI =
            AccessTools.Method(typeof(UsableItem), "ServerSetGlobalItemCooldown", new[] { typeof(float) });

        // private readonly Stopwatch _useStopwatch;
        private static readonly FieldInfo UseStopwatchFI =
            AccessTools.Field(typeof(Scp1576Item), "_useStopwatch");

        static bool Prefix(Scp1576Item __instance)
        {
            if (!NetworkServer.active)
                return true;

            float cooldown = Scp1576Item.UseCooldown;

            Scp1576Events.RaiseStoppingTransmission(__instance, ref cooldown);

            var sw = UseStopwatchFI?.GetValue(__instance) as Stopwatch;
            sw?.Reset();

            var pec = __instance.Owner?.playerEffectsController;
            var effect = pec?.GetEffect<CustomPlayerEffects.Scp1576>();
            if (effect != null)
                effect.IsEnabled = false;


            if (ServerSetGlobalItemCooldownMI != null)
            {
                try
                {
                    ServerSetGlobalItemCooldownMI.Invoke(__instance, new object[] { cooldown });
                }
                catch (Exception ex)
                {
                    Log.Warn($"Scp1576 global cooldown set failed: {ex}");
                }
            }

            Scp1576SpectatorWarningHandler.TriggerStop(__instance);

            new StatusMessage(StatusMessage.StatusType.Cancel, __instance.ItemSerial).SendToAuthenticated();

            var conn = __instance.Owner?.connectionToClient;
            if (conn != null)
                conn.Send(new ItemCooldownMessage(__instance.ItemSerial, cooldown));

            return false;
        }
    }
}