//#define TEST_SENSOR_PAGE_ACTIVE

using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Whip.App.Resources;
using System.Windows.Resources;
using PhoneKit.Framework.Audio;
using PhoneKit.Framework.OS.ShakeGestures;
using PhoneKit.Framework.OS;
using System.Threading;
using PhoneKit.Framework.Advertising;
using Microsoft.Phone.Tasks;
using PhoneKit.Framework.Support;
using Whip.App.Model;
using Windows.Devices.Sensors;
using System.Diagnostics;

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
        /// Indicates whether the tip is hidden.
        /// </summary>
        private bool _isTipHidden = false;

        /// <summary>
        /// The shake sensitivity values.
        /// </summary>
        private ShakeSensitivityData[] _shakeSensitivity;

        /// <summary>
        /// The accelerometer sensor.
        /// </summary>
        private Accelerometer _accelerometer;

        // the last readings
        private double _lastReadingX;
        private double _lastReadingY;
        private double _lastReadingZ;

        // the datetime until the shake detector is open for detection.
        private DateTime _isActiveForWhipUntil;

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

            BuildLocalizedApplicationBar();

            InitializeSoundEffect();
            InitializeShakeGesture();
            InitializeShakeSensitivities();
            LoadAdControl();

            StartupActionManager.Instance.Register(1, ActionExecutionRule.MoreThan, () =>
            {
                if (!_isTipHidden)
                    HideTip.Begin();
            });

            _accelerometer = Accelerometer.GetDefault();
            _accelerometer.ReadingChanged += (s, e) =>
                {
                    _lastReadingX =  e.Reading.AccelerationX;
                    _lastReadingY = e.Reading.AccelerationY;
                    _lastReadingZ = e.Reading.AccelerationZ;

                    if (Math.Abs(_lastReadingY) > 0.9)
                    {
                        _isActiveForWhipUntil = DateTime.Now + TimeSpan.FromMilliseconds(CurrentSensitivity.OpenIntervalForFastMovementYInMillis);
                    }
                };
        }

        /// <summary>
        /// Initializes the shake sensitivities.
        /// </summary>
        private void InitializeShakeSensitivities()
        {
            _shakeSensitivity = new ShakeSensitivityData[Settings.MAX_SENSITIVITY + 1];

            _shakeSensitivity[0] = new ShakeSensitivityData();
            _shakeSensitivity[0].ShakeMagnitudeWithoutGravitationThreshold = 0.35;
            _shakeSensitivity[0].StillCounterThreshold = 7;
            _shakeSensitivity[0].StillMagnitudeWithoutGravitationThreshold = 0.02;
            _shakeSensitivity[0].MaximumStillVectorsNeededForAverage = 20;
            _shakeSensitivity[0].MinimumStillVectorsNeededForAverage = 5;
            _shakeSensitivity[0].MinimumShakeVectorsNeededForShake = 5;
            _shakeSensitivity[0].WeakMagnitudeWithoutGravitationThreshold = 0.8;
            _shakeSensitivity[0].MinimumRequiredMovesForShake = 2;
            _shakeSensitivity[0].MinimumSoundDelayInMilliseconds = 800;
            _shakeSensitivity[0].OpenIntervalForFastMovementYInMillis = 225;

            _shakeSensitivity[1] = new ShakeSensitivityData();
            _shakeSensitivity[1].ShakeMagnitudeWithoutGravitationThreshold = 0.3;
            _shakeSensitivity[1].StillCounterThreshold = 8;
            _shakeSensitivity[1].StillMagnitudeWithoutGravitationThreshold = 0.02;
            _shakeSensitivity[1].MaximumStillVectorsNeededForAverage = 20;
            _shakeSensitivity[1].MinimumStillVectorsNeededForAverage = 5;
            _shakeSensitivity[1].MinimumShakeVectorsNeededForShake = 6;
            _shakeSensitivity[1].WeakMagnitudeWithoutGravitationThreshold = 0.7;
            _shakeSensitivity[1].MinimumRequiredMovesForShake = 2;
            _shakeSensitivity[1].MinimumSoundDelayInMilliseconds = 750;
            _shakeSensitivity[1].OpenIntervalForFastMovementYInMillis = 250;

            _shakeSensitivity[2] = new ShakeSensitivityData();
            _shakeSensitivity[2].ShakeMagnitudeWithoutGravitationThreshold = 0.25;
            _shakeSensitivity[2].StillCounterThreshold = 9;
            _shakeSensitivity[2].StillMagnitudeWithoutGravitationThreshold = 0.02;
            _shakeSensitivity[2].MaximumStillVectorsNeededForAverage = 20;
            _shakeSensitivity[2].MinimumStillVectorsNeededForAverage = 5;
            _shakeSensitivity[2].MinimumShakeVectorsNeededForShake = 4;
            _shakeSensitivity[2].WeakMagnitudeWithoutGravitationThreshold = 0.6;
            _shakeSensitivity[2].MinimumRequiredMovesForShake = 2;
            _shakeSensitivity[2].MinimumSoundDelayInMilliseconds = 700;
            _shakeSensitivity[2].OpenIntervalForFastMovementYInMillis = 275;

            _shakeSensitivity[3] = new ShakeSensitivityData();
            _shakeSensitivity[3].ShakeMagnitudeWithoutGravitationThreshold = 0.2;
            _shakeSensitivity[3].StillCounterThreshold = 10; // 15
            _shakeSensitivity[3].StillMagnitudeWithoutGravitationThreshold = 0.02;
            _shakeSensitivity[3].MaximumStillVectorsNeededForAverage = 20;
            _shakeSensitivity[3].MinimumStillVectorsNeededForAverage = 5;
            _shakeSensitivity[3].MinimumShakeVectorsNeededForShake = 4;
            _shakeSensitivity[3].WeakMagnitudeWithoutGravitationThreshold = 0.5; // 2.75
            _shakeSensitivity[3].MinimumRequiredMovesForShake = 2;
            _shakeSensitivity[3].MinimumSoundDelayInMilliseconds = 650;
            _shakeSensitivity[3].OpenIntervalForFastMovementYInMillis = 300;

            _shakeSensitivity[4] = new ShakeSensitivityData();
            _shakeSensitivity[4].ShakeMagnitudeWithoutGravitationThreshold = 0.15;
            _shakeSensitivity[4].StillCounterThreshold = 11;
            _shakeSensitivity[4].StillMagnitudeWithoutGravitationThreshold = 0.02;
            _shakeSensitivity[4].MaximumStillVectorsNeededForAverage = 20;
            _shakeSensitivity[4].MinimumStillVectorsNeededForAverage = 5;
            _shakeSensitivity[4].MinimumShakeVectorsNeededForShake = 4;
            _shakeSensitivity[4].WeakMagnitudeWithoutGravitationThreshold = 0.4;
            _shakeSensitivity[4].MinimumRequiredMovesForShake = 2;
            _shakeSensitivity[4].MinimumSoundDelayInMilliseconds = 600;
            _shakeSensitivity[4].OpenIntervalForFastMovementYInMillis = 325;

            _shakeSensitivity[5] = new ShakeSensitivityData();
            _shakeSensitivity[5].ShakeMagnitudeWithoutGravitationThreshold = 0.1;
            _shakeSensitivity[5].StillCounterThreshold = 12;
            _shakeSensitivity[5].StillMagnitudeWithoutGravitationThreshold = 0.02;
            _shakeSensitivity[5].MaximumStillVectorsNeededForAverage = 20;
            _shakeSensitivity[5].MinimumStillVectorsNeededForAverage = 5;
            _shakeSensitivity[5].MinimumShakeVectorsNeededForShake = 3;
            _shakeSensitivity[5].WeakMagnitudeWithoutGravitationThreshold = 0.3;
            _shakeSensitivity[5].MinimumRequiredMovesForShake = 2;
            _shakeSensitivity[5].MinimumSoundDelayInMilliseconds = 550;
            _shakeSensitivity[4].OpenIntervalForFastMovementYInMillis = 350;

            _shakeSensitivity[6] = new ShakeSensitivityData();
            _shakeSensitivity[6].ShakeMagnitudeWithoutGravitationThreshold = 0.1;
            _shakeSensitivity[6].StillCounterThreshold = 13;
            _shakeSensitivity[6].StillMagnitudeWithoutGravitationThreshold = 0.02;
            _shakeSensitivity[6].MaximumStillVectorsNeededForAverage = 20;
            _shakeSensitivity[6].MinimumStillVectorsNeededForAverage = 5;
            _shakeSensitivity[6].MinimumShakeVectorsNeededForShake = 3;
            _shakeSensitivity[6].WeakMagnitudeWithoutGravitationThreshold = 0.2;
            _shakeSensitivity[6].MinimumRequiredMovesForShake = 2;
            _shakeSensitivity[6].MinimumSoundDelayInMilliseconds = 500;
            _shakeSensitivity[6].OpenIntervalForFastMovementYInMillis = 375;
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
            {
                AdvertsConteriner.Visibility = System.Windows.Visibility.Visible;
                OfflineAdControl.Start();
            }
            else
            {
                OfflineAdControl.Stop();
                AdvertsConteriner.Visibility = System.Windows.Visibility.Collapsed;
            }

            StartupActionManager.Instance.Fire(e);
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

            // Stop adverts switching
            OfflineAdControl.Stop();
        }

        /// <summary>
        /// Builds the localized application bar
        /// </summary>
        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Minimized;

            // about
            ApplicationBarMenuItem appBarMenuItem1 = new ApplicationBarMenuItem(AppResources.SettingsTitle.ToLower());
            appBarMenuItem1.Click += (s, e) =>
            {
                NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
            };
            ApplicationBar.MenuItems.Add(appBarMenuItem1);

            // about
            ApplicationBarMenuItem appBarMenuItem2 = new ApplicationBarMenuItem(AppResources.AboutTitle.ToLower());
            appBarMenuItem2.Click += (s, e) =>
                {
                    NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
                };
            ApplicationBar.MenuItems.Add(appBarMenuItem2);

