using System;
using System.Linq;
using CSM.API.Commands;
using CSM.API.Networking.Status;
using CSM.Commands;
using CSM.Commands.Data.Game;
using CSM.Commands.Handler.Game;
using CSM.Commands.Handler.Internal;
using CSM.Networking;
using CSM.Panels;
using CSM.Util;
using UnityEngine;

namespace CSM.Helpers
{
    public static class SpeedPauseHelper
    {
        private static System.Random _rand;

        // Current state of the state machine
        private static SpeedPauseState _state;
        // Currently queued speed
        private static int _speed;
        
        // Target time during WaitingForPlay or WaitingForPause states representing real-time and in game time respectively
        private static DateTime _waitTargetTime;


        // Target values if one of the buttons was pressed by the player
        public static bool? PauseTarget = null;
        public static int? SpeedTarget = null;
        
        // If the state machine was already initialized
        private static bool _initialized = false;

        /// <summary>
        ///     Resets the speed and pause states and re-initializes the state machine.
        /// </summary>
        public static void ResetSpeedAndPauseState()
        {
            _initialized = false;
            SetSpeedStateInternal(1);
            SetPauseStateInternal(true);
        }

        /// <summary>
        ///     Checks if the current play/pause state is stable, e.g. for the /sync command.
        /// </summary>
        /// <returns>If the state is either paused or playing but not in any kind of waiting state.</returns>
        public static bool IsStable()
        {
            return _state == SpeedPauseState.Paused || _state == SpeedPauseState.Playing;
        }

