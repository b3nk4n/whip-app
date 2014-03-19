using PhoneKit.Framework.Core.MVVM;
using PhoneKit.Framework.Core.OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whip.App.ViewModel
{
    /// <summary>
    /// The main view model.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// The whip for the dark theme.
        /// </summary>
        private const string WHIP_DARK = "/Assets/Images/whip.dark.png";

        /// <summary>
        /// The whip for the light theme.
        /// </summary>
        private const string WHIP_LIGHT = "/Assets/Images/whip.light.png";

        /// <summary>
        /// Gets the whip image path.
        /// </summary>
        public string WhipImagePath
        {
            get
            {
                if (PhoneThemeHelper.IsLightThemeActive)
                    return WHIP_DARK;
                else
                    return WHIP_LIGHT;
            }
        }
    }
}
