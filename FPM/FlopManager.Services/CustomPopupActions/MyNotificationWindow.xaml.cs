// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.


using System.Windows;
using Prism.Interactivity.InteractionRequest;

namespace FlopManager.Services.CustomPopupActions
{
    /// <summary>
    /// Interaction logic for NotificationChildWindow.xaml
    /// </summary>
    public partial class MyNotificationWindow : Window
    {
        /// <summary>
        /// Creates a new instance of <see cref="MyNotificationWindow"/>
        /// </summary>
        public MyNotificationWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets or gets the <see cref="INotification"/> shown by this window./>
        /// </summary>
        public INotification Notification 
        {
            get
            {
                return this.DataContext as INotification;
            }
            set
            {
                this.DataContext = value;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