        /// <summary>
        ///     Called every simulation step to check for any changed speed or pause states.
        /// </summary>
        public static void SimulationStep()
        {
            // First tick in the game, initialize speed and pause state tracking variables
            if (!_initialized)
            {
                Initialize(SimulationManager.instance.SimulationPaused, SimulationManager.instance.SelectedSimulationSpeed);
                _initialized = true;
            }

            // Stores if a speed or pause state negotiation is allowed to start or if a changed state should be ignored
            bool allowNegotiation = false;

            // If game is blocked or client is connecting
            if (MultiplayerManager.Instance.GameBlocked ||
               (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client && 
                MultiplayerManager.Instance.CurrentClient.Status != ClientStatus.Connected))
            {
                // Pause the game if it is not yet paused (on the server) and thus trigger the pause negotiation (PauseRequest)
                if (!SimulationManager.instance.SimulationPaused &&
                    MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
                {
                    // Only request pause when the state is stable (so no warning message is shown to the user)
                    if (IsStable())
                    {
                        PauseTarget = true;
                        allowNegotiation = true;
                    }
                }
                // Pause the game if it is not yet paused (on the joining/syncing client) but don't trigger a negotiation
                else if (!SimulationManager.instance.SimulationPaused &&
                         MultiplayerManager.Instance.CurrentClient.Status != ClientStatus.Connected)
                {
                    SetSpeedStateInternal(1);
                    SetPauseStateInternal(true);
                }
            }
            else
            {
                allowNegotiation = true;
            }

            if (allowNegotiation)
            {
                // Check if speed or pause state have changed
                if ((SpeedTarget.HasValue && SpeedTarget.Value != SimulationManager.instance.SelectedSimulationSpeed) ||
                    (PauseTarget.HasValue && PauseTarget != SimulationManager.instance.SimulationPaused))
                {
                    PlayPauseSpeedChanged(PauseTarget ?? SimulationManager.instance.SimulationPaused,
                        SpeedTarget ?? SimulationManager.instance.SelectedSimulationSpeed);
                }
            }

            // Reset changes in speed or pause state because we don't want to consider them in a later tick
            SpeedTarget = null;
            PauseTarget = null;

            // Update play/pause state if needed
            ChangePauseStateIfNeeded();
        }

        /// <summary>
        ///     Sets the internal pause state without triggering a negotiation by calling the setter.
        /// </summary>
        /// <param name="paused">If the game should be paused.</param>
        private static void SetPauseStateInternal(bool paused)
        {
            ReflectionHelper.SetAttr(SimulationManager.instance, "m_simulationPaused", paused);
        }

        /// <summary>
        ///     Sets the internal speed without triggering a negotiation by calling the setter.
        /// </summary>
        /// <param name="speed">The game speed to set.</param>
        private static void SetSpeedStateInternal(int speed)
        {
            ReflectionHelper.SetAttr(SimulationManager.instance, "m_simulationSpeed", speed);
        }

        /// <summary>
        ///     Initialize current speed and pause states.
        /// </summary>
        /// <param name="paused">If the game is currently paused.</param>
        /// <param name="speed">Current game speed from 0 to 3.</param>
        private static void Initialize(bool paused, int speed)
        {
            _state = paused ? SpeedPauseState.Paused : SpeedPauseState.Playing;
            _speed = speed;
        }

        /// <summary>
        ///     Called when the speed or pause state have changed.
        ///     This normally happens when the player clicks on one of the two buttons in the bottom left.
        ///     This will only trigger any action if the current state is either Playing or Paused.
        /// </summary>
        /// <param name="pause">If the game should be paused.</param>
        /// <param name="speed">The newly selected speed.</param>
        private static void PlayPauseSpeedChanged(bool pause, int speed)
        {
            if (_state == SpeedPauseState.Paused && !pause)
            {
                RequestPlay(speed);
                Log.Debug($"[SpeedPauseHelper] State {SpeedPauseState.Playing} requested locally.");
            }
            else if (_state == SpeedPauseState.Playing && pause)
            {
                RequestPause();
                Log.Debug($"[SpeedPauseHelper] State {SpeedPauseState.Paused} requested locally.");
            }
            else if (_state == SpeedPauseState.Playing && speed != _speed)
            {
                RequestSpeedChange(speed);
                Log.Debug("[SpeedPauseHelper] Speed change requested locally.");
            }
            else
            {
                ChatLogPanel.PrintGameMessage("Please wait until all players have reached the same play/pause state and speed!");
                Log.Info($"[SpeedPauseHelper] (Pause: {pause}, Speed: {speed}) requested, but state was {_state}");
            }
        }

        /// <summary>
        ///     Called when a SpeedPauseRequest command was received.
        /// </summary>
        /// <param name="pause"></param>
        /// <param name="speed"></param>
        /// <param name="requestId"></param>
        public static void PlayPauseRequest(bool pause, int speed, int requestId)
        {
            if (pause) // Pause requested
            {
                if (_state == SpeedPauseState.Playing)
                {
                    _state = SpeedPauseState.PauseRequested;
                    Log.Debug($"[SpeedPauseHelper] State {SpeedPauseState.Paused} requested remotely.");
                }
                SendSpeedPauseResponse(requestId);
            }
            else // Speed change or play requested
            {
                if (_state == SpeedPauseState.Playing)
                {
                    _state = SpeedPauseState.SpeedChangeRequested;
                    _speed = speed;
                    Log.Debug("[SpeedPauseHelper] Speed change requested remotely.");
                }
                else if (_state == SpeedPauseState.Paused)
                {
                    _state = SpeedPauseState.PlayRequested;
                    _speed = speed;
                    Log.Debug($"[SpeedPauseHelper] State {SpeedPauseState.Playing} requested remotely.");
                }
                SendSpeedPauseResponse(requestId);
            }
        }

        /// <summary>
        ///     Called when all responses to a pause or speed change request have been received.
        /// </summary>
        /// <param name="highestGameTime">The highest game time of all responses.</param>
        /// <param name="highestLatency">The highest latency of all responses.</param>
        public static void SpeedPauseResponseReceived(long highestGameTime, long highestLatency)
        {
            // Pause time is computed by taking the highest game time plus 4 times the maximum latency because this
            // is the worst case roundtrip time from client1 -> server -> client2 -> server -> client1 which means
            // that this amount of time may have already passed since the highest game time was determined.
            DateTime pauseTime = new DateTime(highestGameTime) + MillisecondsToInGameTime(highestLatency * 4);
            if (_state == SpeedPauseState.PauseRequested)
            {
                _state = SpeedPauseState.WaitingForPause;
                _waitTargetTime = pauseTime;
            }
            else if (_state == SpeedPauseState.SpeedChangeRequested)
            {
                _state = SpeedPauseState.WaitingForSpeedChange;
                _waitTargetTime = pauseTime;
            }
            else if (_state == SpeedPauseState.PlayRequested)
            {
                _state = SpeedPauseState.WaitingForPlay;

                // TODO: Maybe find a way to start the games simultaneously
                _waitTargetTime = DateTime.Now;
            }
        }

        /// <summary>
        ///     This method is called every tick and checks if the waitTargetTime has already been reached.
        /// </summary>
        private static void ChangePauseStateIfNeeded()
        {
            if (_state == SpeedPauseState.WaitingForPause && GetCurrentTime() > _waitTargetTime)
            {
                SetPauseStateInternal(true);
                _state = SpeedPauseState.PausedWaiting;
                SendReached();

                // Clear queued drop frames because we decided on an exact pause time, so the games are already in sync
                SlowdownHelper.ClearDropFrames();
            }
            else if (_state == SpeedPauseState.WaitingForSpeedChange && GetCurrentTime() > _waitTargetTime)
            {
                SetSpeedStateInternal(_speed);
                _state = SpeedPauseState.PlayingWaiting;
                SendReached();
                
                // Don't clear dropped frames here because the game still continues.
                // Even if the current amount is not correct anymore because the dropped frames were
                // recorded while using a different speed, it's still an acceptable approximation.
            }
            else if (_state == SpeedPauseState.WaitingForPlay && DateTime.Now >= _waitTargetTime)
            {
                SetPauseStateInternal(false);
                SetSpeedStateInternal(_speed);
                _state = SpeedPauseState.PlayingWaiting;
                SendReached();

                // Clear queued drop frames because those that arrived during paused state can be ignored
                SlowdownHelper.ClearDropFrames();
            }
        }

        /// <summary>
        ///     Called when all SpeedPauseReached commands to a request have been received.
        ///     This will allow the player to press the buttons again.
        /// </summary>
        public static void StateReached()
        {
            if (_state == SpeedPauseState.PlayingWaiting)
            {
                _state = SpeedPauseState.Playing;
            }
            else if (_state == SpeedPauseState.PausedWaiting)
            {
                _state = SpeedPauseState.Paused;
                
                // If a connection request is pending, we complete it here, since now all games are paused.
                if (ConnectionRequestHandler.WorldLoadingPlayer != null)
                {
                    ConnectionRequestHandler.AllGamesBlocked();
                }
            }

            Log.Debug($"[SpeedPauseHelper] State {_state} reached!");
        }

        private static void RequestPlay(int speed)
        {
            _state = SpeedPauseState.PlayRequested;

            InitRand();

            int requestId = _rand.Next();

            Command.SendToAll(new SpeedPauseRequestCommand()
            {
                RequestId = requestId,
                SimulationPaused = false,
                SelectedSimulationSpeed = speed
            });

            _speed = speed;
            SendSpeedPauseResponse(requestId);
        }

        /// <summary>
        ///     Request the pause state.
        ///     Sets state to PauseRequested.
        ///     Sends a SpeedPauseRequest and -Response with a randomly generated request id.
        /// </summary>
        private static void RequestPause()
        {
            _state = SpeedPauseState.PauseRequested;

            InitRand();

            int requestId = _rand.Next();
            
            Command.SendToAll(new SpeedPauseRequestCommand()
            {
                RequestId = requestId,
                SimulationPaused = true
            });
            SendSpeedPauseResponse(requestId);
        }

        /// <summary>
        ///     Request a speed change (while playing).
        ///     Sets state to SpeedChangeRequested.
        ///     Sends a SpeedPauseRequest and -Response with a randomly generated request id.
        /// </summary>
        /// <param name="speed">The target speed to request.</param>
        private static void RequestSpeedChange(int speed)
        {
            _state = SpeedPauseState.SpeedChangeRequested;
            
            InitRand();
            
            int requestId = _rand.Next();
            
            Command.SendToAll(new SpeedPauseRequestCommand()
            {
                RequestId = requestId,
                SimulationPaused = false,
                SelectedSimulationSpeed = speed
            });

            _speed = speed;
            SendSpeedPauseResponse(requestId);
        }

        /// <summary>
        ///     Send the response to a SpeedPauseRequest.
        ///     The response is sent to all other games and also processed by this client.
        /// </summary>
        /// <param name="requestId">The request id to respond to.</param>
        private static void SendSpeedPauseResponse(int requestId)
        {
            int numClients;
            switch (MultiplayerManager.Instance.CurrentRole)
            {
                case MultiplayerRole.Client:
                    numClients = -1;
                    break;
                case MultiplayerRole.Server:
                    numClients = 1 + MultiplayerManager.Instance.CurrentServer.ConnectedPlayers
                        .Count(p => p.Value.Status == ClientStatus.Connected);
                    break;
                default:
                    numClients = 1;
                    break;
            }

            CommandBase cmd = new SpeedPauseResponseCommand()
            {
                CurrentTime = GetCurrentTime().Ticks,
                MaxLatency = GetMaximumLatency(),
                NumberOfClients = numClients,
                RequestId = requestId
            };

            Command.GetCommandHandler(cmd.GetType()).Parse(cmd);
            Command.SendToAll(cmd);
        }

        /// <summary>
        ///     Send a SpeedPauseReached response.
        ///     The command is sent to all other games and also processed by this client.
        /// </summary>
        private static void SendReached()
        {
            CommandBase cmd = new SpeedPauseReachedCommand()
            {
                RequestId = SpeedPauseReachedHandler.GetCurrentId(),
            };
            
            Command.GetCommandHandler(cmd.GetType()).Parse(cmd);
            Command.SendToAll(cmd);
        }

        /// <summary>
        ///     Retrieves the current in game time.
        /// </summary>
        /// <returns>The current in game time.</returns>
        private static DateTime GetCurrentTime()
        {
            return SimulationManager.instance.m_metaData.m_currentDateTime;
        }

        /// <summary>
        ///     Computes the maximum network latency of connected clients.
        ///     When acting as a server with connected clients, this will return the maximum latency to one of the clients.
        ///     When acting as a client, this returns the latency to the server.
        ///     Otherwise this returns 0.
        /// </summary>
        /// <returns>The maximum network latency.</returns>
        private static long GetMaximumLatency()
        {
            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
            {
                return MultiplayerManager.Instance.CurrentClient.ClientPlayer.Latency;
            }
            else if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.None ||
                     MultiplayerManager.Instance.CurrentServer.ConnectedPlayers.Count == 0)
            {
                return 0;
            }
            else
            {
                return MultiplayerManager.Instance.CurrentServer.ConnectedPlayers.Values.Max(player => player.Latency);
            }
        }

