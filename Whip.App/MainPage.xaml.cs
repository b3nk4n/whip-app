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

namespace Whip.App
{
    /// <summary>
    /// The main page.
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        /// <summary>
        /// Creates a MainPage instance.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            BuildLocalizedApplicationBar();
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
            ApplicationBar.MenuItems.Add(appBarMenuItem);
        }
    }
}