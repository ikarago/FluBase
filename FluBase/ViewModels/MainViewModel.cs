using System;
using System.Windows.Input;
using FluBase.Helpers;
using FluBase.Views.Dialogs;

namespace FluBase.ViewModels
{
    public class MainViewModel : Observable
    {
        // Properties


        // Constructor
        public MainViewModel()
        {
            Initialize();
        }

        // Initialize
        private void Initialize()
        {
            // Currently nothing
        }


        // Commands
        private ICommand _aboutCommand;
        public ICommand AboutCommand
        {
            get
            {
                if (_aboutCommand == null)
                {
                    _aboutCommand = new RelayCommand(
                        () =>
                        {
                            ShowAboutDialog();
                        });
                }
                return _aboutCommand;
            }
        }

        private ICommand _settingsCommand;
        public ICommand SettingsCommand
        {
            get
            {
                if (_settingsCommand == null)
                {
                    _settingsCommand = new RelayCommand(
                        () =>
                        {
                            ShowSettingsDialog();
                        });
                }
                return _settingsCommand;
            }
        }


        // Methods
        private async void ShowAboutDialog()
        {
            var dialog = new AboutDialog();
            await dialog.ShowAsync();
        }

        private async void ShowSettingsDialog()
        {
            var dialog = new SettingsDialog();
            await dialog.ShowAsync();
        }


    }
}
