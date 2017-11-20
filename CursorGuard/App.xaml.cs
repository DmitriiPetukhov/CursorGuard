using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Navigation;
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

        private MainWindow mainWindow;
        
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            InitializeTrayIcon();
            windowMonitor.StartMonitoringAsync();
            windowMonitor.ForegroundWindowChanged += windowInfo =>
            {
                
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