        /// <summary>
        ///     Computes the minimum network latency of connected clients.
        ///     When acting as a server with connected clients, this will return the minimum latency to one of the clients.
        ///     When acting as a client, this returns the latency to the server.
        ///     Otherwise this returns 0.
        /// </summary>
        /// <returns>The minimum network latency.</returns>
        private static long GetMinimumLatency()
        {
            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
            {
                return MultiplayerManager.Instance.CurrentClient.ClientPlayer.Latency;
            }
            else if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.None ||
                     MultiplayerManager.Instance.CurrentServer.ConnectedPlayers.Count == 0)
            {
                return 0;
            }
            else
            {
                return MultiplayerManager.Instance.CurrentServer.ConnectedPlayers.Values.Min(player => player.Latency);
            }
        }

        /// <summary>
        ///     Computes how much in game time will pass during the given amount of milliseconds.
        ///     This also considers the current game speed.
        /// </summary>
        /// <param name="millis">The amount of milliseconds.</param>
        /// <returns>The in game TimeSpan equal to the given amount of milliseconds.</returns>
        private static TimeSpan MillisecondsToInGameTime(long millis)
        {
            TimeSpan span = new TimeSpan((long) ((millis / Time.fixedDeltaTime / 1000d) *      // Number of ticks
                                         SimulationManager.instance.FinalSimulationSpeed *     // Frames per tick
                                         SimulationManager.instance.m_timePerFrame.Ticks));    // Time per frame
            return span;
        }

