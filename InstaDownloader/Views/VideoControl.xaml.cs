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

namespace InstaDownloader.Views
{
    /// <summary>
    /// Interaction logic for VideoControl.xaml
    /// </summary>
    public partial class VideoControl : UserControl
    {
        public VideoControl()
        {
            InitializeComponent();
            Video.Pause();
        }

        private void Play_OnClick(object sender, RoutedEventArgs e)
        {
            Video.Play();
        }

        private void Pause_OnClick(object sender, RoutedEventArgs e)
        {
            Video.Pause();
        }

        private void Stop_OnClick(object sender, RoutedEventArgs e)
        {
            Video.Stop();
        }
    }
}
