using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using InstaDownloader.ViewModels;

namespace InstaDownloader.Views
{
    /// <summary>
    /// Interaction logic for FinalPhrases.xaml
    /// </summary>
    public partial class FinalPhrases : UserControl
    {
        public FinalPhrases()
        {
            InitializeComponent();
        }

        private void AddPhrases_OnClick(object sender, RoutedEventArgs e)
        {
            var text = new TextBlock {Text = PhrasesTextBox.Text, TextWrapping = TextWrapping.Wrap};
            var radioButton = new RadioButton {GroupName = "Phrases", Content = text};
            radioButton.Click += FinalPhrase_OnClick;
            RadioButtonPanel.Children.Add(radioButton);
        }

        private void FinalPhrase_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton && DataContext is InstaDownloaderViewModel dataContext)
            {
                dataContext.FinalPhrase = radioButton.Content.ToString();
            }
        }
    }
}
