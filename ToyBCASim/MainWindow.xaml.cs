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
        public MainWindow()
        {
            InitializeComponent();

            // 初回表示時にStage描画
            ContentRendered += (s, e) => { DrawStage(); };

            // 初回表示時にコンフィグウインドウ表示
            ContentRendered += (s, e) => { ShowConfigWindow(); };

        }

        private void MenuItem_Click_Save(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click_Open(object sender, RoutedEventArgs e)
        {

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
                        Rectangle rect = new();
                        rect.Stroke = Brushes.Gray;
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
            Window configWindow = new ConfigDialog();
            configWindow.Owner = this;
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

                this.InvalidateVisual();
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
