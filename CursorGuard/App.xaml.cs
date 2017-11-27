using System;
using System.Diagnostics;
using System.Drawing;
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
        private const int windowBorder = 8;

        private readonly IForegroundWindowMonitor windowMonitor = new ForegroundWindowMonitor();
        private readonly IWindowProcessLocator processLocator = new WindowProcessLocator();
        private readonly IConfigurationManager configurationManager = new ConfigurationManager();

        private MainWindow mainWindow;
        private OptionsWindow optionsWindow;
        private NotifyIcon trayIcon;

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            InitializeTrayIcon();

            windowMonitor.StartMonitoringAsync();
            windowMonitor.ForegroundWindowInfoUpdated += windowInfo =>
            {
                var process = processLocator.GetProcessInfo(windowInfo);
                ApplicationProfile profile = null;
                if (process != null)
                {
                    profile = configurationManager.GetProfileForExecutable(process.ExecutablePath);
                }
                
                if (process == null || profile == null)
                {
                    Debug.WriteLine("Process: " + process?.ExecutablePath);
                    Debug.WriteLine("No process or profile. Resetting clipping.");
                    // reset cursor clip
                    Cursor.Clip = new Rectangle();
                    return;
                }

                Debug.WriteLine("Setting window clipping.");
                Cursor.Clip = new System.Drawing.Rectangle(
                    new System.Drawing.Point(windowInfo.Left + windowBorder, windowInfo.Top),
                    new System.Drawing.Size(
                        windowInfo.Right - windowInfo.Left - windowBorder * 2,
                        windowInfo.Bottom - windowInfo.Top - windowBorder));
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
                new MenuItem("Debug", DisplayDebugWindow), 
                new MenuItem("-"),
                new MenuItem("Exit", (sender, args) => this.Shutdown()),
            });
            trayIcon.Visible = true;
        }

        private void DisplayOptionsWindow(object sender, EventArgs e)
        {
            if (this.optionsWindow == null)
            {
                EventHandler windowClosedHandler = null;
                windowClosedHandler = (o, args) =>
                {
                    this.optionsWindow.Closed -= windowClosedHandler;
                    this.optionsWindow = null;                    
                };

                this.optionsWindow = new OptionsWindow(configurationManager);
                this.optionsWindow.Closed += windowClosedHandler;
                this.optionsWindow.ShowDialog();
            }
            else
            {
                if (this.optionsWindow.WindowState == WindowState.Minimized)
                    this.optionsWindow.WindowState = WindowState.Normal;

                this.optionsWindow.Activate();
            }
        }

        private void DisplayDebugWindow(object sender, EventArgs e)
        {
            if (this.mainWindow == null)
            {
                EventHandler windowClosedHandler = null;
                windowClosedHandler = (o, args) =>
                {
                    this.mainWindow.Closed -= windowClosedHandler;
                    this.mainWindow = null;                    
                };

                this.mainWindow = new MainWindow(windowMonitor, processLocator);
                this.mainWindow.Closed += windowClosedHandler;
                this.mainWindow.ShowDialog();
            }
            else
            {
                if (this.mainWindow.WindowState == WindowState.Minimized)
                    this.mainWindow.WindowState = WindowState.Normal;

                this.mainWindow.Activate();
            }
        }
    }
}
