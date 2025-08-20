using InventorySystem.Items.Usables;
using System.Runtime.CompilerServices;

namespace Scp268Patch.Patches
{
    internal static class Scp268Runtime
    {
        private sealed class Data { public float Duration; }
        private static readonly ConditionalWeakTable<Scp268, Data> Table = new ConditionalWeakTable<Scp268, Data>();

        public static void SetDuration(Scp268 item, float seconds)
        {
            if (item is null) return;
            Table.Remove(item);
            Table.Add(item, new Data { Duration = seconds });
        }

        public static bool TryGetDuration(Scp268 item, out float seconds)
        {
            seconds = 15f;
            if (item is null) return false;
            if (Table.TryGetValue(item, out var data))
            {
                seconds = data.Duration;
                return true;
            }
            return false;
        }

        public static void Clear(Scp268 item)
        {
            if (item is null) return;
            Table.Remove(item);
        }
    }
}