#if TEST_SENSOR_PAGE_ACTIVE
            // test sensor for debug/testing
            ApplicationBarMenuItem appBarMenuItem3 = new ApplicationBarMenuItem("sensor test");
            appBarMenuItem3.Click += (s, e) =>
            {
                NavigationService.Navigate(new Uri("/SensorTestPage.xaml", UriKind.Relative));
            };
            ApplicationBar.MenuItems.Add(appBarMenuItem3);
#endif
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
            var sound = SoundEffects.Instance["whip"].CreateInstance();
            sound.Play();

            Thread.Sleep(TimeSpan.FromMilliseconds(150));

            if (Settings.VibrationEnabled.Value)
                VibrationHelper.Vibrate(0.1f);
        }

        /// <summary>
        /// Initializes the shake gesture.
        /// </summary>
        private void InitializeShakeGesture()
        {
            ShakeGesturesHelper.Instance.ShakeGesture += (s, e) =>
            {
                // filter by time
                if (DateTime.Now - _lastShakeEventTime < TimeSpan.FromMilliseconds(CurrentSensitivity.MinimumSoundDelayInMilliseconds))
                {
                    Debug.WriteLine("Filer Time");
                    return;
                }

                if (DateTime.Now > _isActiveForWhipUntil)
                {
                    Debug.WriteLine("Filer by closed");
                    return;
                }

                Dispatcher.BeginInvoke(() =>
                {
                    if (!_isTipHidden)
                        HideTip.Begin();

                    PlaySoundEffect();

                    _lastShakeEventTime = DateTime.Now;
                });
            };
        }

        /// <summary>
        /// Sets up the shage detectors sensitivity from settings.
        /// </summary>
        private void SetupSensitivity()
        {
            var sensitivity = CurrentSensitivity;

            // setup shake detector
            ShakeGesturesHelper.Instance.MinimumRequiredMovesForShake = sensitivity.MinimumRequiredMovesForShake;
            ShakeGesturesHelper.Instance.WeakMagnitudeWithoutGravitationThreshold = sensitivity.WeakMagnitudeWithoutGravitationThreshold;
            ShakeGesturesHelper.Instance.MinimumShakeVectorsNeededForShake = sensitivity.MinimumShakeVectorsNeededForShake;
            ShakeGesturesHelper.Instance.StillCounterThreshold = sensitivity.StillCounterThreshold;
            ShakeGesturesHelper.Instance.ShakeMagnitudeWithoutGravitationThreshold = 0.2;
        }

        /// <summary>
        /// Gets the current sensitivity data.
        /// </summary>
        private ShakeSensitivityData CurrentSensitivity
        {
            get
            {
                int sensitivity = Settings.Sensitivity.Value;

                // check bounds (just to make sure)
                sensitivity = Math.Min(Settings.MAX_SENSITIVITY, sensitivity);
                sensitivity = Math.Max(Settings.MIN_SENSITIVITY, sensitivity);

                return _shakeSensitivity[sensitivity];
            }
        }

        /// <summary>
        /// Loads the adverts control.
        /// </summary>
        private void LoadAdControl()
        {
            OfflineAdControl.AddAdvert(new AdvertData(new Uri("/Assets/Adverts/Photo-Info_adduplex.png", UriKind.Relative), AdvertData.ActionTypes.AppId, "ac39aa30-c9b1-4dc6-af2d-1cc17d9807cc"));
            OfflineAdControl.AddAdvert(new AdvertData(new Uri("/Assets/Adverts/pocketBRAIN_adduplex.png", UriKind.Relative), AdvertData.ActionTypes.AppId, "ad1227e4-9f80-4967-957f-6db140dc0c90"));
            OfflineAdControl.AddAdvert(new AdvertData(new Uri("/Assets/Adverts/powernAPP_adduplex.png", UriKind.Relative), AdvertData.ActionTypes.AppId, "92740dff-b2e1-4813-b08b-c6429df03356"));
            OfflineAdControl.AddAdvert(new AdvertData(new Uri("/Assets/Adverts/frequenzer_adduplex.png", UriKind.Relative), AdvertData.ActionTypes.AppId, "92bac4f7-05eb-47ec-a75b-11f077f0c8f6"));
            OfflineAdControl.AddAdvert(new AdvertData(new Uri("/Assets/Adverts/ScribbleHunter_adduplex.png", UriKind.Relative), AdvertData.ActionTypes.AppId, "ed250596-e670-4d22-aee1-8ed0a08c411f"));
        }
    }
}