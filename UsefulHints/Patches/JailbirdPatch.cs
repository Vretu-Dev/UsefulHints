using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Exiled.API.Features;
using Exiled.API.Features.Core.Generic.Pools;
using HarmonyLib;
using InventorySystem.Items.Jailbird;

namespace UsefulHints.Patches
{
    [HarmonyPatch(typeof(JailbirdHitreg), "ServerAttack")]
    public static class JailbirdPatch
    {
        public static void EnableEffect(HitboxIdentity hitboxIdentity)
        {
            Player val = Player.Get(hitboxIdentity.TargetHub);
            val.EnableEffect(UsefulHints.Instance.Config.JailbirdEffect, UsefulHints.Instance.Config.JailbirdEffectIntensity, UsefulHints.Instance.Config.JailbirdEffectDuration, true);
            Log.Debug("Effect Activated");
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            int index = newInstructions.FindLastIndex((CodeInstruction code) => code.opcode == OpCodes.Ldfld && (FieldInfo)code.operand == AccessTools.Field(typeof(ReferenceHub), "playerEffectsController")) + 3;
            newInstructions.InsertRange(index, new CodeInstruction[2]
            {
            new CodeInstruction(OpCodes.Ldloc_S, 12),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(JailbirdPatch), nameof(EnableEffect), new Type[1] { typeof(HitboxIdentity) }))
            });
            for (int z = 0; z < newInstructions.Count; z++)
            {
                yield return newInstructions[z];
            }
            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}