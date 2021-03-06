﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
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
using FlopManagerLoanModule.ViewModels;
using System.ComponentModel.Composition;

namespace FlopManagerLoanModule.Views
{
    /// <summary>
    /// Interaction logic for LoanTypesView.xaml
    /// </summary>
    //[Export("LoanTypesView")]
    //[PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class LoanTypes : UserControl
    {
        public LoanTypes()
        {
            InitializeComponent();
        }
        [Import]
        public LoanTypesViewModel ViewModel
        {
            get { return DataContext as LoanTypesViewModel;}
            set { DataContext = value; }
        }
    }
}
