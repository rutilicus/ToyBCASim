using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ToyBCASim
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int VERSION_CODE = 0x00CAFE51;
        private Window? configWindow;

        public MainWindow()
        {
            InitializeComponent();

            // 初回表示時にStage描画
            ContentRendered += (s, e) => { DrawStage(); };

            // 初回表示時にコンフィグウインドウ表示
            ContentRendered += (s, e) => { ShowConfigWindow(); };

        }

        private void SaveFile(string filePath)
        {
            using (var stream = File.Open(filePath, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, false))
                {
                    writer.Write(VERSION_CODE);

                    int height = Stage.Instance.Height;
                    int width = Stage.Instance.Width;
                    int[,] data = Stage.Instance.Data;

                    writer.Write(Stage.Instance.Height);
                    writer.Write(Stage.Instance.Width);
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            writer.Write(data[y, x]);
                        }
                    }
                }
            }
        }

        private void ReadFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (var stream = File.Open(filePath, FileMode.Open))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        int version = reader.ReadInt32();
                        if (version == VERSION_CODE)
                        {
                            int height = reader.ReadInt32();
                            int width = reader.ReadInt32();

                            int[,] data = new int[height, width];

                            for (int y = 0; y < height; y++)
                            {
                                for (int x = 0; x < width; x++)
                                {
                                    data[y, x] = reader.ReadInt32();
                                }
                            }

                            Stage.Instance.Resize(height, width);
                            for (int y = 0; y < height; y++)
                            {
                                for (int x = 0; x < width; x++)
                                {
                                    Stage.Instance.SetData(y, x, data[y, x]);
                                }
                            }

                            InvalidateVisual();

                            if (configWindow?.FindName("heightTextBox") is TextBox heightTextBox)
                            {
                                heightTextBox.Text = height.ToString();
                            }
                            if (configWindow?.FindName("widthTextBox") is TextBox widthTextBox)
                            {
                                widthTextBox.Text = width.ToString();
                            }
                        }
                    }
                }
            }
        }

        private void MenuItem_Click_Save(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new()
            {
                Filter = "BCA(*.bca)|*.bca",
                RestoreDirectory = true,
            };

            if (dialog.ShowDialog() == true)
            {
                SaveFile(dialog.FileName);
            }
        }

        private void MenuItem_Click_Open(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                Filter = "BCA(*.bca)|*.bca",
            };

            if (dialog.ShowDialog() == true)
            {
                ReadFile(dialog.FileName);
            }
        }

        private void DrawStage()
        {
            Canvas? canvas = FindName("canvas") as Canvas;
            if (canvas != null)
            {
                int gridSize = GlobalConfig.Instance.GridSize;

                canvas.Children.Clear();

                Stage stage = Stage.Instance;

                int height = stage.Height;
                int width = stage.Width;
                int[,] data = stage.Data;

                canvas.Width = width * gridSize;
                canvas.Height = height * gridSize;

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        Rectangle rect = new()
                        {
                            Stroke = Brushes.Gray
                        };
                        switch (data[i, j])
                        {
                            case 0:
                                rect.Fill = Brushes.White;
                                break;

                            case 1:
                                rect.Fill = Brushes.Gray;
                                break;

                            case 2:
                                rect.Fill = Brushes.Black;
                                break;
                        }
                        rect.Width = gridSize;
                        rect.Height = gridSize;
                        Canvas.SetTop(rect, gridSize * i);
                        Canvas.SetLeft(rect, gridSize * j);
                        canvas?.Children.Add(rect);
                    }
                }
            }
        }

        private void ShowConfigWindow()
        {
            configWindow = new ConfigDialog
            {
                Owner = this
            };
            configWindow.Show();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            DrawStage();
        }

        private void SetGridData(Point mousePosition)
        {
            int gridSize = GlobalConfig.Instance.GridSize;

            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                Stage.Instance.SetData(
                    (int)(mousePosition.Y / gridSize),
                    (int)(mousePosition.X / gridSize),
                    GlobalConfig.Instance.Palette);

                InvalidateVisual();
            }
        }

        private void Canvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition((IInputElement)sender);
            SetGridData(mousePosition);
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition((IInputElement)sender);
            SetGridData(mousePosition);
        }
    }
}
