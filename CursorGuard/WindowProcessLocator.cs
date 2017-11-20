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

    public interface IWindowProcessLocator
    {
        Process GetProcessInfo(ForegroundWindowInfo windowInfo);
    }
}
