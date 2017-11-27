using CursorGuard.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using static CursorGuard.OptionsWindowModel;

namespace CursorGuard
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public sealed partial class OptionsWindow : Window
    {
        private IConfigurationManager configurationManager;

        public OptionsWindow(IConfigurationManager configurationManager)
        {
            this.InitializeComponent();
            this.configurationManager = Ensure.ArgumentNotNull(configurationManager, nameof(configurationManager));
        }

        public OptionsWindowModel Model { get; private set; } = new OptionsWindowModel();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            this.Model.Profiles = new ObservableCollection<ApplicationProfileObservable>(
                this.configurationManager.GetApplicationProfiles().Select(ProfilesToObservable));
        }

        private ApplicationProfileObservable ProfilesToObservable(ApplicationProfile profile)
        {
            if (profile == null) return null;

            return new ApplicationProfileObservable()
            {
                ExecutablePath = profile.ExecutablePath
            };
        }        
    }

    public sealed class OptionsWindowModel : INotifyPropertyChanged
    {
        private ObservableCollection<ApplicationProfileObservable> profiles;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<ApplicationProfileObservable> Profiles
        {
            get
            {
                return this.profiles;
            }

            set
            {
                if (value == this.profiles) return;
                this.profiles = value;
                OnPropertyChanged(nameof(this.Profiles));
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class ApplicationProfileObservable : INotifyPropertyChanged
        {
            private string executablePath;

            public event PropertyChangedEventHandler PropertyChanged;

            public string ExecutablePath
            {
                get
                {
                    return this.executablePath;
                }

                set
                {
                    if (this.executablePath == value) return;
                    this.executablePath = value;
                    OnPropertyChanged(nameof(this.ExecutablePath));
                }
            }

            private void OnPropertyChanged(string propertyName)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
