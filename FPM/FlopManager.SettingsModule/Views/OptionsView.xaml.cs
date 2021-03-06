﻿using FlopManager.SettingsModule.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlopManager.SettingsModule.Views
{
    /// <summary>
    /// Interaction logic for OptionsView.xaml
    /// </summary>
    [Export("OptionsView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class OptionsView : UserControl
    {
        public OptionsView()
        {
            InitializeComponent();
        }
        [Import]
        public OptionsViewModel ViewModel
        {
            get { return DataContext as OptionsViewModel; }
            set { DataContext = value; }
        }
    }
}
