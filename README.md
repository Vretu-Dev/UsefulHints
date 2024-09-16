# UsefulHints for EXILED

![downloads](https://img.shields.io/github/downloads/Vretu-Dev/UsefulHints/total)

## Features:
- SCP-268 time remaining after use
- SCP-268 duration time
- SCP-2176 lockdown time
- Hint when you look at the SCP-096 face
- Hint how many times the Jailbird charge has been used
- Hint how many SCP 207 you are at when you pick it up
- Summary of the game
- Kill counter

### Minimum Exiled Version: 8.9.8
### Credits for [@NamelessSCP](https://github.com/NamelessSCP) for using the [RoundMVP](https://github.com/NamelessSCP/RoundMVP) idea.

## Config:

```yaml
UH:
  is_enabled: true
  debug: false
  scp096_look_message: 'You looked at SCP-096!'
  scp268_duration: 15
  scp268_time_left_message: 'Remaining: {0}s'
  scp2176_time_left_message: 'Remaining: {0}s'
  jailbird_use_message: 'Jailbird has been used {0} times'
  scp207_hint_message: 'You are on {0} SCP-207'
  kill_count_message: '{0} kills'
  # Should a summary of the round be displayed.
  enable_summary: true
  human_kill_message: '<size=27><color=#70EE9C>{0}</color> had the most kills as <color=green>Human</color>: <color=yellow>{1}</color></size>'
  scp_kill_message: '<size=27><color=#70EE9C>{0}</color> had the most kills as <color=red>SCP</color>: <color=yellow>{1}</color></size>'
  top_damage_message: '<size=27><color=#70EE9C>{0}</color> did the most damage: <color=yellow>{1}</color></size>'
  first_scp_killer_message: '<size=27><color=#70EE9C>{0}</color> was the first to kill <color=red>SCP</color></size>'
  escaper_message: '<size=27><color=#70EE9C>{0}</color> escaped first from the facility: <color=yellow>{1}:{2}</color></size>'
```
## Showcase:
### Hints:
https://github.com/user-attachments/assets/f2125ab2-4ee3-4c71-a697-88949c00ddc2
### Summary:
<p align="center">
    <img src="https://github.com/user-attachments/assets/38238ca6-30f8-432d-a50d-71cacea1212b">
</p>
