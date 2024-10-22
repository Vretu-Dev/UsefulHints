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
        private static Dictionary<Player, ItemType> activeItems = new Dictionary<Player, ItemType>();
        private static Dictionary<Player, CoroutineHandle> activeCoroutines = new Dictionary<Player, CoroutineHandle>();

        public static void Postfix(Scp1576Item __instance)
        {
            Player player = Player.Get(__instance.Owner);

            if (activeCoroutines.ContainsKey(player) && activeItems.ContainsKey(player) && activeItems[player] == ItemType.SCP1576)
            {
                Timing.KillCoroutines(activeCoroutines[player]);
                activeCoroutines.Remove(player);
                activeItems.Remove(player);
            }
        }
    }
}
