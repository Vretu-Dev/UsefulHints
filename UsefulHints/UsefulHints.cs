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

namespace UsefulHints
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "Useful_Hints";
        public override string Author => "Vretu";
        public override string Prefix { get; } = "UH";
        public override Version Version => new Version(1, 0, 0);
        public override Version RequiredExiledVersion { get; } = new Version(8, 9, 8);

        private readonly Dictionary<Player, CoroutineHandle> activeCoroutines = new Dictionary<Player, CoroutineHandle>();

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Scp096.AddingTarget += OnScp096AddingTarget;
            Exiled.Events.Handlers.Player.UsedItem += OnItemUsed;
            Exiled.Events.Handlers.Map.ExplodingGrenade += OnExplodingGrenade;
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpItem;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Scp096.AddingTarget -= OnScp096AddingTarget;
            Exiled.Events.Handlers.Player.UsedItem -= OnItemUsed;
            Exiled.Events.Handlers.Map.ExplodingGrenade -= OnExplodingGrenade;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpItem;
            base.OnDisabled();
        }

        private void OnScp096AddingTarget(AddingTargetEventArgs ev)
        {
            ev.Target.ShowHint(Config.Scp096LookMessage, 5);
        }

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
            }
        }

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

        private IEnumerator<float> Scp268Timer(Player player)
        {
            float duration = Config.Scp268Duration;

            while (duration > 0)
            {
                player.ShowHint($"<color=purple>{string.Format(Config.Scp268TimeLeftMessage, (int)duration)}</color>", 1);
                yield return Timing.WaitForSeconds(1f);
                duration -= 1f;
            }
            activeCoroutines.Remove(player);
        }

        private IEnumerator<float> Scp2176Timer(Player player)
        {
            float duration = 13f;  // 13 sekund

            while (duration > 0)
            {
                player.ShowHint($"<color=green>{string.Format(Config.Scp2176TimeLeftMessage, (int)duration)}</color>", 1);
                yield return Timing.WaitForSeconds(1f);
                duration -= 1f;
            }
            activeCoroutines.Remove(player);
        }

        private void OnWaitingForPlayers()
        {
            activeCoroutines.Clear();
        }

        private void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (ev.Pickup is JailbirdPickup jailbirdPickup)
            {
                int remainingCharges = jailbirdPickup.TotalCharges;
                ev.Player.ShowHint(string.Format(Config.JailbirdUseMessage, remainingCharges), 4);
            }
        }
    }
}