        /// <summary>
        ///     Initializes the random number generator for the request ids.
        ///     On the client, it is initialized using the client id.
        ///     On the server, no seed is given.
        ///     This is used to prevent clients from generating the same number
        ///     when both people click on a button at the same time.
        /// </summary>
        private static void InitRand()
        {
            if (_rand == null)
            {
                if (MultiplayerManager.Instance.CurrentRole != MultiplayerRole.Client)
                {
                    _rand = new System.Random();
                }
                else
                {
                    _rand = new System.Random(MultiplayerManager.Instance.CurrentClient.ClientId);
                }
            }
        }

        private enum SpeedPauseState
        {
            /// <summary>
            ///     The game is paused and button clicks are accepted.
            /// </summary>
            Paused,
            /// <summary>
            ///     The play state is requested, we wait for other games to respond to the request.
            ///     No button clicks are allowed.
            /// </summary>
            PlayRequested,
            /// <summary>
            ///     The game is waiting until waitTargetTime (realtime) is reached to change the state to Playing.
            ///     No button clicks are allowed.
            /// </summary>
            WaitingForPlay,
            /// <summary>
            ///     The game is running and button clicks are accepted.
            /// </summary>
            Playing,
            /// <summary>
            ///     The pause state is requested, we wait for other games to respond to the request.
            ///     No button clicks are allowed.
            /// </summary>
            PauseRequested,
            /// <summary>
            ///     The game is waiting until waitTargetTime (in game time) is reached to change the state to Paused.
            ///     No button clicks are allowed.
            /// </summary>
            WaitingForPause,
            /// <summary>
            ///     A speed change during Playing is requested, we wait for other games to respond to the request.
            ///     No button clicks are allowed.
            /// </summary>
            SpeedChangeRequested,
            /// <summary>
            ///     The game is waiting until waitTargetTime (in game time) is reached to change the speed
            ///     and the state back to Playing.
            ///     No button clicks are allowed.
            /// </summary>
            WaitingForSpeedChange,
            /// <summary>
            ///     The game has changed the state to Playing but is still waiting for other games to reach this state.
            ///     No button clicks are allowed.
            /// </summary>
            PlayingWaiting,
            /// <summary>
            ///     The game has changed the state to Paused but is still waiting for other games to reach this state.
            ///     No button clicks are allowed.
            /// </summary>
            PausedWaiting
        }
    }
}
