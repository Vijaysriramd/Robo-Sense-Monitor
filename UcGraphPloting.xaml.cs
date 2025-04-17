using Robo_Sense_Monitor.Model;
using Robo_Sense_Monitor.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Robo_Sense_Monitor
{
    /// <summary>
    /// Interaction logic for GraphPloting.xaml
    /// </summary>
    public partial class UcGraphPloting : UserControl
    {
        public class GraphVisualHost : FrameworkElement
        {
            private readonly VisualCollection _children;

            public GraphVisualHost()
            {
                _children = new VisualCollection(this);
            }

            public void AddVisual(DrawingVisual visual)
            {
                _children.Add(visual);
            }

            public void ClearVisuals()
            {
                _children.Clear();
            }

            protected override int VisualChildrenCount => _children.Count;

            protected override Visual GetVisualChild(int index) => _children[index];
        }
        //private readonly Logger _logger = new Logger(LogTarget.File, LogTarget.Console);
        private SensorDataViewModel _viewModel;
        //private IDeviceCommunicator _communicator;
        private GraphVisualHost _graphHost;
        private IDeviceCommunicator _communicator;
        private DeviceViewModel _deviceViewModel = new DeviceViewModel();
        public DeviceInfo DeviceDetails { get; set; }
        private ScaleTransform _graphScaleTransform = new ScaleTransform(1.0, 1.0);
        public bool Isplayback { get; set; }

        private int duration = 0;
        private double _previousHorizontalOffset = 0;

        private TimeSpan windowDuration = TimeSpan.FromMinutes(1);
        private DateTime windowStartTime;
        private DateTime windowstageTime;
        private DateTime windowEndTime;


        private ScaleTransform scaleTransform = new ScaleTransform();
        private Point origin;
        private Point start;

        public bool isDrawgraph { get; set; }
        public UcGraphPloting(ObservableCollection<DataPoint> data, DeviceInfo dev, bool isPlayback, DataCollectorService _dataCollectorService)
        {

            InitializeComponent();
            windowstageTime = DateTime.Now;
            windowEndTime = DateTime.Now + windowDuration;
            windowStartTime = windowEndTime - windowDuration;


            _graphScaleTransform.ScaleX = Math.Max(0.1, Math.Min(_graphScaleTransform.ScaleX, 10));
            _graphScaleTransform.ScaleY = Math.Max(0.1, Math.Min(_graphScaleTransform.ScaleY, 10));

            _viewModel = new SensorDataViewModel(_dataCollectorService, dev);

            GraphCanvas.RenderTransform = _graphScaleTransform;
            _viewModel.IsPlaybackMode = isPlayback;
            // _viewModel.Isplayback = isPlayback;
            DataContext = _viewModel;
            DispatcherTimer renderTimer = new DispatcherTimer();
            renderTimer.Interval = TimeSpan.FromSeconds(1); // Check every second for updates
            renderTimer.Tick += (s, e) =>
            {
                if (_viewModel.HasNewData()) // Custom method to check for new data
                {
                    DrawGraph();
                    DrawHistogram();
                }
            };
            renderTimer.Start();
        }
        private void DrawGraph()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GraphCanvas.Children.Clear();

                var dataToDraw = _viewModel.VisibleSensorData; // Draw the visible data

                if (dataToDraw.Count < 2) return;




                double width = GraphCanvas.ActualWidth;
                double height = GraphCanvas.ActualHeight;

                // var dataToDraw = _viewModel.VisibleSensorData;
                var minTime = dataToDraw.First().Timestamp;
                var maxTime = dataToDraw.Last().Timestamp;
                double timeRange = (maxTime - minTime).TotalSeconds;

                double minVal = dataToDraw.Min(d => d.Value);
                double maxVal = dataToDraw.Max(d => d.Value);
                double valRange = maxVal - minVal == 0 ? 1 : maxVal - minVal;


                List<SensorData> above = new();
                List<SensorData> within = new();
                List<SensorData> below = new();

                foreach (var point in dataToDraw)
                {
                    if (point.Value > _viewModel.Threshold.UpperLimit)
                        above.Add(point);
                    else if (point.Value < _viewModel.Threshold.LowerLimit)
                        below.Add(point);
                    else
                        within.Add(point);
                }

                DropShadowEffect shadowEffect = new DropShadowEffect
                {
                    Color = Colors.LightGray,
                    BlurRadius = 5,
                    ShadowDepth = 2,
                    Direction = 315,
                    Opacity = 0.3
                };


                void DrawWave(List<SensorData> group, Brush color)
                {
                    if (group.Count < 2) return;
                    for (int i = 1; i < group.Count; i++)
                    {
                        var prev = group[i - 1];
                        var curr = group[i];

                        double x1 = (prev.Timestamp - minTime).TotalSeconds / timeRange * width;
                        double x2 = (curr.Timestamp - minTime).TotalSeconds / timeRange * width;
                        double y1 = height - ((prev.Value - minVal) / valRange * height);
                        double y2 = height - ((curr.Value - minVal) / valRange * height);

                        var line = new Line
                        {
                            X1 = x1,
                            Y1 = y1,
                            X2 = x2,
                            Y2 = y2,
                            Stroke = color,
                            StrokeThickness = 2,
                            Effect = shadowEffect

                        };
                        //ToolTipService.SetToolTip(line, $"{curr.Timestamp:HH:mm:ss}\n{curr.Value:F2}");
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var tooltip = new ToolTip { Content = $"{curr.Timestamp:HH:mm:ss}\n{curr.Value:F2}" };
                            ToolTipService.SetToolTip(line, tooltip);
                        });
                        GraphCanvas.Children.Add(line);

                        // Create a transparent ellipse at the current data point for tooltip
                        var ellipse = new Ellipse
                        {
                            Width = 10,
                            Height = 10,
                            Fill = Brushes.Transparent,
                            Stroke = Brushes.Transparent,
                            StrokeThickness = 0,
                            Effect = shadowEffect
                        };
                        Canvas.SetLeft(ellipse, x2 - 5); // Center the ellipse horizontally 
                        Canvas.SetTop(ellipse, y2 - 5);  // Center the ellipse vertically

                        // Assign tooltip to the ellipse
                        // ToolTipService.SetToolTip(ellipse, $"{curr.Timestamp:HH:mm:ss}\n{curr.Value:F2}");
                        ellipse.MouseEnter += (sender, args) =>
                        {
                            ShowPopup($"{curr.Timestamp:HH:mm:ss}\n{curr.Value:F2}", x2, y2);
                        };

                        GraphCanvas.Children.Add(ellipse);

                    }
                }

                DrawWave(above, Brushes.Red);    // 🔴 Above threshold
                DrawWave(within, Brushes.Green); // 🟢 Normal range
                DrawWave(below, Brushes.Blue);   // 🔵 Below threshold



                int horizontalDivisions = 20;
                for (int i = 0; i <= horizontalDivisions; i++)
                {
                    double y = i * height / horizontalDivisions;
                    var gridLine = new Line
                    {
                        X1 = 0,
                        Y1 = y,
                        X2 = width,
                        Y2 = y,
                        Stroke = Brushes.LightGray,
                        StrokeThickness = 0.5
                    };
                    GraphCanvas.Children.Add(gridLine);



                    var yLabel = new TextBlock
                    {
                        Text = string.Format("{0:F0}", maxVal - (valRange * i / horizontalDivisions)),
                        FontSize = 8,
                        Foreground = Brushes.Gray
                    };
                    Canvas.SetLeft(yLabel, 0);
                    Canvas.SetTop(yLabel, y - 10);
                    GraphCanvas.Children.Add(yLabel);
                }

                int verticalDivisions = 20;
                for (int i = 0; i <= verticalDivisions; i++)
                {
                    double x = i * width / verticalDivisions;
                    var gridLine = new Line
                    {
                        X1 = x,
                        Y1 = 0,
                        X2 = x,
                        Y2 = height,
                        Stroke = Brushes.LightGray,
                        StrokeThickness = 0.5
                    };
                    GraphCanvas.Children.Add(gridLine);

                    var timeLabel = new TextBlock
                    {
                        Text = minTime.AddSeconds(i * timeRange / verticalDivisions).ToString("mm:ss"),
                        FontSize = 8,
                        Foreground = Brushes.Gray
                    };
                    Canvas.SetLeft(timeLabel, x + 2);
                    Canvas.SetTop(timeLabel, height - 20);
                    GraphCanvas.Children.Add(timeLabel);

                }
            });
        }

        private void DrawHistogram()
        {
            HistoCanvas.Children.Clear();
            var data = _viewModel.SensorDataList;
            if (data.Count == 0) return;

            double width = HistoCanvas.ActualWidth;
            double height = HistoCanvas.ActualHeight;

            int above = data.Count(d => d.Value > _viewModel.Threshold.UpperLimit);
            int within = data.Count(d => d.Value <= _viewModel.Threshold.UpperLimit && d.Value >= _viewModel.Threshold.LowerLimit);
            int below = data.Count(d => d.Value < _viewModel.Threshold.LowerLimit);
            int total = above + within + below;

            if (total == 0) return;

            double barWidth = width / 3;
            double scale = height / total;

            void DrawBar(int count, Brush brush, double x, string label)
            {
                double barHeight = count * scale;
                var rect = new Rectangle
                {
                    Width = barWidth * 0.6,
                    Height = barHeight,
                    Fill = brush,
                    ToolTip = $"{label}: {count} ({(double)count / total:P1})"
                };
                Canvas.SetLeft(rect, x + barWidth * 0.2);
                Canvas.SetTop(rect, height - barHeight);
                HistoCanvas.Children.Add(rect);

                var text = new TextBlock
                {
                    Text = $"{label}\n{count}",
                    TextAlignment = TextAlignment.Center,
                    Foreground = Brushes.Black
                };
                Canvas.SetLeft(text, x + barWidth * 0.1);
                Canvas.SetTop(text, height - barHeight - 40);
                HistoCanvas.Children.Add(text);
            }


            DrawBar(above, Brushes.Red, 0 * barWidth, "Above");
            DrawBar(within, Brushes.Green, 1 * barWidth, "Within");
            DrawBar(below, Brushes.Blue, 2 * barWidth, "Below");
        }

        void ShowPopup(string content, double x, double y)
        {
            // Create the popup
            var popup = new Popup
            {
                Placement = PlacementMode.Relative,
                StaysOpen = false,
                IsOpen = true,
                PlacementTarget = GraphCanvas,
                HorizontalOffset = x,
                VerticalOffset = y,
                Child = new Border
                {
                    Background = Brushes.LightYellow,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1),
                    Child = new TextBlock
                    {
                        Text = content,
                        Foreground = Brushes.Black,
                        FontSize = 12,
                        Padding = new Thickness(4)
                    }
                }
            };


        }
    }
}
