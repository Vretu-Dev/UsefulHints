
![UsefulHints](https://github.com/user-attachments/assets/a01fc940-f540-4c8b-8caf-65848a22335d)<br><br><br>
[![downloads](https://img.shields.io/github/downloads/JustVretu/UsefulHints-LabAPI/total?style=for-the-badge&logo=icloud&color=%233A6D8C)](https://github.com/JustVretu/UsefulHints-LabAPI/releases/latest)
![Latest](https://img.shields.io/github/v/release/Vretu-Dev/UsefulHints?style=for-the-badge&label=Latest%20Release&color=%23D91656)

## Downloads:
| Framework | Version    |  Release                                                              |
|:---------:|:----------:|:----------------------------------------------------------------------:|
| Exiled    | ≥ 9.6.0    | [⬇️](https://github.com/Vretu-Dev/UsefulHints/releases/latest)        |
| LabAPI    | 1.0.2      | [⬇️](https://github.com/JustVretu/UsefulHints-LabAPI/releases/latest) |

## Features:
- SCP-268 countdown timer
- SCP-1576 countdown timer
- SCP-2176 lockdown timer
- Hint displayed when looking at SCP-096's face
- Track remaining Jailbird charges
- Display SCP-207 & Anti SCP-207 doses when picked up
- Warning hints when affected by SCP-207, Anti SCP-207, or SCP-1853
- Friendly fire warnings when damaging teammates
- Kill counter for players
- End-of-game summary
- Show teammates at the start of the round
- Broadcast the last human alive

## Support:
<h4>UCS Independent Developer</h4>
<a href='https://opencollective.com/ucs'><img src="https://github.com/user-attachments/assets/517f8cae-32ac-4886-8bd6-5e95447073e2" style="width:154px;height:40px;"></a>
<a href='https://discord.gg/5StRGu8EJV'><img src="https://github.com/user-attachments/assets/17cc06b8-ced8-4223-93dc-8d3a21c55753" style="width:154px;height:40px;"></a>
<h4>If you like this plugin you can support me!</h4>
<a href="https://opencollective.com/ucs/projects/usefulhints/donate"><img src="https://github.com/user-attachments/assets/f5aa3e98-d73c-470d-a134-5e9d923ea30b" style="width:154px;height:40px;"></a>

## Credits:
- Thanks [@NamelessSCP](https://github.com/NamelessSCP) for using the [RoundMVP](https://github.com/NamelessSCP/RoundMVP) idea.<br>
- Thanks [@XoMiya-WPC](https://github.com/XoMiya-WPC) for using the [WhoAreMyTeammates](https://github.com/XoMiya-WPC/WhoAreMyTeammates) idea.<br>
- Thanks [@Cat Potato](https://github.com/Cat-Potato) for using the [Auto Update](https://github.com/Vretu-Dev/UsefulHints/blob/81b7fde14ef6753ef215c4959c87c1a7abd988ec/UsefulHints/Update.cs) idea.<br>
- Thank you to these wonderful people for testing, bug reporting and translating:<br>
Testers: [Cat Potato](https://github.com/Cat-Potato), [Aserciak](https://steamcommunity.com/profiles/76561199053527692), [AVE_SATAN](https://steamcommunity.com/id/AVE_S4TAN/), [N](https://steamcommunity.com/profiles/76561199207670378), [Folia](https://steamcommunity.com/profiles/76561198004167374), [MVP_Faker](https://steamcommunity.com/id/746237524/), [Aime](https://steamcommunity.com/profiles/76561199125886809), [Clown](https://steamcommunity.com/profiles/76561199318901590), [OneManArmy](https://steamcommunity.com/profiles/76561199120200596)<br>
Bug Reporters: iksemdem, 𝒯𝓇𝒾𝓈𝓉𝒶𝓃𝐿𝒾𝓀𝑒𝓈𝒰𝓇𝒶𝓃 <br>
Translators: dxstruction [RU], Vretu [PL], Vretu [EN], [kldhsh123](https://github.com/kldhsh123) [ZH], baek_sol_ha [KO], [Lukaol-is](https://github.com/Lukaol-is) [CS]

## Config:

```yaml
UH:
  is_enabled: true
  debug: false
  # Hint Settings:
  enable_hints: true
  scp096_look_message: 'You looked at SCP-096!'
  scp268_time_left_message: 'Remaining: {0}s'
  scp2176_time_left_message: 'Remaining: {0}s'
  scp1576_time_left_message: 'Remaining: {0}s'
  grenade_damage_hint: '{0} Damage'
  jailbird_use_message: 'Remaining charges: {0}'
  scp207_hint_message: 'You have {0} doses of SCP-207'
  anti_scp207_hint_message: 'You have {0} doses of Anti SCP-207'
  show_hint_on_equip_item: false
  # Item Warnings:
  enable_warnings: true
  scp207_warning: '<color=yellow>⚠</color> You are already affected by <color=#A60C0E>SCP-207</color>'
  anti_scp207_warning: '<color=yellow>⚠</color> You are already affected by <color=#2969AD>Anti SCP-207</color>'
  scp1853_warning: '<color=yellow>⚠</color> You are already affected by <color=#1CAA21>SCP-1853</color>'
  # Friendly Fire Warning:
  enable_ff_warning: true
  friendly_fire_warning: '<size=27><color=yellow>⚠ Do not hurt your teammate</color></size>'
  damage_taken_warning: '<size=27><color=red>{0}</color> <color=yellow>(teammate) hit you</color></size>'
  class_d_are_teammates: true
  enable_cuffed_warning: false
  cuffed_attacker_warning: '<size=27><color=yellow>⚠ Player is cuffed</color></size>'
  cuffed_player_warning: '<size=27><color=red>{0}</color> <color=yellow>hit you when you were cuffed</color></size>'
  # Kill Counter:
  enable_kill_counter: true
  kill_count_message: '{0} kills'
  # Round Summary:
  enable_round_summary: true
  round_summary_message_duration: 10
  human_kill_message: '<size=27><color=#70EE9C>{0}</color> had the most kills as a <color=green>Human</color>: <color=yellow>{1}</color></size>'
  scp_kill_message: '<size=27><color=#70EE9C>{0}</color> had the most kills as a <color=red>SCP</color>: <color=yellow>{1}</color></size>'
  top_damage_message: '<size=27><color=#70EE9C>{0}</color> dealt the most damage: <color=yellow>{1}</color></size>'
  first_scp_killer_message: '<size=27><color=#70EE9C>{0}</color> was the first to kill an <color=red>SCP</color></size>'
  escaper_message: '<size=27><color=#70EE9C>{0}</color> escaped first from the facility: <color=yellow>{1}:{2}</color></size>'
  # Teammates:
  enable_teammates: true
  teammate_hint_delay: 4
  teammate_hint_message: |-
    <align=left><size=28><color=#70EE9C>Your Teammates</color></size> 
    <size=25><color=yellow>{0}</color></size></align>
  teammate_message_duration: 8
  alone_hint_message: '<align=left><color=red>You are playing Solo</color></align>'
  alone_message_duration: 4
  # Last Human Broadcast:
  enable_last_human_broadcast: true
  broadcast_for_human: '<color=red>You are the last human alive!</color>'
  broadcast_for_scp: '<color=#70EE9C>{0}</color> is the last human alive, playing as {1} in <color=yellow>{2}</color>'
  ignore_tutorial_role: true
  # Map Broadcast:
  enable_map_broadcast: true
  broadcast_warning_lcz: '<color=yellow>Light Zone</color> will be decontaminated in 5 minutes!'
```

## Showcase:
### Hints:
https://github.com/user-attachments/assets/f2125ab2-4ee3-4c71-a697-88949c00ddc2
### Summary:
<p align="center">
    <img src="https://github.com/user-attachments/assets/38238ca6-30f8-432d-a50d-71cacea1212b">
</p>

### Teammates:
<p align="center">
<img src="https://github.com/user-attachments/assets/9cd2ecce-237d-4801-bbe5-c253e8e22121">
</p>

### Last Alive Human:
<p align="center">
<img src="https://github.com/user-attachments/assets/c1a547af-c01a-4060-b810-0aeece2a9f7f">
<img src="https://github.com/user-attachments/assets/14829059-9249-4f53-a54b-2f5820a6f208">
</p>
