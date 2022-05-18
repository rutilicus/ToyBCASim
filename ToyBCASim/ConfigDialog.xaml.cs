using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ToyBCASim
{
    /// <summary>
    /// ConfigDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ConfigDialog : Window
    {
        private static readonly Regex _numberRegex = new("^[0-9]+");

        public ConfigDialog()
        {
            InitializeComponent();

            // 初回表示時にテキスト設定
            ContentRendered += (s, e) => { SetText(); };
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = true;
        }

        private void SetPlalette(int palette)
        {
            GlobalConfig.Instance.Palette = palette;

            Button?[] palettes = new Button?[3];
            palettes[0] = FindName("paletteButton0") as Button;
            palettes[1] = FindName("paletteButton1") as Button;
            palettes[2] = FindName("paletteButton2") as Button;

            for (int i = 0; i < palettes.Length; i++)
            {
                if (palettes[i] != null)
                {
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
                    palettes[i].BorderBrush = i == palette ? Brushes.Red : Brushes.Gray;
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
                }
            }
        }

        private void Button_Click_Palette0(object sender, RoutedEventArgs e)
        {
            SetPlalette(0);
        }
        private void Button_Click_Palette1(object sender, RoutedEventArgs e)
        {
            SetPlalette(1);
        }
        private void Button_Click_Palette2(object sender, RoutedEventArgs e)
        {
            SetPlalette(2);
        }

        private void NumberPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !_numberRegex.IsMatch(e.Text);
        }

        private void SetText()
        {
            TextBox? heightTextBox = FindName("heightTextBox") as TextBox;
            if (heightTextBox != null)
            {
                heightTextBox.Text = Stage.Instance.Height.ToString();
            }
            TextBox? widthTextBox = FindName("widthTextBox") as TextBox;
            if (widthTextBox != null)
            {
                widthTextBox.Text = Stage.Instance.Width.ToString();
            }
            TextBox? gridSizeTextBox = FindName("gridSizeTextBox") as TextBox;
            if (gridSizeTextBox != null)
            {
                gridSizeTextBox.Text = GlobalConfig.Instance.GridSize.ToString();
            }
            TextBox? delayTextBox = FindName("delayTextBox") as TextBox;
            if (delayTextBox != null)
            {
                delayTextBox.Text = GlobalConfig.Instance.Delay.ToString();
            }
        }

        private void Width_TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int width = string.IsNullOrEmpty(textBox.Text) ? 1 : Math.Max(int.Parse(textBox.Text), 1);
            textBox.Text = width.ToString();
            Stage.Instance.Resize(Stage.Instance.Height, width);

            Application.Current.MainWindow.InvalidateVisual();
        }

        private void Height_TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int height = string.IsNullOrEmpty(textBox.Text) ? 1 : Math.Max(int.Parse(textBox.Text), 1);
            textBox.Text = height.ToString();
            Stage.Instance.Resize(height, Stage.Instance.Width);

            Application.Current.MainWindow.InvalidateVisual();
        }

        private void GridSize_TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int gridSize = string.IsNullOrEmpty(textBox.Text) ? 3 : Math.Max(int.Parse(textBox.Text), 3);
            textBox.Text = gridSize.ToString();
            GlobalConfig.Instance.GridSize = gridSize;

            Application.Current.MainWindow.InvalidateVisual();
        }

        private void Delay_TextBox_LostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int delay = string.IsNullOrEmpty(textBox.Text) ? 100 : Math.Max(int.Parse(textBox.Text), 100);
            textBox.Text = delay.ToString();
            GlobalConfig.Instance.Delay = delay;
        }

        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            // とりあえず1ステップだけやる
            Stage.Instance.Step();

            Application.Current.MainWindow.InvalidateVisual();
        }
    }
}
