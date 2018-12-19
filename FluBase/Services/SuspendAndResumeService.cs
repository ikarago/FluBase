﻿using System;
using System.Reflection;
using System.Threading.Tasks;

using FluBase.Activation;
using FluBase.Helpers;

using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace FluBase.Services
{
    // The SuspendAndResumeService allows you to save the App data before the App is being suspended (or enters in background state).
    // In case the App is terminated during suspension, the data is restored during App launch by this ActivationHandler.
    // In case the App is resumed without being terminated no data should be lost, a resume event is fired that allows you to refresh App data that might
    // be outdated (e.g data from online feeds)
    // Documentation:
    //     * How to implement and test: https://github.com/Microsoft/WindowsTemplateStudio/blob/dev/docs/features/suspend-and-resume.md
    //     * Application Lifecycle: https://docs.microsoft.com/windows/uwp/launch-resume/app-lifecycle
    internal class SuspendAndResumeService : ActivationHandler<LaunchActivatedEventArgs>
    {
        private const string StateFilename = "SuspendAndResumeState";

        // TODO WTS: Subscribe to the OnBackgroundEntering event from your current Page to save the current App data.
        // Only one Page should subscribe to OnBackgroundEntering at a time, as the App will navigate to that Page on resume.
        public event EventHandler<OnBackgroundEnteringEventArgs> OnBackgroundEntering;

        // TODO WTS: Subscribe to the OnResuming event from the current Page
        // if you need to refresh online data when the App resumes without being terminated.
        public event EventHandler OnResuming;

        // This method saves the application state before entering background state. It fires the event OnBackgroundEntering to collect
        // state data from the current subscriber and saves it to the local storage.
        public async Task SaveStateAsync()
        {
            var suspensionState = new SuspensionState()
            {
                SuspensionDate = DateTime.Now
            };

            var target = OnBackgroundEntering?.Target.GetType();
            var onBackgroundEnteringArgs = new OnBackgroundEnteringEventArgs(suspensionState, target);

            OnBackgroundEntering?.Invoke(this, onBackgroundEnteringArgs);

            await ApplicationData.Current.LocalFolder.SaveAsync(StateFilename, onBackgroundEnteringArgs);
        }

        // This method allows subscribers to refesh data that might be outdated when the App is resuming from suspension.
        // If the App was terminated during suspension this event will not fire, data restore is handled by the method HandleInternalAsync.
        public void ResumeApp()
        {
            OnResuming?.Invoke(this, EventArgs.Empty);
        }

        // This method restores application state when the App is launched after termination, it navigates to the stored Page passing the recovered state data.
        protected override async Task HandleInternalAsync(LaunchActivatedEventArgs args)
        {
            var saveState = await ApplicationData.Current.LocalFolder.ReadAsync<OnBackgroundEnteringEventArgs>(StateFilename);
            if (saveState?.Target != null && typeof(Page).IsAssignableFrom(saveState.Target))
            {
                NavigationService.Navigate(saveState.Target, saveState.SuspensionState);
            }
        }

        protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
        {
            // Application State must only be restored if the App was terminated during suspension.
            return args.PreviousExecutionState == ApplicationExecutionState.Terminated;
        }
    }
}
