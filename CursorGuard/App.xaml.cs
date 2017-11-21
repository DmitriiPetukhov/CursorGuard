using System;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace CursorGuard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private NotifyIcon trayIcon;
        private readonly IForegroundWindowMonitor windowMonitor = new ForegroundWindowMonitor();
        private readonly IWindowProcessLocator processLocator = new WindowProcessLocator();
        private readonly IConfigurationManager configurationManager = new ConfigurationManager();

        private MainWindow mainWindow;
        
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            InitializeTrayIcon();

            windowMonitor.StartMonitoringAsync();
            windowMonitor.ForegroundWindowInfoUpdated += windowInfo =>
            {
                var process = processLocator.GetProcessInfo(windowInfo);
                string filePath;
                try
                {
                    filePath = process.MainModule.FileName;
                }
                catch (Exception ex)
                {
                    return;
                }

                var profile = configurationManager.GetProfileForExecutable(filePath);
                if (profile == null)
                {
                    return;
                }

                Cursor.Position = new System.Drawing.Point(
                    Math.Min(Math.Max(Cursor.Position.X, windowInfo.Left), windowInfo.Right),
                    Math.Min(Math.Max(Cursor.Position.Y, windowInfo.Top), windowInfo.Bottom));
            };
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            trayIcon.Visible = false;
            windowMonitor.Dispose();
        }

        private void InitializeTrayIcon()
        {
            trayIcon = new NotifyIcon
            {
                // https://findicons.com/icon/258922/cursor
                Icon = CursorGuard.Properties.Resources.cursor,
                Text = "Cursor Guard"
            };
            trayIcon.ContextMenu = new ContextMenu(new[]
            {
                new MenuItem("Options", DisplayOptionsWindow), 
                new MenuItem("-"),
                new MenuItem("Exit", (sender, args) => this.Shutdown()),
            });
            trayIcon.Visible = true;
        }

        private void DisplayOptionsWindow(object sender, EventArgs e)
        {
            if (this.mainWindow == null)
            {
                this.mainWindow = new MainWindow(windowMonitor, processLocator);
                mainWindow.Closed += (o, args) =>
                {
                    this.mainWindow = null;
                };
                this.mainWindow.ShowDialog();
            }
            else
            {
                this.mainWindow.Activate();
            }
        }
    }
}
