using System;
using Exiled.API.Features;
using InventorySystem.Items.Usables.Scp1576;

namespace Events.Scp1576
{
    public sealed class StoppingTransmissionEventArgs : EventArgs
    {
        public StoppingTransmissionEventArgs(Player player, Scp1576Item item, float cooldown)
        {
            Player = player;
            Item = item;
            Cooldown = cooldown;
        }
        public Player Player { get; }
        public Scp1576Item Item { get; }
        public float Cooldown { get; set; }
    }
}