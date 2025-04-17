using Robo_Sense_Monitor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RJCP.IO.Ports;
using System.Windows.Threading;
using Robo_Sense_Monitor.Repository;
using Robo_Sense_Monitor.ViewModel;
using System.Windows;
using System.Data;
using Robo_Sense_Monitor.Model;

namespace Robo_Sense_Monitor.Communicators
{
    public class SerialCommunicator : IDeviceCommunicator
    {
        public event Action<SensorData> DataReceived;
       
        private SerialPortStream _serialPort;

        private DeviceViewModel _deviceViewModel = new DeviceViewModel();
        private DeviceInfo _deviceInfo;

        public SerialCommunicator(DeviceInfo device)
        {
            
            _serialPort = new SerialPortStream
            {
                PortName = device.PortName, // or COMx
                BaudRate = device.baudRate, // Optional: make configurable
                DataBits = device.DataBits,
                StopBits = (StopBits)device.StopBits,
                Parity = (Parity)device.Parity,
                Handshake = Handshake.None, // Or map from FlowControl
                ReadTimeout = 1000
            };
        }
        public async Task StartListening(Action<string> onDataReceived, CancellationToken token)
        {
            _serialPort.Open();

            try
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        string line = _serialPort.ReadLine();
                        onDataReceived(line);
                    }
                    catch (TimeoutException)
                    {
                        // Ignore timeout
                    }
                   // await Task.Delay(50, token); // Slight delay to prevent tight loop
                }
            }
            finally
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();
            }
        }
        // public string ConnectionStatus { get; set; }
        //public void Connect(DeviceInfo deviceInfo)
        //{
        //    //ConnectionStatus = "Not Connected";
        //    if (_serialPort != null)
        //    {
        //        if (_serialPort.IsOpen)
        //        {
        //            _serialPort.Close(); // Close the serial port
        //        }

        //        _serialPort.Dispose(); // Dispose of the object to release resources
        //        _serialPort = null;    // Clear the reference to allow garbage collection
        //    }

        //    _deviceInfo = deviceInfo;
        //    _serialPort = new SerialPortStream(deviceInfo.PortName.ToString())
        //    {
        //        BaudRate = 9600,
        //        DataBits = deviceInfo.DataBits,
        //        StopBits = (RJCP.IO.Ports.StopBits)deviceInfo.StopBits,
        //        Parity = (Parity)deviceInfo.Parity
        //    };

        //    _serialPort.DataReceived += (s, e) =>
        //    {
        //        string data = _serialPort.ReadLine();
        //        //ConnectionStatus = "Connected";
        //        //var sensorData = ParseData(data);
        //        //DataReceived?.Invoke(sensorData);
       
        //    };

        //    _serialPort.Open();

        //}

        //public  void StartListening() {
         
        //    _serialPort.DataReceived += (s, e) =>
        //    {
        //        string data = _serialPort.ReadLine();
        //        //double value = Convert.ToDouble(data.Replace("\r",""));
        //        if (double.TryParse(data, out double value))
        //        {
        //            // Update the chart on the UI thread
        //            SensorData sensorData = new SensorData();    
        //            sensorData.DeviceId = _deviceInfo.DeviceId;
        //            sensorData.Timestamp = DateTime.Now;
        //            sensorData.Value = value;

        //            //OnDataReceived?.Invoke(DateTime.Now, value);
        //            Application.Current.Dispatcher.Invoke(() =>
        //            {
        //                //  AddPointToGraph(value);
        //                DataReceived?.Invoke(sensorData);
        //                _deviceViewModel.OnDataReceived(sensorData);
                        
                      
        //                // Record the value to the database (optional)
        //                //_RepositoryRecorderr.SaveData(value);
        //            });
        //        }

        //    };
            //_serialPort.Open();
        //}

        public void Disconnect() => _serialPort?.Close();

        private SensorData ParseData(string raw)
        {
            // Custom parse logic
            return new SensorData { Value = double.Parse(raw), Timestamp = DateTime.Now };
        }
    }

}
