using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using Microsoft.Win32;

namespace INDZ_5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _leftFilePath = string.Empty;
        private string _rightFilePath = string.Empty;
        private bool _leftTextChanged = false;
        private bool _rightTextChanged = false;
        private static string _path = @"Software\INDZ_5";
        public MainWindow()
        {
            InitializeComponent();
            LeftTextBox.Focus();
            LoadWindowSettings();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                if (LeftTextBox.IsFocused)
                {
                    _leftFilePath = openFileDialog.FileName;
                    LeftTextBox.Text = File.ReadAllText(_leftFilePath);
                    LeftLabel.Content = "Left File: " + _leftFilePath;
                    _leftTextChanged = false;
                }
                else if (RightTextBox.IsFocused)
                {
                    _rightFilePath = openFileDialog.FileName;
                    RightTextBox.Text = File.ReadAllText(_rightFilePath);
                    RightLabel.Content = "Right File: " + _rightFilePath;
                    _rightTextChanged = false;
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (LeftTextBox.IsFocused && !string.IsNullOrEmpty(_leftFilePath))
            {
                File.WriteAllText(_leftFilePath, LeftTextBox.Text);
                LeftLabel.Content = "Left File: " + _leftFilePath;
                _leftTextChanged = false;
            }
            else if (RightTextBox.IsFocused && !string.IsNullOrEmpty(_rightFilePath))
            {
                File.WriteAllText(_rightFilePath, RightTextBox.Text);
                RightLabel.Content = "Right File: " + _rightFilePath;
                _rightTextChanged = false;
            }
        }

        private void Compare_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(LeftTextBox.Text) && !string.IsNullOrEmpty(RightTextBox.Text))
            {
                int minLength = Math.Min(LeftTextBox.Text.Length, RightTextBox.Text.Length);
                for (int i = 0; i < minLength; i++)
                {
                    if (LeftTextBox.Text[i] != RightTextBox.Text[i])
                    {
                        LeftTextBox.Focus();
                        LeftTextBox.Select(i, 1);
                        RightTextBox.Select(i, 1);
                        return;
                    }
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (_leftTextChanged || _rightTextChanged)
            {
                MessageBoxResult result = MessageBox.Show("Чи хочете зберегти зміни?", "Exit", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    Save_Click(sender, e);
                    Application.Current.Shutdown();
                }
                else if (result == MessageBoxResult.No)
                {
                    Application.Current.Shutdown();
                }
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender == LeftTextBox)
            {
                _leftTextChanged = true;
                LeftLabel.Content = "Left File: *" + _leftFilePath;
            }
            else if (sender == RightTextBox)
            {
                _rightTextChanged = true;
                RightLabel.Content = "Right File: *" + _rightFilePath;
            }
        }

        private void LoadWindowSettings()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(_path);

            if (key != null)
            {
                string leftPath = (string)key.GetValue("LeftFilePath");
                if (!string.IsNullOrEmpty(leftPath) && File.Exists(leftPath))
                {
                    _leftFilePath = leftPath;
                    LeftTextBox.Text = File.ReadAllText(_leftFilePath);
                    LeftLabel.Content = "Left File: " + _leftFilePath;
                }

                string rightPath = (string)key.GetValue("RightFilePath");
                if (!string.IsNullOrEmpty(rightPath) && File.Exists(rightPath))
                {
                    _rightFilePath = rightPath;
                    RightTextBox.Text = File.ReadAllText(_rightFilePath);
                    RightLabel.Content = "Right File: " + _rightFilePath;
                }

                int leftCursorPosition = (int)key.GetValue("LeftCursorPosition", 0);
                int rightCursorPosition = (int)key.GetValue("RightCursorPosition", 0);

                LeftTextBox.Select(leftCursorPosition, 0);
                RightTextBox.Select(rightCursorPosition, 0);

                int windowWidth = (int)key.GetValue("WindowWidth", (int)Width);
                int windowHeight = (int)key.GetValue("WindowHeight", (int)Height);

                Width = windowWidth;
                Height = windowHeight;
            }
        }

        private void SaveWindowSettings()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(_path);

            if (key != null)
            {
                key.SetValue("LeftFilePath", _leftFilePath);
                key.SetValue("RightFilePath", _rightFilePath);
                key.SetValue("LeftCursorPosition", LeftTextBox.CaretIndex);
                key.SetValue("RightCursorPosition", RightTextBox.CaretIndex);
                key.SetValue("WindowWidth", (int)Width);
                key.SetValue("WindowHeight", (int)Height);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            SaveWindowSettings();
        }
    }
}