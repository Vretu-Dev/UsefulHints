using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Exiled.API.Features;
using Exiled.API.Features.Pools;
using HarmonyLib;
using InventorySystem.Items.Jailbird;

namespace UsefulHints
{
    [HarmonyPatch(typeof(JailbirdHitreg), "ServerAttack")]
    public static class Patch
    {
        public static void EnableEffect(HitboxIdentity hitboxIdentity)
        {
            Player val = Player.Get(hitboxIdentity.TargetHub);
            val.EnableEffect(((Plugin<Config>)UsefulHints.Instance).Config.JailbirdEffect, ((Plugin<Config>)UsefulHints.Instance).Config.JailbirdEffectIntensity, ((Plugin<Config>)UsefulHints.Instance).Config.JailbirdEffectDuration, true);
            Log.Debug("Effect Activated");
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            int index = newInstructions.FindLastIndex((CodeInstruction code) => code.opcode == OpCodes.Ldfld && (FieldInfo)code.operand == AccessTools.Field(typeof(ReferenceHub), "playerEffectsController")) + 3;
            newInstructions.InsertRange(index, (IEnumerable<CodeInstruction>)(object)new CodeInstruction[2]
            {
            new CodeInstruction(OpCodes.Ldloc_S, (object)12),
            new CodeInstruction(OpCodes.Call, (object)AccessTools.Method(typeof(Patch), "EnableEffect", new Type[1] { typeof(HitboxIdentity) }, (Type[])null))
            });
            for (int z = 0; z < newInstructions.Count; z++)
            {
                yield return newInstructions[z];
            }
            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
