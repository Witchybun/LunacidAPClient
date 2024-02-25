# Lunacid Setup Guide
## Requirements

You will need:
- [BepInEx 5.4.22](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.22)
- [Archipelago Client](https://github.com/ArchipelagoMW/Archipelago/releases/tag/0.4.4)
- The .apworld packaged with this mod.
- The mod itself.

## Installation

- Download and unpackage the BepInEx version for x64 into your Lunacid installation folder.  If you don't know where it is, in Steam you can right click and Manage -> Browse Local Files will get you there.
- **LINUX ONLY**: Right click Lunacid in Steam, go to Properties, and in Launch Options put `WINEDLLOVERRIDES="winhttp.dll=n,b" %command%`.
- Launch the game at least once, close.
- In BepInEx/plugins, put the folder in the attached mod zip.
- Install Archipelago Client.  Documentation is [here](https://archipelago.gg/tutorial/Archipelago/setup/en).
- Once installed, go to where your client is installed, go to lib/worlds, drop the attached .apworld here.
- Run ArchipelagoLauncher, hit Generate Template Settings, in order for the Lunacid.yaml to be generated.

Hosting a game locally is simply taking the Lunacid.yaml file, editing it to suit the settings you want, putting the file in the Players folder, and hitting Generate in ArchipelagoLauncher.  For yaml formatting help, look [here](https://archipelago.gg/tutorial/Archipelago/advanced_settings/en).

## In-game setup

- Upon starting, **do not login yet**.  The system already instakicks you if you try to, as there is no slot data to work with.
- Create a new save and a new character and wait until you are loaded into Hollow Basin.
- Top left corner of the screen, click "Show", put in your hostname, username, password, hit connect.
- If successful, all data is saved to a file, found in the base game's folder under ArchSaves.
- Enjoy playing!

## Troubleshooting

* Q: My server's port changed and I can't connect.  How do I fix this?*
**A: Follow the guide for connecting, but use the new host name and port.  This should save appropriately.**

*Q: I was sent a key to open a door, but it won't open.*
**A: Currently, the way I modify flag states is very direct, as the base system requires a lot of game object data that might not be present.  Simply reload the area to force a Load call for now.**

If more stuff comes up I'll add it here.  Probably will, you know how it is.
