using FluBase.Helpers;
using FluBase.ViewModels;

using System;
using System.Windows.Input;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace FluBase.Views.Dialogs
{
    public sealed partial class AboutDialog : ContentDialog
    {
        // Properties
        public AboutViewModel ViewModel { get; } = new AboutViewModel();


        // Constructor
        public AboutDialog()
        {
            RequestedTheme = (Window.Current.Content as FrameworkElement).RequestedTheme;
            this.InitializeComponent();

            // Theme trigger for the logo
            this.ActualThemeChanged += AboutDialog_ActualThemeChanged;
            CheckThemeForLogo();
        }


        // Commands
        private ICommand _closeDialogCommand;
        public ICommand CloseDialogCommand
        {
            get
            {
                if (_closeDialogCommand == null)
                {
                    _closeDialogCommand = new RelayCommand(
                        () =>
                        {
                            Hide();
                        });
                }
                return _closeDialogCommand;
            }
        }


        // Methods
        private void AboutDialog_ActualThemeChanged(FrameworkElement sender, object args)
        {
            CheckThemeForLogo();
        }

        private void CheckThemeForLogo()
        {
            // Change the displayed logo
            if (ActualTheme == ElementTheme.Dark)
            {
                BitmapImage image = new BitmapImage(new Uri("ms-appx:///Assets/Logo/in-app/logo-white.png"));
                imgLogo.Source = image;
            }
            else if (ActualTheme == ElementTheme.Light)
            {
                BitmapImage image = new BitmapImage(new Uri("ms-appx:///Assets/Logo/in-app/logo-black.png"));
                imgLogo.Source = image;
            }
        }
    }
}
