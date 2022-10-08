using Microsoft.Win32;
using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HW8
{
    public partial class MainWindow : Window
    {
        private System.Windows.Threading.DispatcherTimer Timer = new System.Windows.Threading.DispatcherTimer();
        private SaveFileDialog saveFileDialog = new SaveFileDialog();
        private string SelectedTabTextBox { get; set; }

        public DateTime Time
        {
            get { return (DateTime)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }
        public static readonly DependencyProperty MyPropertyProperty =
           DependencyProperty.Register("Time", typeof(DateTime), typeof(MainWindow), new PropertyMetadata(DateTime.Now));


        public int Line
        {
            get { return (int)GetValue(LineProperty); }
            set { SetValue(LineProperty, value); }
        }
        public static readonly DependencyProperty LineProperty =
            DependencyProperty.Register("Line", typeof(int), typeof(MainWindow), new PropertyMetadata(1));

        public int Column
        {
            get { return (int)GetValue(ColumnProperty); }
            set { SetValue(ColumnProperty, value); }
        }
        public static readonly DependencyProperty ColumnProperty =
            DependencyProperty.Register("Column", typeof(int), typeof(MainWindow), new PropertyMetadata(1));


        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;
            saveFileDialog.Filter = "Текстовий Документ|*.txt";

            Time = DateTime.Now;
            Timer.Tick += Timer_Tick;
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            Timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            Time = DateTime.Now;
            TextBox_PreviewTextInput(sender, null);
        }
        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox textBox1 = TextBoxCheck();
            var caretIndex = textBox1.CaretIndex;
            Line = textBox1.GetLineIndexFromCharacterIndex(caretIndex);
            Column = caretIndex - textBox1.GetCharacterIndexFromLineIndex(Line);
            Line++; Column++;
        }


        private void NewTabButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NewTab newTab = new NewTab();
                if (newTab.ShowDialog() == true)
                {
                    var textBox = new TextBox();
                    textBox.TextWrapping = TextWrapping.Wrap;
                    textBox.AcceptsReturn = true;
                    textBox.AcceptsTab = true;

                    var tab = new TabItem();
                    tab.Content = textBox;
                    tab.Header = newTab.FileName;
                    tab.IsSelected = true;


                    mainTabControl.Items.Add(tab);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void OpenTextFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();

                openFileDialog.Filter = "Текстовий Документ|*.txt";

                if (openFileDialog.ShowDialog() == true)
                {
                    SelectedTabTextBox = File.ReadAllText(openFileDialog.FileName, Encoding.Default);

                    TabItem tabPage = new TabItem { Header = System.IO.Path.GetFileName(openFileDialog.FileName) };

                    //?
                    Title = System.IO.Path.GetFullPath(openFileDialog.FileName);

                    foreach (TabItem tp in mainTabControl.Items)
                    {
                        if (tp.Header == tabPage.Header)
                        {
                            MessageBox.Show("Вкладка з таким іменем вже відкрита");
                            return;
                        }
                    }

                    var textBox = new TextBox();
                    textBox.TextWrapping = TextWrapping.Wrap;
                    textBox.AcceptsReturn = true;
                    textBox.AcceptsTab = true;
                    textBox.Text = SelectedTabTextBox;
                    tabPage.Content = textBox;

                    mainTabControl.Items.Add(tabPage);
                    mainTabControl.SelectedItem = tabPage;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SaveAsFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBox tb = TextBoxCheck();
                if (saveFileDialog.ShowDialog() == true)
                {
                    SelectedTabTextBox = tb.Text;
                    string fname = saveFileDialog.FileName;
                    File.WriteAllText(fname, SelectedTabTextBox);

                    //?
                    Title = System.IO.Path.GetFullPath(fname);

                    MessageBox.Show("Файл було збережено");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBox tb = TextBoxCheck();

                if (MessageBoxResult.Yes == MessageBox.Show(
                     "Чи бажаєте ви зберегти файл перед закриттям?",
                     "Повідомлення", MessageBoxButton.YesNo))
                {
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        string fname = saveFileDialog.FileName;
                        File.WriteAllText(fname, SelectedTabTextBox);
                        MessageBox.Show("Файл було збережено");
                    }
                }
                mainTabControl.Items.Remove(mainTabControl.SelectedItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                while(mainTabControl.Items.Count > 0)
                    CloseTab_Click(sender, e);
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseApp_Click(e, null);
        }


        private TextBox TextBoxCheck()
        {
            if (mainTabControl.SelectedItem is TabItem tab && tab != null && tab.Content is TextBox box && box != null)
                return box;
            else return null;
        }
        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBoxCheck()?.Copy();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBoxCheck()?.Paste();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBoxCheck()?.Cut();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBoxCheck()?.SelectAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TextBoxCheck().CanUndo)
                    TextBoxCheck()?.Undo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var temp_textBox = (mainTabControl.SelectedItem as TabItem)?.Content as TextBox;
                if (temp_textBox != null && temp_textBox.FontSize <= 50)
                    temp_textBox.FontSize += 5;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ZoomOutClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var temp_textBox = (mainTabControl.SelectedItem as TabItem)?.Content as TextBox;
                if (temp_textBox != null && temp_textBox.FontSize >= 15)
                    temp_textBox.FontSize -= 5;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ZoomByDefault(object sender, RoutedEventArgs e)
        {
            try
            {
                ((mainTabControl.SelectedItem as TabItem)?.Content as TextBox).FontSize = 12;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void TimewNewRomanFont(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBoxCheck().FontFamily = new FontFamily("Times New Roman");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ComicSansMSFont(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBoxCheck().FontFamily = new FontFamily("Comic Sans MS");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void BadScriptFont(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBoxCheck().FontFamily = new FontFamily("Bad Script");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CascadiaMonoFont(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBoxCheck().FontFamily = new FontFamily("Cascadia Mono");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void DefaultFont(object sender, RoutedEventArgs e)
        {
            try
            {
                ((mainTabControl.SelectedItem as TabItem)?.Content as TextBox).FontFamily = new FontFamily("Default");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ForeroundChoose(object sender, RoutedEventArgs e)
        {
            try
            {
                MyColorDialog myColorDialog = new MyColorDialog();
                if (myColorDialog.ShowDialog() == true)
                {
                    TextBoxCheck().Foreground = new SolidColorBrush(Color.FromRgb(
                        Convert.ToByte(myColorDialog.Red),
                        Convert.ToByte(myColorDialog.Green),
                        Convert.ToByte(myColorDialog.Blue)));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ForeroundDefault(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBoxCheck().Foreground = Brushes.Black;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void BackgroundChoose(object sender, RoutedEventArgs e)
        {
            try
            {
                MyColorDialog myColorDialog = new MyColorDialog();
                if (myColorDialog.ShowDialog() == true)
                {
                    TextBoxCheck().Background = new SolidColorBrush(Color.FromRgb(
                        Convert.ToByte(myColorDialog.Red),
                        Convert.ToByte(myColorDialog.Green),
                        Convert.ToByte(myColorDialog.Blue)));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void BackgroundDefault(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBoxCheck().Background = Brushes.White;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}