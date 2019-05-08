﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluBase.Activation;
using FluBase.Helpers;

using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace FluBase.Services
{
    // For more information on application activation see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/activation.md
    internal class ActivationService
    {
        private readonly App _app;
        private readonly Lazy<UIElement> _shell;
        private readonly Type _defaultNavItem;

        public static readonly KeyboardAccelerator AltLeftKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu);

        public static readonly KeyboardAccelerator BackKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.GoBack);

        public ActivationService(App app, Type defaultNavItem, Lazy<UIElement> shell = null)
        {
            _app = app;
            _shell = shell;
            _defaultNavItem = defaultNavItem;
        }

        public async Task ActivateAsync(object activationArgs)
        {
            if (IsInteractive(activationArgs))
            {
                // Initialize things like registering background task before the app is loaded
                await InitializeAsync();

                // Do not repeat app initialization when the Window already has content,
                // just ensure that the window is active
                if (Window.Current.Content == null)
                {
                    // Create a Frame to act as the navigation context and navigate to the first page
                    Window.Current.Content = _shell?.Value ?? new Frame();
                    NavigationService.NavigationFailed += (sender, e) =>
                    {
                        throw e.Exception;
                    };
                    NavigationService.Navigated += Frame_Navigated;
                    if (SystemNavigationManager.GetForCurrentView() != null)
                    {
                        SystemNavigationManager.GetForCurrentView().BackRequested += ActivationService_BackRequested;
                    }
                }
            }

            var activationHandler = GetActivationHandlers()
                                                .FirstOrDefault(h => h.CanHandle(activationArgs));

            if (activationHandler != null)
            {
                await activationHandler.HandleAsync(activationArgs);
            }

            if (IsInteractive(activationArgs))
            {
                var defaultHandler = new DefaultLaunchActivationHandler(_defaultNavItem);
                if (defaultHandler.CanHandle(activationArgs))
                {
                    await defaultHandler.HandleAsync(activationArgs);
                }

                // Set custom view stuff
                SetTitlebar();
                
                // Ensure the current window is active
                Window.Current.Activate();

                // Tasks after activation
                await StartupAsync();
            }
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = NavigationService.CanGoBack ?
                AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

        private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
        {
            var keyboardAccelerator = new KeyboardAccelerator() { Key = key };
            if (modifiers.HasValue)
            {
                keyboardAccelerator.Modifiers = modifiers.Value;
            }

            keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;
            return keyboardAccelerator;
        }

        private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var result = NavigationService.GoBack();
            args.Handled = result;
        }

        private void ActivationService_BackRequested(object sender, BackRequestedEventArgs e)
        {
            var result = NavigationService.GoBack();
            e.Handled = result;
        }

        private async Task InitializeAsync()
        {
            await ThemeSelectorService.InitializeAsync();
        }

        private async Task StartupAsync()
        {
            await ThemeSelectorService.SetRequestedThemeAsync();
            await WhatsNewDisplayService.ShowIfAppropriateAsync();
            await FirstRunDisplayService.ShowIfAppropriateAsync();
        }

        private IEnumerable<ActivationHandler> GetActivationHandlers()
        {
            yield return Singleton<SuspendAndResumeService>.Instance;
        }

        private bool IsInteractive(object args)
        {
            return args is IActivatedEventArgs;
        }

        private void SetTitlebar()
        {
            // Make the buttons transparent
            ApplicationViewTitleBar titlebar = ApplicationView.GetForCurrentView().TitleBar;
            titlebar.ButtonBackgroundColor = Colors.Transparent;
            titlebar.ButtonInactiveBackgroundColor = Colors.Transparent;

            if (App.Current.RequestedTheme == ApplicationTheme.Dark)
            {
                titlebar.ButtonForegroundColor = Colors.White;
            }
            else if (App.Current.RequestedTheme == ApplicationTheme.Light)
            {
                titlebar.ButtonForegroundColor = Colors.Black;
            }

            // Extend the normal window to the Titlebar for the blur to reach there too
            CoreApplicationViewTitleBar coreTitlebar = CoreApplication.GetCurrentView().TitleBar;
            coreTitlebar.ExtendViewIntoTitleBar = true;
        }
    }
}
