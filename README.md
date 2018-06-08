# Project Tango
## Introduction
Project Tango (known as CitiesSkylinesMultiplayer in code) is a Multiplayer mod for Cities: Skylines loosely based off of GurkenPlayer. The mod aims to provide a simple client-server experience. All players will share and sync the same resources such as money, costs, demand etc. Hopefully in the future roads and terrain will also be synced. Further in the future, I would like to implement the ability to have separate cities / income and make use of steams lobby / multiplayer system. Buy as this is only proof of concept for now, nothing can be guaranteed.

It should be noted that I'm currently developing this mod in my free-free time. Once the mod is more mature, contributions would be much appreciated. In the mean time, I'll try add as many notes as possible below to help any development efforts.

This page will be split into four main sections, `Current Status`, `Progress`, `Installation` and `Developer Resources`. `Current Status` displays build/CI information, `Progress` will show the monthly road map, `Installation` tells you how to install and setup the mod and finally, `Developer Resources` contains all resources related to developing this mod.

## Current Status

## Progress

### Phase 1
- [ ] Functional Build Script.

### Phase 2


### Phase 3

## Installation

## Developer Resources
### Introduction
This repository is split into two projects. `CitiesSkylinesMultiplayer` and `CitiesSkylinesMultiplayer.GUITester`. `CitiesSkylinesMultiplayer` is the base project and contains all logic for the mod. `CitiesSkylinesMultiplayer.GUITester` on the other hand helps simplify testing of multiplayer elements without having two games open.

The wiki contains more information, but I'm aiming to transfer that information to this file.

Ideally you should be running the latest version of Visual Studio 2017 on Windows 10.

### Client-Server Model
This mod uses the client-server model. A user will setup their game as a server and transmit information like currency, roads, needs etc. to all connected clients. Clients will connect to the server and retrieve currency, roads, needs etc. from the server, updating the client UI.

This is all done by running a UDP server alongside Cities Skylines. This UDP server will interact with the extension methods in the ICities DLL. It is important that we extend and override the base methods for these extensions (as we override some values).

### Logic Flow
Below is information that I have jotted down about the flow of this mod.

**Server:**
1. Open a level (new or existing).
2. "Show Multiplayer Menu" --> "Host Game". User enters port and password (optional).
3. Server is setup and message process queue is started. (`Networking/Server.cs`)

Message Queue:
* Parse incoming messages and call appropiate event handlers (UpdateEconomy etc.)
* On extension changes, send a packet to all clients.

**Client:**
* User launches game, enabled mod, loads a level.
* Click "Show Muiltiplayer Menu" --> "Join Game".
* Enter game IP address / port.
  * Unload the level
  * Connect the client using `Networking/Client.cs`.
  * Client connects to server (try) and performs setup functions (check mods same etc.)
  * Update the client to mirror the server
  * On all events, update the server.
  * On incoming message, update client UI.

## License
This project is licensed under MIT.