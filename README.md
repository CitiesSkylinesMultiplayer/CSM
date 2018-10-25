# CSM - Cities: Skylines Multiplayer
## Introduction
CSM is an in-development multiplayer mod for Cities: Skylines. This mod aims to provide a simple client-server experience where users can play and build together in a single game. Initially, resources such as money, costs demand will be synced between clients alongside roads and buildings. Further along in development, terrain sculpting and more advanced features may become available.  

Instructions to download and install the latest build of the mod can be seen below, alongside developer information about extending the mod. Pull requests and contributions would be much appreciated as I'm only developing this mod in my free time (although I will have a lot more free time during November 2018 - February 2019).

This mod and its source code is licensed under the MIT license.

## Download & Install

1. Download the latest build from the Azure DevOps site [here](https://dev.azure.com/gridentertainment/Tango/_build?definitionId=11). Click on the latest successful build -> `Artifacts` -> `CitiesSkylinesMultiplayer` -> `CitiesSkylinesMultiplayer.{Version}.zip`.
2. Either run the install script, or copy all the *.dll files to `%APPDATA%\Colossal Order\Cities_Skylines\Addons\Mods\CSM`.
3. Open Cities: Skylines and enable the mod.

If you want to build the mod yourself, follow the developer instructions below.

## Usage

1. Create a new game / open an existing game.
2. Click the `Show Multiplayer Menu` button in the top-left of your screen.
3. To host a game click on `Host Game`.
4. To join a game click on `Join Game` and enter the required information.

## Server & Client Information

* The default port used the the server and client is `4230`. You may need this to port forward your router.
* You may have to allow Cities: Skylines under your local firewall.
* If you're playing over LAN (in the same house for example), run `ipconfig` on the computer that will be hosting the server to find the local IP address. This is the address that you will want to use when connecting a client.
* If you're playing over the Internet, you may need  to portforward your router to expose the server to the internet (this is usually port `4230`). Once port forwarded, Google "What's my IP" on the server computer to find the IP address for the client to connect to.

## Current Status
On every commit and pull request, Azure Pipelines will build a new version. You can see the current build status below:

![Build Status](https://dev.azure.com/gridentertainment/Tango/_apis/build/status/Tango%20-%20Continuous%20Integration)

## Progress

### Phase 1
*Initial phase for setting up the mod.*

- [x] Build Basic UI
- [x] Allow basic connect, disconnect and host.

### Phase 2
*Make the mod easier to develop for/run. Build server management (kick users, view users). Allow sending messages through chat (works both in game and for debugging.*

- [x] Automatic Build Script.
- [x] Keep track of and view clients connected to a server.
- [ ] Client can request a list of other clients connected to the server.
- [ ] Build event system around client join, leave etc.

### Phase 3
*TBD*

## Developer Resources
### Introduction
This repository is split into two projects. `CSM` and `CSM.Testing`. `CSM` is the base project and contains all logic for the mod. `CSM.Testing` on the other hand helps simplify testing of multiplayer elements without having two games open.

The wiki contains more information, but I'm aiming to transfer that information to this file.

Ideally you should be running the latest version of Visual Studio 2017 on Windows 10.

### Installation
The mod can manually installed using the built in scripts. The following steps will guide you though this. Please note: You will need to have Visual Studio 2017 & Cities: Skylines installed, be running Windows 10 and have developer mode enabled.
This script will automatically pull in the required files (after specifying a folder), build the mod and then install it.

1. Open the `scripts` folder.
2. Run the following command in powershell `.\build.ps1 -Update -Build -Install`. This will match the mod to your game, build it and then install it. 
4. When you run this script, it will ask you for your steam folder. This is just the root folder of steam, e.g 'C:\Program Files\Steam\' 
5. Run Cities: Skylines and enable the mod. The mod can also be built and installed when the game is running (in most cases).

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
