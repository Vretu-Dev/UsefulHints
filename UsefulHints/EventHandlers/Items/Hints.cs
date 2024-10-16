using System;
using System.Linq;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features.Pickups;
using Player = Exiled.API.Features.Player;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.ThrowableProjectiles;
using MEC;

namespace UsefulHints.EventHandlers.Items
{
    public static class Hints
    {
        private static readonly Dictionary<Player, CoroutineHandle> activeCoroutines = new Dictionary<Player, CoroutineHandle>();
        private static Dictionary<Player, ItemType> activeItems = new Dictionary<Player, ItemType>();
        public static void RegisterEvents()
        {
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpMicroHid;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpSCP207;
            Exiled.Events.Handlers.Player.UsedItem += OnSCP1576Used;
            Exiled.Events.Handlers.Player.ChangedItem += OnSCP1576ChangedItem;
            Exiled.Events.Handlers.Player.UsedItem += OnSCP268Used;
            Exiled.Events.Handlers.Player.InteractingDoor += OnSCP268Interacting;
            Exiled.Events.Handlers.Player.ChangedItem += OnSCP268ChangedItem;
            Exiled.Events.Handlers.Map.ExplodingGrenade += OnSCP2176Grenade;
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpJailbird;
        }
        public static void UnregisterEvents()
        {
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpMicroHid;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpSCP207;
            Exiled.Events.Handlers.Player.UsedItem -= OnSCP1576Used;
            Exiled.Events.Handlers.Player.ChangedItem -= OnSCP1576ChangedItem;
            Exiled.Events.Handlers.Player.UsedItem -= OnSCP268Used;
            Exiled.Events.Handlers.Player.InteractingDoor -= OnSCP268Interacting;
            Exiled.Events.Handlers.Player.ChangedItem -= OnSCP268ChangedItem;
            Exiled.Events.Handlers.Map.ExplodingGrenade -= OnSCP2176Grenade;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpJailbird;
        }
        private static void OnPickingUpMicroHid(PickingUpItemEventArgs ev)
        {
            if (ev.Pickup.Type == ItemType.MicroHID)
            {
                var microHidPickup = ev.Pickup.Base as InventorySystem.Items.MicroHID.MicroHIDPickup;

                if (microHidPickup != null)
                {
                    float energyPercentage = microHidPickup.Energy * 100;
                    float roundedEnergyPercentage = (float)Math.Round(energyPercentage, 1);

                    if (roundedEnergyPercentage < 5)
                    {
                        ev.Player.ShowHint($"<color=red>{string.Format(UsefulHints.Instance.Config.MicroLowEnergyMessage)}</color>", 4);
                    }
                    else
                    {
                        ev.Player.ShowHint($"<color=#4169E1>{string.Format(UsefulHints.Instance.Config.MicroEnergyMessage, roundedEnergyPercentage)}</color>", 4);
                    }
                }
            }
        }
        // SCP 207 Handler
        private static void OnPickingUpSCP207(PickingUpItemEventArgs ev)
        {
            if (ev.Pickup.Type == ItemType.SCP207)
            {
                CustomPlayerEffects.StatusEffectBase scp207Effect = ev.Player.ActiveEffects.FirstOrDefault(effect => effect.GetEffectType() == EffectType.Scp207);

                if (scp207Effect != null)
                {
                    ev.Player.ShowHint($"<color=#A60C0E>{string.Format(UsefulHints.Instance.Config.Scp207HintMessage, scp207Effect.Intensity)}</color>", 4);
                }
            }
            if (ev.Pickup.Type == ItemType.AntiSCP207)
            {
                CustomPlayerEffects.StatusEffectBase antiscp207Effect = ev.Player.ActiveEffects.FirstOrDefault(effect => effect.GetEffectType() == EffectType.AntiScp207);

                if (antiscp207Effect != null)
                {
                    ev.Player.ShowHint($"<color=#2969AD>{string.Format(UsefulHints.Instance.Config.AntiScp207HintMessage, antiscp207Effect.Intensity)}</color>", 4);
                }
            }
        }
        // SCP 1576 Handler
        private static void OnSCP1576Used(UsedItemEventArgs ev)
        {
            if (ev.Item.Type == ItemType.SCP1576)
            {
                if (activeCoroutines.ContainsKey(ev.Player))
                {
                    Timing.KillCoroutines(activeCoroutines[ev.Player]);
                    activeCoroutines.Remove(ev.Player);
                }
                if (activeItems.ContainsKey(ev.Player))
                {
                    activeItems.Remove(ev.Player);
                }

                var coroutine = Timing.RunCoroutine(Scp1576Timer(ev.Player));
                activeCoroutines[ev.Player] = coroutine;
                activeItems[ev.Player] = ev.Item.Type;
            }
        }
        private static void OnSCP1576ChangedItem(ChangedItemEventArgs ev)
        {
            if (activeCoroutines.ContainsKey(ev.Player) && activeItems.ContainsKey(ev.Player) && activeItems[ev.Player] == ItemType.SCP1576)
            {
                Timing.KillCoroutines(activeCoroutines[ev.Player]);
                activeCoroutines.Remove(ev.Player);
                activeItems.Remove(ev.Player);
            }
        }
        private static IEnumerator<float> Scp1576Timer(Player player)
        {
            float duration = 30f;

            while (duration > 0)
            {
                player.ShowHint($"<color=#FFA500>{new string('\n', 10)}{string.Format(UsefulHints.Instance.Config.Scp1576TimeLeftMessage, (int)duration)}</color>", 1.15f);
                yield return Timing.WaitForSeconds(1f);
                duration -= 1f;
            }
            activeCoroutines.Remove(player);
        }
        // SCP 268 Handler
        private static void OnSCP268Used(UsedItemEventArgs ev)
        {
            if (ev.Item.Type == ItemType.SCP268)
            {
                if (activeCoroutines.ContainsKey(ev.Player))
                {
                    Timing.KillCoroutines(activeCoroutines[ev.Player]);
                    activeCoroutines.Remove(ev.Player);
                }
                if (activeItems.ContainsKey(ev.Player))
                {
                    activeItems.Remove(ev.Player);
                }

                var coroutine = Timing.RunCoroutine(Scp268Timer(ev.Player));
                activeCoroutines.Add(ev.Player, coroutine);
                activeItems.Add(ev.Player, ev.Item.Type);
            }
        }
        private static void OnSCP268Interacting(InteractingDoorEventArgs ev)
        {
            if (activeCoroutines.ContainsKey(ev.Player) && activeItems.ContainsKey(ev.Player) && activeItems[ev.Player] == ItemType.SCP268)
            {
                Timing.KillCoroutines(activeCoroutines[ev.Player]);
                activeCoroutines.Remove(ev.Player);
                activeItems.Remove(ev.Player);
            }
        }
        private static void OnSCP268ChangedItem(ChangedItemEventArgs ev)
        {
            if (activeCoroutines.ContainsKey(ev.Player) && activeItems.ContainsKey(ev.Player) && activeItems[ev.Player] == ItemType.SCP268)
            {
                Timing.KillCoroutines(activeCoroutines[ev.Player]);
                activeCoroutines.Remove(ev.Player);
                activeItems.Remove(ev.Player);
            }
        }
        private static IEnumerator<float> Scp268Timer(Player player)
        {
            float duration = UsefulHints.Instance.Config.Scp268Duration;

            while (duration > 0)
            {
                player.ShowHint($"<color=purple>{new string('\n', 10)}{string.Format(UsefulHints.Instance.Config.Scp268TimeLeftMessage, (int)duration)}</color>", 1.15f);
                yield return Timing.WaitForSeconds(1f);
                duration -= 1f;
            }
            activeCoroutines.Remove(player);
        }
        // SCP 2176 Handler
        private static void OnSCP2176Grenade(ExplodingGrenadeEventArgs ev)
        {
            if (ev.Projectile.Base is Scp2176Projectile)
            {
                if (ev.Player != null)
                {
                    if (activeCoroutines.ContainsKey(ev.Player))
                    {
                        Timing.KillCoroutines(activeCoroutines[ev.Player]);
                        activeCoroutines.Remove(ev.Player);
                    }

                    var coroutine = Timing.RunCoroutine(Scp2176Timer(ev.Player));
                    activeCoroutines.Add(ev.Player, coroutine);
                }
            }
        }
        private static IEnumerator<float> Scp2176Timer(Player player)
        {
            float duration = 13f;

            while (duration > 0)
            {
                player.ShowHint($"<color=#1CAA21>{new string('\n', 10)}{string.Format(UsefulHints.Instance.Config.Scp2176TimeLeftMessage, (int)duration)}</color>", 1.15f);
                yield return Timing.WaitForSeconds(1f);
                duration -= 1f;
            }
            activeCoroutines.Remove(player);
        }
        // Reset Coroutines
        private static void OnWaitingForPlayers()
        {
            activeCoroutines.Clear();
        }
        // Jailbird Handler
        private static void OnPickingUpJailbird(PickingUpItemEventArgs ev)
        {
            if (ev.Pickup is JailbirdPickup jailbirdPickup)
            {
                int maxCharges = 5;
                int remainingCharges = maxCharges - jailbirdPickup.TotalCharges;
                if (remainingCharges > 1)
                {
                    ev.Player.ShowHint($"<color=#00B7EB>{string.Format(UsefulHints.Instance.Config.JailbirdUseMessage, remainingCharges)}</color>", 4);
                }
                else
                {
                    ev.Player.ShowHint($"<color=#C73804>{string.Format(UsefulHints.Instance.Config.JailbirdUseMessage, remainingCharges)}</color>", 4);
                }
            }
        }
    }
}