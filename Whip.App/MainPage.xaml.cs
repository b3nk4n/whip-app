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
using PhoneKit.Framework.Advertising;
using PhoneKit.Framework.Core.Net;
using Microsoft.Phone.Tasks;
using PhoneKit.Framework.Support;

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
        /// The advertisment control.
        /// </summary>
        private MsDuplexAdControl adControl;

        /// <summary>
        /// Indicates whether the tip is hidden.
        /// </summary>
        private bool _isTipHidden = false;

        /// <summary>
        /// Creates a MainPage instance.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            Loaded += (s, e) =>
                {
                    // pre-play the sound (muted) to have it in memory
                    SoundEffects.Instance["whip"].Play(0f, 0.0f, 0.0f);
                };

            RemoveAdButton.Tap += (s, e) =>
                {
                    if (MessageBox.Show(AppResources.MessageBoxRemoveAdContent, AppResources.MessageBoxRemoveAdTitle, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        Settings.HasReviewed.Value = true;

                        var reviewTask = new MarketplaceReviewTask();
                        reviewTask.Show();
                    }
                };

            StaticBanner.Tap += (s, e) =>
            {
                var marketplacetask = new MarketplaceDetailTask();
                marketplacetask.ContentType = MarketplaceContentType.Applications;
                marketplacetask.ContentIdentifier = "ad1227e4-9f80-4967-957f-6db140dc0c90";
                marketplacetask.Show();
            };

            BuildLocalizedApplicationBar();

            InitializeSoundEffect();
            InitializeShakeGesture();

            StartupActionManager.Instance.Register(1, ActionExecutionRule.MoreThan, () =>
            {
                if (!_isTipHidden)
                    HideTip.Begin();
            });
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

            if (!Settings.HasReviewed.Value)
                LoadAdControl();
            else
            {
                // remove add control and button
                AdvertsConteriner.Visibility = System.Windows.Visibility.Collapsed;
                
            }

            StartupActionManager.Instance.Fire();
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
                    NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
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

            Thread.Sleep(TimeSpan.FromMilliseconds(150));

            VibrationHelper.Vibrate(0.1f);
        }

        /// <summary>
        /// Initializes the shake gesture.
        /// </summary>
        private void InitializeShakeGesture()
        {
            ShakeGesturesHelper.Instance.ShakeGesture += (s, e) =>
            {
                if (DateTime.Now - _lastShakeEventTime < TimeSpan.FromMilliseconds(500))
                    return;

                Dispatcher.BeginInvoke(() =>
                {
                    if (!_isTipHidden)
                        HideTip.Begin();

                    PlaySoundEffect();

                    _lastShakeEventTime = DateTime.Now;
                });

            };
            ShakeGesturesHelper.Instance.MinimumRequiredMovesForShake = 2;
            ShakeGesturesHelper.Instance.WeakMagnitudeWithoutGravitationThreshold = 2.75f;
            ShakeGesturesHelper.Instance.MinimumShakeVectorsNeededForShake = 4;
            ShakeGesturesHelper.Instance.StillCounterThreshold = 15;
        }

        /// <summary>
        /// Loads the adverts control.
        /// </summary>
        private void LoadAdControl()
        {
            if (!ConnectivityHelper.HasNetwork)
                return;

            ShowCommercialBanner.Begin();

            adControl = new MsDuplexAdControl();
            adControl.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            adControl.MsApplicationId = "da6d9cd6-5ae8-41bd-bb99-f693afa63372";
            adControl.MsAdUnitId = "166592";
            adControl.AdDuplexAppId = "92959";
            adControl.IsTest = true;

            DynamicAdControlContainer.Child = adControl;
        }
    }
}