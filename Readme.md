# FHEMlogs
 Feststellen, wann ein Sensor zuletzt Daten gesendet hat
## Problem
Smart Home mit FHEM: 
- Sensoren senden Daten
- Daten werden in Logfiles gespeichert
- Sensoren können ausfallen
## Lösung
- Programm liest Logfiles
- stellt fest, wann ein Sensor zuletzt gesensort hat
- alarmiert, wenn das zu lange her ist
