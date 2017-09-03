# Project Tango
Project Tango is a Multiplayer mod for Cities: Skylines losely based off of Gurkenplayer. 
This mod aims to primarly focus on a client-server expirence where everyone shares the same resources / buildings etc. 
In the future I would like the ability to have seperate cities / income and make use of steams lobby / muiltiplayer system.

## Flow
Below is information that I have jotted down about the flow of this mod.

**Server:**
* User launches game, enabled mod, loads a level.
* Click "Show Muiltiplayer Menu" --> "Host Game".
  * Start the server using `Networking/Server.cs`.
  * Read incoming messages
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
