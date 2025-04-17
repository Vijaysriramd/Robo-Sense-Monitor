using Robo_Sense_Monitor.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robo_Sense_Monitor.Model
{
    //internal class ModelData
    //{
    //}
    public enum CommunicationType 
    { 
        Serial, 
        TCP, 
        GRPC 
    }
    public enum LogTarget 
    { 
        File, 
        Database, 
        Console, 
        EventViewer 
    }
    public enum LogLevel { 
        Info, 
        Warning, 
        Error, 
        Critical 
    }

    public class DeviceInfo : INotifyPropertyChanged
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                    
                }
            }
        }
        public int DeviceId { get; set; }
        //public int DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string Type { get; set; }
        public string IPAddress { get; set; }
        public string PortName { get; set; }

        public int baudRate { get; set; }
        public int DataBits { get; set; }
        public int StopBits { get; set; }
        public int Parity { get; set; }
        public bool FlowControl { get; set; }

        //public bool IsListening { get; set; }

        private bool _isListening;
        public bool IsListening
        {
            get => _isListening;
            set
            {
                if (_isListening != value)
                {
                    _isListening = value;
                    OnPropertyChanged(nameof(IsListening));
                   // NotifyUserListeningStatus(); // Additional notification method
                }
            }
        }
        // public  DataCollectorService DataService { get; set; }
        //public WcSendorPloting objplot { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SensorData
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int DeviceId { get; set; }
        public double Value { get; set; }
    }

    public class Threshold
    {
        public int DeviceId { get; set; }
        public double LowerLimit { get; set; }
        public double UpperLimit { get; set; }
    }

    public class LogEntry
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Source { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public LogLevel Level { get; set; } = LogLevel.Error;
    }

    // Interfaces 
    public interface IDeviceCommunicator
    {
        event Action<SensorData> DataReceived;
        //void Connect(DeviceInfo deviceInfo);
        //void Disconnect();
        Task StartListening(Action<string> onDataReceived, CancellationToken token);
        

    }
    public class ThresholdStats
    {
        public double AbovePercentage { get; set; }
        public double NormalPercentage { get; set; }
        public double BelowPercentage { get; set; }
    }

    public interface ILogWriter
    {
        void WriteLog(LogEntry entry);
    }
}
