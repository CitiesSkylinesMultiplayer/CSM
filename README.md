# CSM - Cities: Skylines Multiplayer

[![Discord](https://img.shields.io/discord/508902220943851522.svg)](https://discord.gg/RjACPhd)
[![Build Status](https://dev.azure.com/gridentertainment/Tango/_apis/build/status/Tango%20-%20Continuous%20Integration)](https://dev.azure.com/gridentertainment/Tango/_build?definitionId=11)
[![Steam File Size](https://img.shields.io/steam/size/1558438291.svg)](https://steamcommunity.com/sharedfiles/filedetails/?id=1558438291)


## Introduction
CSM is an in-development multiplayer mod for Cities: Skylines. This mod aims to provide a simple client-server experience where users can play and build together in a single game. Initially, resources such as money, costs demand will be synced between clients alongside roads and buildings. Further along in development, terrain sculpting and more advanced features may become available.  

Instructions to download and install the latest build of the mod can be seen below, alongside developer information about extending the mod. Pull requests and contributions would be much appreciated as I'm only developing this mod in my free time (although I will have a lot more free time during November 2018 - February 2019).

Feel free to join the development Discord server [here](https://discord.gg/RjACPhd).

This mod and its source code is licensed under the MIT license.

## Download & Install

You can now also install this mod from the Steam Workshop [here](https://steamcommunity.com/sharedfiles/filedetails/?id=1558438291).

1. Download the latest build from the Azure DevOps site [here](https://dev.azure.com/gridentertainment/Tango/_build?definitionId=11). Click on the latest successful build -> `Artifacts` -> `CitiesSkylinesMultiplayer` -> `CitiesSkylinesMultiplayer.{Version}.zip`.
2. Either run the install script, or copy all the *.dll files to `%LOCALAPPDATA%\Colossal Order\Cities_Skylines\Addons\Mods\CSM`.
3. Open Cities: Skylines and enable the mod.

If you want to build the mod yourself, follow the developer instructions below.

## Usage

1. Create a new game / open an existing game. If you open an existing game, make sure both client and server load the exact same world.
2. Click the `Show Multiplayer Menu` button in the top-left of your screen.
3. To host a game click on `Host Game`.
4. To join a game click on `Join Game` and enter the required information.

## Server & Client Information

* The default port used the the server and client is `4230`. You may need this to port forward your router.
* You may have to allow Cities: Skylines under your local firewall.
* You can find the Local (LAN) & External IP of your computer in the Host Game menu.
* If you're playing over LAN (in the same house for example), run `ipconfig` on the computer that will be hosting the server to find the local IP address. This is the address that you will want to use when connecting a client.
* If you're playing over the Internet, you may need to port forward your router to expose the server to the internet (this is usually port `4230`). Once port forwarded, Google "What's my IP" on the server computer to find the IP address for the client to connect to.

## Progress

### Synced Items
- Play/pause status.
- Game speed.
- Money.
- Building created.
- Building removed.
- Building relocated.
- Demand.
- Tax (Sync but be aware that it does not show on the UI).
- Budget (Sync but be aware that it does not show on the UI).
- Road creation and deletion.
- Powerline creation and deletion.
- Water pipe creation and deletion.
- Zones.

## Contributors
- [Dominic Maas (DominicMaas)](https://github.com/DominicMaas)
- [Sander Jochems (Sander0542)](https://github.com/Sander0542)
- [Treholt (Treholt3103)](https://github.com/Treholt3103)
- [kaenganxt](https://github.com/kaenganxt)

## Developer Resources
Developer recourses can be found on the GitHub wiki.