# Project Tango
## Introduction
Project Tango is a Multiplayer mod for Cities: Skylines losely based off of Gurkenplayer. 
This mod aims to primarly focus on a client-server expirence where everyone shares the same resources / buildings etc. 
In the future I would like the ability to have seperate cities / income and make use of steams lobby / muiltiplayer system.

This respository is split into two projects. `Tango` and `Tango.GUITester`. `Tango` is the base project and contains all logic for the mod. `Tango.GUITester` on the other hand helps simplify testing of muiltiplayer elements.

## Current Features
* Pretty slick looking muiltiplayer GUI.
* Ability to host server (no logic or connection, but the server is active). Can be closed and re-opened more than once in game.

## Logic Flow
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

## Development Notes
Make sure the Lidgren.Network.dll is in the same location as your mod DLL when testing.

## License
This project is licensed under MIT.
