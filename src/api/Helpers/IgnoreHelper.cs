using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CSM.API.Helpers
{
    /// <summary>
    ///     Helper class for keeping track if the injection handlers should ignore
    ///     all method calls. Can handle nested combinations of Start/End calls.
    /// </summary>
    public class IgnoreHelper
    {
        public static IgnoreHelper Instance
        {
            get => _instance.Value;
            set => _instance.Value = value;
        }

        private static readonly ThreadLocal<IgnoreHelper> _instance = new ThreadLocal<IgnoreHelper>(() => new IgnoreHelper());

        private int _ignoreAll = 0;
        private readonly HashSet<string> _exceptions = new HashSet<string>();

        /// <summary>
        ///     Starts the ignore mode where the injection handlers
        ///     ignore all method calls.
        /// </summary>
        public void StartIgnore()
        {
            _ignoreAll++;
        }

        /// <summary>
        ///     Starts the ignore mode where the injection handlers
        ///     ignore all method calls.
        /// </summary>
        /// <param name="except">An action that should still be allowed.</param>
        public void StartIgnore(string except)
        {
            StartIgnore();
            _exceptions.Add(except);
        }

        /// <summary>
        ///     Stop the ignore mode where the injection handlers
        ///     ignore all method calls.
        /// </summary>
        public void EndIgnore()
        {
            _ignoreAll = Math.Max(_ignoreAll - 1, 0);
        }

        /// <summary>
        ///     Stop the ignore mode where the injection handlers
        ///     ignore all method calls.
        /// </summary>
        /// <param name="except">The action that should be removed from the exceptions.</param>
        public void EndIgnore(string except)
        {
            EndIgnore();
            _exceptions.Remove(except);
        }

        /// <summary>
        ///     Checks if the injection handlers should ignore all method calls.
        /// </summary>
        /// <returns>If the calls should be ignored.</returns>
        public bool IsIgnored()
        {
            return _ignoreAll > 0;
        }

        /// <summary>
        ///     Checks if the injection handlers should ignore all method calls.
        /// </summary>
        /// <param name="action">The current action (not ignored when in list of exceptions)</param>
        /// <returns>If the calls should be ignored.</returns>
        public bool IsIgnored(string action)
        {
            return IsIgnored() && !_exceptions.Contains(action);
        }

        /// <summary>
        ///     Reset ignore state.
        /// </summary>
        public void ResetIgnore()
        {
            Log.Debug($"Resetting {_ignoreAll} levels of ignoring: {string.Join(", ", _exceptions.ToArray())}");
            _ignoreAll = 0;
            _exceptions.Clear();
        }
    }
}
