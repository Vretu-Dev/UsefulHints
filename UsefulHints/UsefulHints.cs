using System;
using System.Collections.Generic;
using Exiled.API.Features;
using Player = Exiled.API.Features.Player;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp096;
using InventorySystem.Items.ThrowableProjectiles;
using MEC;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using HarmonyLib;

namespace UsefulHints
{
    public class UsefulHints : Plugin<Config>
    {
        public override string Name => "Useful Hints";
        public override string Author => "Vretu";
        public override string Prefix { get; } = "UH";
        public override Version Version => new Version(1, 2, 0);
        public override Version RequiredExiledVersion { get; } = new Version(8, 9, 8);
        public static UsefulHints Instance { get; private set; }
        public Harmony Harmony { get; private set; }
        public string HarmonyName { get; private set; }

        private readonly Dictionary<Player, CoroutineHandle> activeCoroutines = new Dictionary<Player, CoroutineHandle>();
        private readonly Dictionary<Player, int> playerKills = new Dictionary<Player, int>();

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Scp096.AddingTarget += OnScp096AddingTarget;
            Exiled.Events.Handlers.Player.UsedItem += OnItemUsed;
            Exiled.Events.Handlers.Map.ExplodingGrenade += OnExplodingGrenade;
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpJailbird;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpSCP207;
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
            Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
            Exiled.Events.Handlers.Player.Died += OnPlayerDied;
            if (Config.EnableCustomJailbirdSettings)
            {
                Instance = this;
                HarmonyName = $"com-vretu.uh-{DateTime.UtcNow.Ticks}";
                Harmony = new Harmony(HarmonyName);
                Harmony.PatchAll();
            }
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Scp096.AddingTarget -= OnScp096AddingTarget;
            Exiled.Events.Handlers.Player.UsedItem -= OnItemUsed;
            Exiled.Events.Handlers.Map.ExplodingGrenade -= OnExplodingGrenade;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpJailbird;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpSCP207;
            Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
            Exiled.Events.Handlers.Player.ChangedItem -= OnChangedItem;
            Exiled.Events.Handlers.Player.Died -= OnPlayerDied;
            if (Config.EnableCustomJailbirdSettings)
            {
                Harmony.UnpatchAll(HarmonyName);
            }
            base.OnDisabled();
        }
        // SCP 207 Handler
        private void OnPickingUpSCP207(PickingUpItemEventArgs ev)
        {
            if (ev.Pickup.Type == ItemType.SCP207)
            {
                CustomPlayerEffects.StatusEffectBase scp207Effect = ev.Player.ActiveEffects.FirstOrDefault(effect => effect.GetEffectType() == EffectType.Scp207);

                if (scp207Effect != null)
                {
                    ev.Player.ShowHint($"<color=#A60C0E>{string.Format(Config.Scp207HintMessage, scp207Effect.Intensity)}</color>", 4);
                }
            }
        }
        // SCP 096 Handler
        private void OnScp096AddingTarget(AddingTargetEventArgs ev)
        {
            ev.Target.ShowHint(Config.Scp096LookMessage, 5);
        }
        // SCP 268 Handler
        private Dictionary<Player, ItemType> activeItems = new Dictionary<Player, ItemType>();
        private void OnItemUsed(UsedItemEventArgs ev)
        {
            if (ev.Item.Type == ItemType.SCP268)
            {
                if (activeCoroutines.ContainsKey(ev.Player))
                {
                    Timing.KillCoroutines(activeCoroutines[ev.Player]);
                    activeCoroutines.Remove(ev.Player);
                }

                var coroutine = Timing.RunCoroutine(Scp268Timer(ev.Player));
                activeCoroutines.Add(ev.Player, coroutine);
                activeItems.Add(ev.Player, ev.Item.Type);
            }
        }
        private void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (activeCoroutines.ContainsKey(ev.Player) && activeItems.ContainsKey(ev.Player) && activeItems[ev.Player] == ItemType.SCP268)
            {
                Timing.KillCoroutines(activeCoroutines[ev.Player]);
                activeCoroutines.Remove(ev.Player);
                activeItems.Remove(ev.Player);
            }
        }
        private void OnChangedItem(ChangedItemEventArgs ev)
        {
            if (activeCoroutines.ContainsKey(ev.Player) && activeItems.ContainsKey(ev.Player) && activeItems[ev.Player] == ItemType.SCP268)
            {
                Timing.KillCoroutines(activeCoroutines[ev.Player]);
                activeCoroutines.Remove(ev.Player);
                activeItems.Remove(ev.Player);
            }
        }
        // SCP 2176 Handler
        private void OnExplodingGrenade(ExplodingGrenadeEventArgs ev)
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
        // Timers for SCP 268 & SCP 2176
        private IEnumerator<float> Scp268Timer(Player player)
        {
            float duration = Config.Scp268Duration;

            while (duration > 0)
            {
                player.ShowHint($"<color=purple>{string.Format(Config.Scp268TimeLeftMessage, (int)duration)}</color>", 1.15f);
                yield return Timing.WaitForSeconds(1f);
                duration -= 1f;
            }
            activeCoroutines.Remove(player);
        }
        private IEnumerator<float> Scp2176Timer(Player player)
        {
            float duration = 13f;

            while (duration > 0)
            {
                player.ShowHint($"<color=green>{string.Format(Config.Scp2176TimeLeftMessage, (int)duration)}</color>", 1.15f);
                yield return Timing.WaitForSeconds(1f);
                duration -= 1f;
            }
            activeCoroutines.Remove(player);
        }

        private void OnWaitingForPlayers()
        {
            activeCoroutines.Clear();
        }
        // Jailbird Handler
        private void OnPickingUpJailbird(PickingUpItemEventArgs ev)
        {
            if (ev.Pickup is JailbirdPickup jailbirdPickup)
            {
                int remainingCharges = jailbirdPickup.TotalCharges;
                ev.Player.ShowHint(string.Format(Config.JailbirdUseMessage, remainingCharges), 4);
            }
        }
        // Kill Counter Handler
        private void OnPlayerDied(DiedEventArgs ev)
        {
            if (ev.Attacker != null)
            {
                Player killer = ev.Attacker;

                if (playerKills.ContainsKey(killer))
                {
                    playerKills[killer]++;
                }
                else
                {
                    playerKills[killer] = 1;
                }
                killer.ShowHint(string.Format(Config.KillCountMessage, playerKills[killer]), 4);
            }
        }
    }
}