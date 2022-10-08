using System;
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
using System.Windows.Shapes;

namespace HW8
{
    public partial class NewTab : Window
    {
        public NewTab()
        {
            InitializeComponent();
        }

        public string? FileName { get; set; }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FileNameTextBox.Text != null && !string.IsNullOrWhiteSpace(FileNameTextBox.Text))
                    FileName = FileNameTextBox.Text;
                else throw new NullReferenceException();

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
