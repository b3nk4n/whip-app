using PhoneKit.Framework.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whip.App
{
    /// <summary>
    /// The application settings.
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// Indicates whether the user has reviewed the app.
        /// </summary>
        public static StoredObject<bool> HasReviewed = new StoredObject<bool>("hasReviewed", false);

        /// <summary>
        /// Indicates whether the vibration is enabled.
        /// </summary>
        public static StoredObject<bool> VibrationEnabled = new StoredObject<bool>("vibrationEnabled", true);

        /// <summary>
        /// The shake sensitivity from low to high [0,...,6].
        /// </summary>
        public static StoredObject<int> Sensitivity = new StoredObject<int>("sensitivity", 3);

        /// <summary>
        /// The minimum sensitivity.
        /// </summary>
        public const int MIN_SENSITIVITY = 0;

        /// <summary>
        /// The maximum sensitivity.
        /// </summary>
        public const int MAX_SENSITIVITY = 6;
    }
}
