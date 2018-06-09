# Project Tango
## Introduction
Project Tango (known as CitiesSkylinesMultiplayer in code) is a Multiplayer mod for Cities: Skylines loosely based off of GurkenPlayer. The mod aims to provide a simple client-server experience. All players will share and sync the same resources such as money, costs, demand etc. Hopefully in the future roads and terrain will also be synced. Further in the future, I would like to implement the ability to have separate cities / income and make use of steams lobby / multiplayer system. Buy as this is only proof of concept for now, nothing can be guaranteed.

It should be noted that I'm currently developing this mod in my free-free time. Once the mod is more mature, contributions would be much appreciated. In the mean time, I'll try add as many notes as possible below to help any development efforts.

This project is licensed under MIT.

## Current Status

## Progress

### Phase 1
*Initial phase for setting up the mod.*

- [x] Build Basic UI
- [x] Allow basic connect, disconnect and host.

### Phase 2
*Make the mod easier to develop for/run. Build server management (kick users, view users). Allow sending messages through chat (works both in game and for debugging.*

- [x] Automatic Build Script.
- [ ] Keep track of and view clients connected to a server.
- [ ] Client can request a list of other clients connected to the server.
- [ ] Build event system around client join, leave etc.

### Phase 3
*TBD*

## Installation
The mod can manually installed using the built in scripts. The following steps will guide you though this. Please note: You will need to have Visual Studio 2017 & Cities: Skylines installed, be running Windows 10 and have developer mode enabled.
This script will automatically pull in the required files (after specifying a folder), build the mod and then install it.

1. Open the `scripts` folder.
2. Run the `copy_mod.ps1` powershell script with both `-update` and `-build` flags enabled. 
3. Run Cities: Skylines and enable the mod. The mod can also be built and installed when the game is running (in most cases).

## Usage
1. Create a new game / open an existing game.
2. Click the `Show Multiplayer Menu` button in the top-left of your screen.
3. To host a game click on `Host Game`.
4. To join a game click on `Join Game` and enter the required information.

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
* Parse incoming messages and call appropriate event handlers (UpdateEconomy etc.)
* On extension changes, send a packet to all clients.

**Client:**
* User launches game, enabled mod, loads a level.
* Click "Show Multiplayer Menu" --> "Join Game".
* Enter game IP address / port.
  * Unload the level
  * Connect the client using `Networking/Client.cs`.
  * Client connects to server (try) and performs setup functions (check mods same etc.)
  * Update the client to mirror the server
  * On all events, update the server.
  * On incoming message, update client UI.
  
### `ICities.dll` Extensions
Here is a list of extensions that can be used in the ICities.dll (not much documentation elsewhere).

#### AreasExtensionBase
We can use these extension methods to sync which areas have been bought.

|Method|Return|
|--|--|
|`OnCanUnlockArea(int x, int z, bool originalResult)`|`originalResult`|
|`OnGetAreaPrice(uint ore, uint oil, uint forest, uint fertility, uint water, bool road, bool train, bool ship, bool plane, float landFlatness, int originalPrice)`|`originalPrice`|
|`OnUnlockArea(int x, int z)`|**VOID**|

#### BuildingExtensionBase
We can use this to find out where builds are. Looks like it may be quite complicated. Guess we will see.

|Method|Return|
|--|--|
|`SpawnData OnCalculateSpawn(Vector3 location, SpawnData spawn)`|`spawn`|
|`OnBuildingCreated(ushort id)`|**VOID**|
|`OnBuildingRelocated(ushort id)`|**VOID**|
|`OnBuildingReleased(ushort id)`|**VOID**|

#### ChirperExtensionBase
This really is not that important, (I personally don't really like the "Twitter" like feature), but it can still be implemented. On Server we send the new chirper message to the clients on `OnNewMessage` event. Client side we ignore these chirpers.

|Method|Return|
|--|--|
|`OnNewMessage(IChirperMessage message)`|**VOID**|

#### DemandExtensionBase
This extension will allow us to synchronize demand across all connected clients. More research is required, but from what I understand, we need to override the `OnCalculate*Demmand` methods to grab the calculated demand from the server. On the server we will access the demand manager to get the current demand. The `OnUpdateDemand` method will also be used for server-client syncing.

|Method|Return|
|--|--|
|`OnCalculateResidentialDemand(int originalDemand)`|`originalDemand`|
|`OnCalculateCommercialDemand(int originalDemand)`|`originalDemand`|
|`OnCalculateWorkplaceDemand(int originalDemand)`|`originalDemand`|
|`OnUpdateDemand(int lastDemand, int nextDemand, int targetDemand)`|`nextDemand`|

#### EconomyExtensionBase
Currently looking at implementing `OnUpdateMoneyAmount` to sync money between clients. Some basic testing showed that this was only updating the UI? Need to look further into it.

|Method|Return|
|--|--|
|`OnAddResource(EconomyResource resource, int amount, Service service, SubService subService, Level level)`|`amount`|
|`OnFetchResource(EconomyResource resource, int amount, Service service, SubService subService, Level level)`|`amount`|
|`OnPeekResource(EconomyResource resource, int amount)`|`amount`|
|`OnGetConstructionCost(int originalConstructionCost, Service service, SubService subService, Level level)`|`amount`|
|`OnGetMaintenanceCost(int originalMaintenanceCost, Service service, SubService subService, Level level)`|`amount`|
|`OnGetRelocationCost(int constructionCost, int relocationCost, Service service, SubService subService, Level level)`|`amount`|
|`OnGetRefundAmount(int constructionCost, int refundAmount, Service service, SubService subService, Level level)`|`amount`|
|`OnUpdateMoneyAmount(long internalMoneyAmount)`|`internalMoneyAmount`|

#### IDisasterBase
This has a different naming scheme for some reason? This would be used for syncing disasters (going to be interesting to setup)

|Method|Return|
|--|--|
|`OnDisasterCreated(ushort disasterID)`|**VOID**|
|`OnDisasterStarted(ushort disasterID)`|**VOID**|
|`OnDisasterDetected(ushort disasterID)`|**VOID**|
|`OnDisasterActivated(ushort disasterID)`|**VOID**|
|`OnDisasterDeactivated(ushort disasterID)`|**VOID**|
|`OnDisasterFinished(ushort disasterID)`|**VOID**|

#### LevelUpExtensionBase
todo

#### LoadingExtensionBase
todo

#### MilestonesExtensionBase
todo

#### ResourceExtensionBase
todo

#### SerializableDataExtensionBase
todo

#### TerrainExtensionBase
todo

#### ThreadingExtensionBase
todo
