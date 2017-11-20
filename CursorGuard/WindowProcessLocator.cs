using System;
using System.Diagnostics;
using CursorGuard.Helpers;

namespace CursorGuard
{
    internal class WindowProcessLocator : IWindowProcessLocator
    {
        public ProcessInfo GetProcessInfo(ForegroundWindowInfo windowInfo)
        {
            Ensure.ArgumentNotNull(windowInfo, nameof(windowInfo));

            User32.GetWindowThreadProcessId(windowInfo.Handle, out int processId);

            var process = Process.GetProcessById(processId);

            return new ProcessInfo
            {
                ProcessId = processId
            };
        }
    }

    public interface IWindowProcessLocator
    {
        ProcessInfo GetProcessInfo(ForegroundWindowInfo windowInfo);
    }

    public class ProcessInfo
    {
        public int ProcessId { get; set; }
    }
}
