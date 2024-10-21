using HarmonyLib;
using System.Collections.Generic;
using MEC;
using Exiled.API.Features;
using InventorySystem.Items.Usables.Scp1576;

namespace UsefulHints.Patches
{
    [HarmonyPatch(typeof(Scp1576Item), nameof(Scp1576Item.ServerStopTransmitting))]
    public static class Scp1576StopTransmissionPatch
    {
        private static Dictionary<Player, CoroutineHandle> activeCoroutines = new Dictionary<Player, CoroutineHandle>();

        public static void Postfix(Scp1576Item __instance)
        {
            Player player = Player.Get(__instance.Owner);

            if (activeCoroutines.ContainsKey(player))
            {
                Timing.KillCoroutines(activeCoroutines[player]);
                activeCoroutines.Remove(player);
            }
        }
        public static void StartTransmissionCoroutine(Player player, CoroutineHandle coroutine)
        {
            if (activeCoroutines.ContainsKey(player))
            {
                Timing.KillCoroutines(activeCoroutines[player]);
            }
            activeCoroutines[player] = coroutine;
        }
    }
}
