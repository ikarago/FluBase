using System;

using FluBase.ViewModels;

using Windows.UI.Xaml.Controls;

namespace FluBase.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
