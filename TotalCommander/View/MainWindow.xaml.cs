﻿using System;
using System.Collections.Generic;
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

namespace TotalCommander
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel.MainWindowViewModel(this);
        }
        public void KeyDown(object sender, KeyEventArgs e)
        {
            if ((this.DataContext as ViewModel.MainWindowViewModel).KeyDown != null)
                (this.DataContext as ViewModel.MainWindowViewModel).KeyDown(sender, e);
        }

        public void WindowSizeChange(object sender, SizeChangedEventArgs e)
        {
            if ((this.DataContext as ViewModel.MainWindowViewModel).WindowSizeChange != null)
                (this.DataContext as ViewModel.MainWindowViewModel).WindowSizeChange(sender, e);
        }
    }
}