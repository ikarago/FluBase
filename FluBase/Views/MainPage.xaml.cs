using System;

using FluBase.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace FluBase.Views
{
    public sealed partial class MainPage : Page
    {
        // Properties
        public MainViewModel ViewModel { get; } = new MainViewModel();


        // Constructor
        public MainPage()
        {
            InitializeComponent();
            Initialize();
        }

        // Initialize
        private void Initialize()
        {
            // Put in triggers for the logo
            this.ActualThemeChanged += Page_ActualThemeChanged;
            CheckThemeForLogo();
        }


        // Commands


        // Methods
        private void Page_ActualThemeChanged(FrameworkElement sender, object args)
        {
            CheckThemeForLogo();
        }

        private void CheckThemeForLogo()
        {
            // Change the displayed logo
            if (ActualTheme == ElementTheme.Dark)
            {
                BitmapImage image = new BitmapImage(new Uri("ms-appx:///Assets/Logo/contrast-black/Square44x44Logo.altform-unplated_targetsize-256.png"));
                imgAppIcon.Source = image;
            }
            else if (ActualTheme == ElementTheme.Light)
            {
                BitmapImage image = new BitmapImage(new Uri("ms-appx:///Assets/Logo/contrast-white/Square44x44Logo.altform-unplated_targetsize-256.png"));
                imgAppIcon.Source = image;
            }
        }
    }
}
