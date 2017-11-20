using System;
using System.Threading;
using System.Threading.Tasks;
using CursorGuard.Helpers;

namespace CursorGuard
{
    internal class ForegroundWindowMonitor : IForegroundWindowMonitor
    {
        public event Action<ForegroundWindowInfo> ForegroundWindowChanged;

        private Task monitoringTask;
        private CancellationTokenSource tokenSource;
        private bool started;
        private bool disposed;

        public void StartMonitoringAsync()
        {
            Ensure.Precondition<InvalidOperationException>(!started, "Monitoring is already active");
            Ensure.Precondition<InvalidOperationException>(!disposed, "Object is already disposed");

            started = true;

            tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            monitoringTask = Task.Run(() => MonitorWindows(token), token);
        }

        
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            tokenSource.Cancel();

            try
            {
                monitoringTask.Wait();
            }
            catch (AggregateException)
            {
                // ignore
            }

            monitoringTask?.Dispose();
            disposed = true;
        }

        protected virtual void OnForegroundWindowChanged(ForegroundWindowInfo windowInfo)
        {
            ForegroundWindowChanged?.Invoke(windowInfo);
        }

        private void MonitorWindows(CancellationToken ct)
        {
            for (;;)
            {
                ct.ThrowIfCancellationRequested();

                var foregroundHandle = User32.GetForegroundWindow();

                User32.RECT rect = new User32.RECT();
                User32.GetWindowRect(foregroundHandle, ref rect);

                OnForegroundWindowChanged(new ForegroundWindowInfo
                {
                    Handle = foregroundHandle,
                    Left = rect.Left,
                    Top = rect.Top,
                    Right = rect.Right,
                    Bottom = rect.Bottom
                });

                Task.Delay(100, ct);
            }
        }
    }

    public interface IForegroundWindowMonitor : IDisposable
    {
        /// <summary>
        /// Start monitoring
        /// </summary>
        void StartMonitoringAsync();

        /// <summary>
        /// Fires when foreground window changed
        /// </summary>
        event Action<ForegroundWindowInfo> ForegroundWindowChanged;
    }
}
