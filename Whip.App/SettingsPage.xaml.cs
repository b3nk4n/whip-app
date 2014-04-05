using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Whip.App
{
    /// <summary>
    /// The settings page.
    /// </summary>
    public partial class SettingsPage : PhoneApplicationPage
    {
        /// <summary>
        /// Creates a SettingPage instance.
        /// </summary>
        public SettingsPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// When the page is navigated to.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ToggleVibration.IsChecked = Settings.VibrationEnabled.Value;
            SliderSensitivity.Value = Settings.Sensitivity.Value;
        }

        /// <summary>
        /// When the page is navigated from.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            Settings.VibrationEnabled.Value = ToggleVibration.IsChecked.Value;
            Settings.Sensitivity.Value = (int)SliderSensitivity.Value;
        }
    }
}