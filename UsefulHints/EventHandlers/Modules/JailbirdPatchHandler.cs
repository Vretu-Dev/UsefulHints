using HarmonyLib;
using System;

namespace UsefulHints.EventHandlers.Modules
{
    public static class JailbirdPatchHandler
    {
        public static Harmony Harmony { get; private set; }
        public static string HarmonyName { get; private set; }
        public static void RegisterEvents()
        {
            HarmonyName = $"com-vretu.uh-{DateTime.UtcNow.Ticks}";
            Harmony = new Harmony(HarmonyName);
            Harmony.PatchAll();
        }
        public static void UnregisterEvents()
        {
            Harmony.UnpatchAll(HarmonyName);
        }
    }
}