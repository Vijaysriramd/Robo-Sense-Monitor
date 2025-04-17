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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Robo_Sense_Monitor.Converter;

namespace Robo_Sense_Monitor
{
    /// <summary>
    /// Interaction logic for DeviceGraph.xaml
    /// </summary>
    public partial class DeviceGraph : Window
    {
        private readonly List<DeviceGraphViewModel> _viewModels;
        public DeviceGraph(IEnumerable<DeviceInfo> devices, DataCollectorService collector,bool Isplayback)
        {
            InitializeComponent();

            _viewModels = devices.Select(d => new DeviceGraphViewModel(d.DeviceId, collector)).ToList();

            foreach (var vm in _viewModels)
            {
                DeviceInfo dev = devices.FirstOrDefault(d => d.DeviceId == vm.Deviceid);

                var tab = new TabItem { Header =dev.DeviceName };
                //var canvas = new Canvas { Background = Brushes.White };
               
                tab.Content = new UcGraphPloting(vm.DataPoints,dev, Isplayback, collector);
                DeviceTabs.Items.Add(tab);

               // vm.DataPoints.CollectionChanged += (s, e) => DrawGraph(canvas, vm.DataPoints);
                //vm.SwitchMode(true);
            }
        }
        private void DrawGraph(Canvas canvas, ObservableCollection<DataPoint> dataPoints)
        {
            canvas.Children.Clear();
            if (dataPoints.Count < 2) return;

            double width = canvas.ActualWidth;
            double height = canvas.ActualHeight;
            if (width == 0 || height == 0) return;

            double maxVal = dataPoints.Max(dp => dp.Value);
            double minVal = dataPoints.Min(dp => dp.Value);
            double range = maxVal - minVal == 0 ? 1 : maxVal - minVal;

            double xStep = width / (dataPoints.Count - 1);

            for (int i = 1; i < dataPoints.Count; i++)
            {
                var color = Brushes.Blue;
                var val = dataPoints[i].Value;
                if (val < 30)
                    color = Brushes.Green;
                else if (val > 70)
                    color = Brushes.Red;

                var line = new Line
                {
                    X1 = (i - 1) * xStep,
                    Y1 = height - ((dataPoints[i - 1].Value - minVal) / range * height),
                    X2 = i * xStep,
                    Y2 = height - ((dataPoints[i].Value - minVal) / range * height),
                    Stroke = color,
                    StrokeThickness = 2
                };
                canvas.Children.Add(line);
            }
        }

    }
}
