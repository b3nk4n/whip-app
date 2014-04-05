using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneKit.Framework.OS.ShakeGestures;
using PhoneKit.Framework.Audio;
using System.Threading;
using PhoneKit.Framework.OS;
using Whip.App.Model;
using Windows.Devices.Sensors;

namespace Whip.App
{
    public partial class SensorTestPage : PhoneApplicationPage
    {
        /// <summary>
        /// The last shake event time.
        /// </summary>
        private DateTime _lastShakeEventTime = DateTime.MinValue;

        /// <summary>
        /// The shake sensitivity values.
        /// </summary>
        private ShakeSensitivityData _shakeSensitivity = new ShakeSensitivityData();

        private Accelerometer _accelerometer;

        public SensorTestPage()
        {
            InitializeComponent();

            _accelerometer = Accelerometer.GetDefault();
            _accelerometer.Shaken += _acelerometer_Shaken;
            _accelerometer.ReadingChanged +=_accelerometer_ReadingChanged;

            UpdateUI();
            UpdateShakeDetector();
        }

        private double minX;
        private double minY;
        private double minZ;
        private double maxX;
        private double maxY;
        private double maxZ;

        private void _accelerometer_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            minX = Math.Min(minX, args.Reading.AccelerationX);
            minY = Math.Min(minY, args.Reading.AccelerationY);
            minZ = Math.Min(minZ, args.Reading.AccelerationZ);
            maxX = Math.Max(maxX, args.Reading.AccelerationX);
            maxY = Math.Max(maxY, args.Reading.AccelerationY);
            maxZ = Math.Max(maxZ, args.Reading.AccelerationZ);

            Dispatcher.BeginInvoke(() =>
                {
                    AccX.Text = string.Format("{0:0.000}", args.Reading.AccelerationX);
                    AccY.Text = string.Format("{0:0.000}", args.Reading.AccelerationY);
                    AccZ.Text = string.Format("{0:0.000}", args.Reading.AccelerationZ);
                    MinMaxX.Text = string.Format("{0:0.000} / {1:0.000}", minX, maxX);
                    MinMaxY.Text = string.Format("{0:0.000} / {1:0.000}", minY, maxY);
                    MinMaxZ.Text = string.Format("{0:0.000} / {1:0.000}", minZ, maxZ);
                });
        }

        private int _shakeCounter = 0;

        void _acelerometer_Shaken(Accelerometer sender, AccelerometerShakenEventArgs args)
        {
            Dispatcher.BeginInvoke(() =>
            {
                _shakeCounter++;
                ShakeCounter.Text = _shakeCounter.ToString();
            });
        }

        private void UpdateUI()
        {
            ShakeMagnitudeWithoutGravitationThresholdSlider.Value = _shakeSensitivity.ShakeMagnitudeWithoutGravitationThreshold;
            StillCounterThresholdSlider.Value = _shakeSensitivity.StillCounterThreshold;
            StillMagnitudeWithoutGravitationThresholdSlider.Value = _shakeSensitivity.StillMagnitudeWithoutGravitationThreshold;
            MaximumStillVectorsNeededForAverageSlider.Value = _shakeSensitivity.MaximumStillVectorsNeededForAverage;
            MinimumStillVectorsNeededForAverageSlider.Value = _shakeSensitivity.MinimumStillVectorsNeededForAverage;
            MinimumShakeVectorsNeededForShakeSlider.Value = _shakeSensitivity.MinimumShakeVectorsNeededForShake;
            WeakMagnitudeWithoutGravitationThresholdSlider.Value = _shakeSensitivity.WeakMagnitudeWithoutGravitationThreshold;
            MinimumRequiredMovesForShakeSlider.Value = _shakeSensitivity.MinimumRequiredMovesForShake;
            MinimumSoundDelayInMillisecondsSlider.Value = _shakeSensitivity.MinimumSoundDelayInMilliseconds;
        }

        private void UpdateShakeDetector()
        {
            _shakeSensitivity.ShakeMagnitudeWithoutGravitationThreshold = ShakeMagnitudeWithoutGravitationThresholdSlider.Value;
            _shakeSensitivity.StillCounterThreshold = (int)StillCounterThresholdSlider.Value;
            _shakeSensitivity.StillMagnitudeWithoutGravitationThreshold = StillMagnitudeWithoutGravitationThresholdSlider.Value;
            _shakeSensitivity.MaximumStillVectorsNeededForAverage = (int)MaximumStillVectorsNeededForAverageSlider.Value;
            _shakeSensitivity.MinimumStillVectorsNeededForAverage = (int)MinimumStillVectorsNeededForAverageSlider.Value;
            _shakeSensitivity.MinimumShakeVectorsNeededForShake = (int)MinimumShakeVectorsNeededForShakeSlider.Value;
            _shakeSensitivity.WeakMagnitudeWithoutGravitationThreshold = WeakMagnitudeWithoutGravitationThresholdSlider.Value;
            _shakeSensitivity.MinimumRequiredMovesForShake = (int)MinimumRequiredMovesForShakeSlider.Value;
            _shakeSensitivity.MinimumSoundDelayInMilliseconds = (int)MinimumSoundDelayInMillisecondsSlider.Value;

            ShakeGesturesHelper.Instance.ShakeMagnitudeWithoutGravitationThreshold = _shakeSensitivity.ShakeMagnitudeWithoutGravitationThreshold;
            ShakeGesturesHelper.Instance.StillCounterThreshold = _shakeSensitivity.StillCounterThreshold;
            ShakeGesturesHelper.Instance.StillMagnitudeWithoutGravitationThreshold = _shakeSensitivity.StillMagnitudeWithoutGravitationThreshold;
            ShakeGesturesHelper.Instance.MaximumStillVectorsNeededForAverage = _shakeSensitivity.MaximumStillVectorsNeededForAverage;
            ShakeGesturesHelper.Instance.MinimumStillVectorsNeededForAverage = _shakeSensitivity.MinimumStillVectorsNeededForAverage;
            ShakeGesturesHelper.Instance.MinimumShakeVectorsNeededForShake = _shakeSensitivity.MinimumShakeVectorsNeededForShake;
            ShakeGesturesHelper.Instance.WeakMagnitudeWithoutGravitationThreshold = _shakeSensitivity.WeakMagnitudeWithoutGravitationThreshold;
            ShakeGesturesHelper.Instance.MinimumRequiredMovesForShake = _shakeSensitivity.MinimumRequiredMovesForShake;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // activate shake listener
            ShakeGesturesHelper.Instance.Active = true;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // activate shake listener
            ShakeGesturesHelper.Instance.Active = false;
        }

        /// <summary>
        /// Plays the whip sound effect.
        /// </summary>
        private void PlaySoundEffect()
        {
            if (DateTime.Now - _lastShakeEventTime < TimeSpan.FromMilliseconds(_shakeSensitivity.MinimumSoundDelayInMilliseconds))
                return;

            var sound = SoundEffects.Instance["whip"].CreateInstance();
            sound.Play();

            Thread.Sleep(TimeSpan.FromMilliseconds(150));

            if (Settings.VibrationEnabled.Value)
                VibrationHelper.Vibrate(0.1f);
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateShakeDetector();
            UpdateUI();
        }

        private void ResetMinMax_Click(object sender, RoutedEventArgs e)
        {
            minX = maxX = minY = maxY = minZ = maxZ = 0;
        }
    }
}