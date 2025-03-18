# FHEMlogs
Hilfsprogramm für das [Smart-Home-Paket FHEM](https://fhem.de/fhem.html)
> [!NOTE]
> Feststellen, wann ein Sensor zuletzt Daten gesendet hat
## Problem
Smart Home mit FHEM: 
- :green_square: Sensoren senden Daten
- :green_square: Daten werden in Logfiles gespeichert
- :red_square: Sensoren können ausfallen :hurtrealbad::hurtrealbad::hurtrealbad: 
## Lösung
- Programm liest Logfiles
- stellt fest, wann ein Sensor zuletzt gesensort hat
- alarmiert, wenn das zu lange her ist :warning:
## Installation
in cron.daily:
```
mono /usr/local/fhemskripte/FHEMlogs.exe  /opt/fhem/log
```

