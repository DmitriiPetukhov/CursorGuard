using System;
using System.Linq;
using System.Management;
using CursorGuard.Helpers;

namespace CursorGuard
{
    internal class WindowProcessLocator : IWindowProcessLocator
    {
        public ProcessInfo GetProcessInfo(ForegroundWindowInfo windowInfo)
        {
            Ensure.ArgumentNotNull(windowInfo, nameof(windowInfo));

            User32.GetWindowThreadProcessId(windowInfo.Handle, out int processId);
            var processExecutablePath = GetMainModuleFilepath(processId);

            if (processId == 0 || string.IsNullOrEmpty(processExecutablePath))
            {
                return null;
            }

            return new ProcessInfo
            {
                ProcessId = processId,
                ExecutablePath = processExecutablePath
            };
        }

        private string GetMainModuleFilepath(int processId)
        {
            string wmiQueryString = "SELECT ProcessId, ExecutablePath FROM Win32_Process WHERE ProcessId = " + processId;
            using (var searcher = new ManagementObjectSearcher(wmiQueryString))
            {
                using (var results = searcher.Get())
                {
                    ManagementObject mo = results.Cast<ManagementObject>().FirstOrDefault();
                    if (mo != null)
                    {
                        return (string)mo["ExecutablePath"];
                    }
                }
            }
            return null;
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
        ProcessInfo GetProcessInfo(ForegroundWindowInfo windowInfo);
    }

    public class ProcessInfo
    {
        public int ProcessId { get; set; }

        public string ExecutablePath { get; set; }
    }
}
