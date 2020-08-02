using CSM.Commands;
using CSM.Commands.Data.Internal;
using NLog;
using UnityEngine;

namespace CSM.Helpers
{
    /// <summary>
    ///     This class takes care of slowing the game down if another player's game runs too slow.
    ///     This happens by keeping track of dropped frames on the slower client and also
    ///     dropping them on the other clients.
    /// </summary>
    public static class SlowdownHelper
    {
        private static readonly NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        private static int _locallyDroppedFrames = 0;
        private static readonly object _localDropLock = new object();

        private static int _framesToDrop = 0;
        private static readonly object _toDropLock = new object();
        private static int _dropInterval;
        private static int _curTickInc = 0;
        
        /// <summary>
        ///     Called by the TickLoopHandler when a frame was dropped.
        ///     Increases the queued dropped frame count that will later be sent to other clients.
        /// </summary>
        public static void FrameDropped()
        {
            lock (_localDropLock)
            {
                _locallyDroppedFrames++;
            }
        }

        /// <summary>
        ///     Send queued dropped frame count to other clients.
        /// </summary>
        public static void SendDroppedFrames()
        {
            int dropped;
            lock (_localDropLock)
            {
                dropped = _locallyDroppedFrames;
                _locallyDroppedFrames = 0;
            }

            if (dropped > 0)
            {
                _logger.Debug($"{dropped} dropped frames!");
                Command.SendToAll(new SlowdownCommand()
                {
                    DroppedFrames = dropped
                });
            }
        }

        /// <summary>
        ///     Adds a number of frames to drop (they were dropped on another client).
        ///     Also computes the _dropInterval at which the frames will be dropped.
        ///     It is chosen so that the queued frames are evenly dropped within ten seconds.
        /// </summary>
        /// <param name="frames">The number of frames.</param>
        public static void AddDropFrames(int frames)
        {
            int ticksUntil = (int) (10 * (1 / Time.fixedDeltaTime)); // 10 seconds * ticks per second (1/tick interval)
            lock (_toDropLock)
            {
                _framesToDrop += frames;
                _dropInterval = ticksUntil / _framesToDrop;
            }
            _logger.Debug($"Dropping {_framesToDrop} frames in the next 10 seconds (drop every {_dropInterval} frames)");
        }

        /// <summary>
        ///     Checks if a frame needs to be dropped.
        ///     Every _dropInterval a frame is dropped.
        /// </summary>
        /// <returns>If the current frame needs to be dropped.</returns>
        public static bool CheckDropAndReduce()
        {
            _curTickInc++;
            lock (_toDropLock)
            {
                if (_framesToDrop > 0 && _curTickInc >= _dropInterval)
                {
                    _framesToDrop--;
                    _curTickInc = 0;
                    return true;
                }
                return false;
            }
        }
    }
}
