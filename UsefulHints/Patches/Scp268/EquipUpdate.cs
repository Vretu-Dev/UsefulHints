using Exiled.API.Features;
using HarmonyLib;
using InventorySystem.Items.Usables;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Scp268Patch.Patches
{
    // Transpiler: zamienia stałą 15.0 na dynamiczny czas z eventu
    [HarmonyPatch(typeof(Scp268), nameof(Scp268.EquipUpdate))]
    public static class Scp268_EquipUpdate_Transpiler
    {
        private static readonly MethodInfo GetDurationDoubleMI =
            AccessTools.Method(typeof(Scp268_EquipUpdate_Transpiler), nameof(GetDurationDouble));

        private static double GetDurationDouble(Scp268 inst)
        {
            return Scp268Runtime.TryGetDuration(inst, out var seconds) ? seconds : 15.0;
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var replaced = false;

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_R8 && codes[i].operand is double d && System.Math.Abs(d - 15.0) < 0.0001)
                {
                    codes[i] = new CodeInstruction(OpCodes.Ldarg_0);
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, GetDurationDoubleMI));
                    replaced = true;
                    break;
                }
            }

            if (!replaced)
                Log.Warn("[Scp268] Transpiler: nie znaleziono stałej 15.0 w EquipUpdate – sprawdź wersję gry/IL.");

            return codes;
        }
    }
}