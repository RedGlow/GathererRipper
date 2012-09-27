using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GathererRipper
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Configure save file dialog box
            chooseDatabaseDialog.FileName = "Gatherer"; // Default file name
            chooseDatabaseDialog.DefaultExt = ".db"; // Default file extension
            chooseDatabaseDialog.OverwritePrompt = false; // Can choose an existing file
            chooseDatabaseDialog.Filter = "Database files (.db)|*.db|All files|*.*"; // Filter files by extension 
        }

        private Microsoft.Win32.SaveFileDialog chooseDatabaseDialog = new Microsoft.Win32.SaveFileDialog();

        private void chooseDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            // Show save file dialog box
            var result = chooseDatabaseDialog.ShowDialog();

            // Process save file dialog box results 
            if (result == true)
            {
                // Save choice
                ((RipperViewModel)App.Current.Resources["Ripper"]).DatabasePath = chooseDatabaseDialog.FileName;
            }
        }

    }
}
