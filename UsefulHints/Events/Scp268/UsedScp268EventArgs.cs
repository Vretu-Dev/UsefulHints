using Exiled.API.Features;
using InventorySystem.Items.Usables;
using System;

namespace Events.Scp268Item
{
    public sealed class UsedScp268EventArgs : EventArgs
    {
        public UsedScp268EventArgs(Player player, Scp268 item, float cooldown, float invisibilityTime)
        {
            Player = player;
            Item = item;
            Cooldown = cooldown;
            InvisibilityTime = invisibilityTime;
        }

        public Player Player { get; }
        public Scp268 Item { get; }
        public float Cooldown { get; set; }
        public float InvisibilityTime { get; set; }
    }
}