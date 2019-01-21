using System.Threading.Tasks;
using System.Windows.Input;

using FluBase.Helpers;
using FluBase.Services;

using Windows.UI.Xaml;

namespace FluBase.ViewModels
{
    // TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/pages/settings.md
    public class SettingsViewModel : Observable
    {
        // Properties
        private bool _hasInstanceBeenInitialized = false;

        private ElementTheme _elementTheme = ThemeSelectorService.Theme;
        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { Set(ref _elementTheme, value); }
        }

        private string _versionDescription;
        public string VersionDescription
        {
            get { return _versionDescription; }

            set { Set(ref _versionDescription, value); }
        }


        // Constructor
        public SettingsViewModel()
        {
            Initialize();
        }

        // Initialize
        public void Initialize()
        {
            // Empty... for now :)
        }


        // Commands
        private ICommand _switchThemeCommand;
        public ICommand SwitchThemeCommand
        {
            get
            {
                if (_switchThemeCommand == null)
                {
                    _switchThemeCommand = new RelayCommand<ElementTheme>(
                        async (param) =>
                        {
                            ElementTheme = param;
                            await ThemeSelectorService.SetThemeAsync(param);
                        });
                }

                return _switchThemeCommand;
            }
        }


        // Methods
        public async Task EnsureInstanceInitializedAsync()
        {
            if (!_hasInstanceBeenInitialized)
            {
                Initialize();

                _hasInstanceBeenInitialized = true;
            }
        }
    }
}
