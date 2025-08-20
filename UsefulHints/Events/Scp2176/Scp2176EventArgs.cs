using System;
using Exiled.API.Features;
using InventorySystem.Items.ThrowableProjectiles;
using UnityEngine;

namespace Events.Scp2176
{
    public sealed class UsedScp2176EventArgs : EventArgs
    {
        public UsedScp2176EventArgs(Player player, Scp2176Projectile projectile, float lockdownDuration, Vector3 position)
        {
            Player = player;
            Projectile = projectile;
            LockdownDuration = lockdownDuration;
            Position = position;
        }

        public Player Player { get; }
        public Scp2176Projectile Projectile { get; }
        public float LockdownDuration { get; set; }
        public Vector3 Position { get; }
    }
}