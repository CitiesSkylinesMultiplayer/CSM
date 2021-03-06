using System;
using System.Collections.Generic;

namespace CSM.Helpers
{
    /// <summary>
    ///     Helper class for keeping track if the injection handlers should ignore
    ///     all method calls. Can handle nested combinations of Start/End calls.
    /// </summary>
    public static class IgnoreHelper
    {
        private static int IgnoreAll = 0;
        private static readonly HashSet<string> Exceptions = new HashSet<string>();

        /// <summary>
        ///     Starts the ignore mode where the injection handlers
        ///     ignore all method calls.
        /// </summary>
        public static void StartIgnore()
        {
            IgnoreAll++;
        }

        /// <summary>
        ///     Starts the ignore mode where the injection handlers
        ///     ignore all method calls.
        /// </summary>
        /// <param name="except">An action that should still be allowed.</param>
        public static void StartIgnore(string except)
        {
            StartIgnore();
            Exceptions.Add(except);
        }

        /// <summary>
        ///     Stop the ignore mode where the injection handlers
        ///     ignore all method calls.
        /// </summary>
        public static void EndIgnore()
        {
            IgnoreAll = Math.Max(IgnoreAll - 1, 0);
        }

        /// <summary>
        ///     Stop the ignore mode where the injection handlers
        ///     ignore all method calls.
        /// </summary>
        /// <param name="except">The action that should be removed from the exceptions.</param>
        public static void EndIgnore(string except)
        {
            EndIgnore();
            Exceptions.Remove(except);
        }

        /// <summary>
        ///     Checks if the injection handlers should ignore all method calls.
        /// </summary>
        /// <returns>If the calls should be ignored.</returns>
        public static bool IsIgnored()
        {
            return IgnoreAll > 0;
        }

        /// <summary>
        ///     Checks if the injection handlers should ignore all method calls.
        /// </summary>
        /// <param name="action">The current action (not ignored when in list of exceptions)</param>
        /// <returns>If the calls should be ignored.</returns>
        public static bool IsIgnored(string action)
        {
            return IsIgnored() && !Exceptions.Contains(action);
        }
    }
}
