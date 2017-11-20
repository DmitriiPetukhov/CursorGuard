using System;
using System.Diagnostics;
using CursorGuard.Helpers;

namespace CursorGuard
{
    internal class WindowProcessLocator : IWindowProcessLocator
    {
        public Process GetProcessInfo(ForegroundWindowInfo windowInfo)
        {
            Ensure.ArgumentNotNull(windowInfo, nameof(windowInfo));

            User32.GetWindowThreadProcessId(windowInfo.Handle, out int processId);

            return Process.GetProcessById(processId);
        }
    }

    /// <summary>
    /// Provides process information
    /// </summary>
    public interface IWindowProcessLocator
    {
        /// <summary>
        /// Gets process information by foreground window information
        /// </summary>
        /// <param name="windowInfo">Foreground window information</param>
        Process GetProcessInfo(ForegroundWindowInfo windowInfo);
    }
}
