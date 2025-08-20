using HarmonyLib;
using InventorySystem.Items.Usables;

namespace Scp268Patch.Patches
{
    // Sprzątanie runtime przy zakończeniu niewidki
    [HarmonyPatch(typeof(Scp268), nameof(Scp268.SetState))]
    public static class Scp268_SetState_Patch
    {
        static void Postfix(Scp268 __instance, bool state)
        {
            if (!state)
            {
                Scp268Runtime.Clear(__instance);
            }
        }
    }
}