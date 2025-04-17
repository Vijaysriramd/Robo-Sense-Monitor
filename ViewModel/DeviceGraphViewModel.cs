using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Robo_Sense_Monitor.ViewModel
{
    public class DataPoint
    {
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
    }

    public class DeviceGraphViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<DataPoint> DataPoints { get; set; } = new();
        public bool IsLiveMode { get; set; }

        private readonly int _deviceId;
        public int Deviceid
        {
            get { return _deviceId; } 
        }
        private readonly DataCollectorService _collectorService;


        public DeviceGraphViewModel(int deviceId, DataCollectorService collectorService)
        {
            _deviceId = deviceId;
            //_collectorService = collectorService;
            //_collectorService.DataReceived += OnDataReceived;
        }

        public void SwitchMode(bool live)
        {
            IsLiveMode = live;

            if (!IsLiveMode)
            {
                LoadDataFromDatabase(_deviceId);
            }
            else
            {
                DataPoints.Clear();
            }
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (IsLiveMode && e.DeviceId == _deviceId)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    DataPoints.Add(new DataPoint
                    {
                        Timestamp = e.Timestamp,
                        Value = ParseValue(e.value)
                    });
                });
            }
        }

        private void LoadDataFromDatabase(int deviceId)
        {
            // Load and populate DataPoints from DB
        }

        private double ParseValue(string raw)
        {
            return double.TryParse(raw, out var val) ? val : 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
