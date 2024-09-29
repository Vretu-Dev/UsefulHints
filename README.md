# UsefulHints for EXILED

[![downloads](https://img.shields.io/github/downloads/Vretu-Dev/UsefulHints/total)](https://github.com/Vretu-Dev/UsefulHints/releases/latest)<br><br>
**If you like this plugin you can support me!** <p text-align="center"> [![PayPal](https://img.shields.io/badge/PayPal-00457C?style=for-the-badge&logo=paypal&logoColor=white)](https://www.paypal.com/paypalme/vretu)</p>

## Features:
- SCP-268 time remaining after use
- SCP-268 duration time
- SCP-2176 lockdown time
- Hint when you look at the SCP-096 face
- Hint how many times the Jailbird charge has been used
- Hint how many SCP 207 you are at when you pick it up
- Summary of the game
- Kill counter
- Hint show your teammates at the start of the game
- Broadcast last human alive for players
- Jailbird Custom Setting

### Minimum Exiled Version: 8.9.8
### Credits:
- Thanks [@NamelessSCP](https://github.com/NamelessSCP) for using the [RoundMVP](https://github.com/NamelessSCP/RoundMVP) idea.<br>
- Thanks [@cherniichernish](https://steamcommunity.com/id/Denis_Ik/) for using Jailbird Patch.<br>
- Thanks [@XoMiya-WPC](https://github.com/XoMiya-WPC) for using the [WhoAreMyTeammates](https://github.com/XoMiya-WPC/WhoAreMyTeammates) idea.<br>
## Config:

```yaml
UH:
  is_enabled: true
  debug: false
  # [Module] Hints:
  enable_hints: false
  scp096_look_message: 'You looked at SCP-096!'
  scp268_duration: 15
  scp268_time_left_message: 'Remaining: {0}s'
  scp2176_time_left_message: 'Remaining: {0}s'
  jailbird_use_message: 'Jailbird has been used {0}/5 times'
  scp207_hint_message: 'You are on {0} SCP-207'
  # [Module] Kill Counter:
  enable_kill_counter: true
  kill_count_message: '{0} kills'
  # [Module] Round Summary:
  enable_round_summary: true
  round_summary_message_duration: 10
  human_kill_message: '<size=27><color=#70EE9C>{0}</color> had the most kills as <color=green>Human</color>: <color=yellow>{1}</color></size>'
  scp_kill_message: '<size=27><color=#70EE9C>{0}</color> had the most kills as <color=red>SCP</color>: <color=yellow>{1}</color></size>'
  top_damage_message: '<size=27><color=#70EE9C>{0}</color> did the most damage: <color=yellow>{1}</color></size>'
  first_scp_killer_message: '<size=27><color=#70EE9C>{0}</color> was the first to kill <color=red>SCP</color></size>'
  escaper_message: '<size=27><color=#70EE9C>{0}</color> escaped first from the facility: <color=yellow>{1}:{2}</color></size>'
  # [Module] Teammates:
  enable_teammates: true
  teammate_hint_delay: 4
  teammate_hint_message: |-
    <align=left><size=28><color=#70EE9C>Your Teammates</color></size> 
    <size=25><color=yellow>{0}</color></size></align>
  teammate_message_duration: 8
  alone_hint_message: '<align=left><color=red>You are playing Solo</color></align>'
  alone_message_duration: 4
  # [Module] Last Human Broadcast:
  enable_last_human_broadcast: true
  broadcast_for_human: '<color=red>You are the last human alive!</color>'
  broadcast_for_scp: '<color=#70EE9C>{0}</color> is the last human alive playing as {1} in <color=yellow>{2}</color>'
  # [Module] Jailbird Custom Settings:
  enable_custom_jailbird_settings: false
  jailbird_effect: Flashed
  jailbird_effect_duration: 4
  jailbird_effect_intensity: 1
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
