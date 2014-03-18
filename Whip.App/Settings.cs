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
    }
}
