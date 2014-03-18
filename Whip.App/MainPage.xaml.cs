using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Whip.App.Resources;
using Microsoft.Xna.Framework.Audio;
using System.Windows.Resources;
using PhoneKit.Framework.Audio;
using PhoneKit.Framework.OS.ShakeGestures;
using PhoneKit.Framework.OS;
using System.Threading;

namespace Whip.App
{
    /// <summary>
    /// The main page.
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        /// <summary>
        /// The last shake event time.
        /// </summary>
        private DateTime _lastShakeEventTime = DateTime.MinValue;

        /// <summary>
        /// Creates a MainPage instance.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            BuildLocalizedApplicationBar();

            InitializeSoundEffect();
            InitializeShakeGesture();
        }

        /// <summary>
        /// When the page is navigated to.
        /// </summary>
        /// <param name="e">>The event args.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // disable lock screen
            PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;

            // activate shake listener
            ShakeGesturesHelper.Instance.Active = true;
        }

        /// <summary>
        /// When the page is navigated from.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // enable lock screen
            PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Enabled;

            // deactivate shake listener
            ShakeGesturesHelper.Instance.Active = false;
        }

        /// <summary>
        /// Builds the localized application bar
        /// </summary>
        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Minimized;

            // about
            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AboutTitle.ToLower());
            appBarMenuItem.Click += (s, e) =>
                {
                    //NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
                };
            ApplicationBar.MenuItems.Add(appBarMenuItem);
        }

        /// <summary>
        /// Initializes the sound effect.
        /// </summary>
        private void InitializeSoundEffect()
        {
            StreamResourceInfo alarmResource = App.GetResourceStream(new Uri("Assets/Sounds/whip.wav", UriKind.Relative));
            SoundEffects.Instance.Load("whip", alarmResource.Stream);
        }

        /// <summary>
        /// Plays the whip sound effect.
        /// </summary>
        private void PlaySoundEffect()
        {
            if (DateTime.Now - _lastShakeEventTime < TimeSpan.FromMilliseconds(500))
                return;

            var sound = SoundEffects.Instance["whip"].CreateInstance();
            sound.Play();

            Thread.Sleep(TimeSpan.FromMilliseconds(333));

            VibrationHelper.Vibrate(0.1f);
        }

        /// <summary>
        /// Initializes the shake gesture.
        /// </summary>
        private void InitializeShakeGesture()
        {
            ShakeGesturesHelper.Instance.ShakeGesture += (s, e) =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    PlaySoundEffect();

                    _lastShakeEventTime = DateTime.Now;
                });

            };
            ShakeGesturesHelper.Instance.MinimumRequiredMovesForShake = 2;
            ShakeGesturesHelper.Instance.WeakMagnitudeWithoutGravitationThreshold = 3.0f;
            ShakeGesturesHelper.Instance.MinimumShakeVectorsNeededForShake = 4;
        }
    }
}