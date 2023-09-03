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
        public static IgnoreHelper Instance = new IgnoreHelper();

        private readonly ThreadLocal<int> _ignoreAll = new ThreadLocal<int>();
        private readonly ThreadLocal<HashSet<string>> _exceptions = new ThreadLocal<HashSet<string>>();

        /// <summary>
        ///     Starts the ignore mode where the injection handlers
        ///     ignore all method calls.
        /// </summary>
        public void StartIgnore()
        {
            _ignoreAll.Value++;
        }

        /// <summary>
        ///     Starts the ignore mode where the injection handlers
        ///     ignore all method calls.
        /// </summary>
        /// <param name="except">An action that should still be allowed.</param>
        public void StartIgnore(string except)
        {
            StartIgnore();
            if (_exceptions.Value == null)
            {
                _exceptions.Value = new HashSet<string>();
            }
            _exceptions.Value.Add(except);
        }

        /// <summary>
        ///     Stop the ignore mode where the injection handlers
        ///     ignore all method calls.
        /// </summary>
        public void EndIgnore()
        {
            _ignoreAll.Value = Math.Max(_ignoreAll.Value - 1, 0);
        }

        /// <summary>
        ///     Stop the ignore mode where the injection handlers
        ///     ignore all method calls.
        /// </summary>
        /// <param name="except">The action that should be removed from the exceptions.</param>
        public void EndIgnore(string except)
        {
            EndIgnore();
            if (_exceptions.Value == null)
            {
                _exceptions.Value = new HashSet<string>();
            }
            _exceptions.Value.Remove(except);
        }

        /// <summary>
        ///     Checks if the injection handlers should ignore all method calls.
        /// </summary>
        /// <returns>If the calls should be ignored.</returns>
        public bool IsIgnored()
        {
            return _ignoreAll.Value > 0;
        }

        /// <summary>
        ///     Checks if the injection handlers should ignore all method calls.
        /// </summary>
        /// <param name="action">The current action (not ignored when in list of exceptions)</param>
        /// <returns>If the calls should be ignored.</returns>
        public bool IsIgnored(string action)
        {
            if (_exceptions.Value == null)
            {
                _exceptions.Value = new HashSet<string>();
            }
            return IsIgnored() && !_exceptions.Value.Contains(action);
        }

        /// <summary>
        ///     Reset ignore state.
        /// </summary>
        public void ResetIgnore()
        {
            if (_exceptions.Value == null)
            {
                _exceptions.Value = new HashSet<string>();
            }

            Log.Debug($"Resetting {_ignoreAll.Value} levels of ignoring: {string.Join(", ", _exceptions.Value.ToArray())}");
            _ignoreAll.Value = 0;
            _exceptions.Value.Clear();
        }
    }
}
