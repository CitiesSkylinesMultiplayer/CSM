using System;

namespace CSM.Helpers
{
    /// <summary>
    /// Helper class for keeping track if the injection handlers should ignore
    /// all method calls. Can handle nested combinations of Start/End calls.
    /// </summary>
    public static class IgnoreHelper
    {
        private static int IgnoreAll = 0;

        /// <summary>
        /// Starts the ignore mode where the injection handlers
        /// ignore all method calls.
        /// </summary>
        public static void StartIgnore()
        {
            IgnoreAll++;
        }

        /// <summary>
        /// Stop the ignore mode where the injection handlers
        /// ignore all method calls.
        /// </summary>
        public static void EndIgnore()
        {
            IgnoreAll = Math.Max(IgnoreAll - 1, 0);
        }

        /// <summary>
        /// Checks if the injection handlers should ignore all method calls.
        /// </summary>
        /// <returns>If the calls should be ignored.</returns>
        public static bool IsIgnored()
        {
            return IgnoreAll > 0;
        }
    }
}
