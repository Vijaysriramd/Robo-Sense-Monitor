using Robo_Sense_Monitor.Model;
using Robo_Sense_Monitor.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robo_Sense_Monitor
{
    public class DataReceivedEventArgs : EventArgs
    {
        public int DeviceId { get; set; }
        public string value { get; set; }
        public DateTime Timestamp { get; set; }
    }
    public class DataCollectorService
    {
        private readonly Dictionary<string, CancellationTokenSource> _deviceCtsMap = new();
        public event EventHandler<DataReceivedEventArgs> DataReceived;

        private DeviceViewModel _deviceViewModel = new DeviceViewModel();
        private readonly List<CancellationTokenSource> _cancellationTokens = new();
        

        public void StartMonitoringDevices(IEnumerable<DeviceInfo> devices)
        {
            foreach (var device in devices)
            {
                if (device.IsSelected)
                {
                    if (_deviceCtsMap.ContainsKey(device.DeviceId.ToString()))
                        continue; // Already listening

                    var connection = CommunicatorFactory.Create(device);
                    var cts = new CancellationTokenSource();
                    _deviceCtsMap[device.DeviceId.ToString()] = cts;

                    Task.Run(async () =>
                    {
                        try
                        {
                            await connection.StartListening(data =>
                            {
                                SaveToDatabase(device, data);
                            }, cts.Token);
                        }
                        catch (Exception ex)
                        {
                            // Optionally log error
                        }
                    });
                }
            }
        }

        public void StopMonitoring()
        {
            foreach (var kvp in _deviceCtsMap)
            {
                kvp.Value.Cancel();
            }

            _deviceCtsMap.Clear();
        }

        public void StopMonitoringDevice(string deviceId)
        {
            if (_deviceCtsMap.TryGetValue(deviceId, out var cts))
            {
                cts.Cancel();
                _deviceCtsMap.Remove(deviceId);
            }
        }

        private void SaveToDatabase(DeviceInfo device, string data)
        {
            var timestamp = DateTime.Now;

            // Notify live graph subscribers
            DataReceived?.Invoke(this, new DataReceivedEventArgs
            {
                DeviceId = device.DeviceId,
                value = data,
                Timestamp = timestamp
            });
            SensorData sensorData = new SensorData();
            sensorData.DeviceId = device.DeviceId;
            sensorData.Value = Convert.ToInt32(data);
            sensorData.Timestamp = timestamp;

            // Save to SQL DB
            _deviceViewModel.OnDataReceived(sensorData);
        }
    }

    //public class DataCollectorService
    //{
    //    private DeviceViewModel _deviceViewModel = new DeviceViewModel();
    //    private readonly List<CancellationTokenSource> _cancellationTokens = new();
    //    public event EventHandler<DataReceivedEventArgs> DataReceived;



    //    public void StartMonitoringDevices(IEnumerable<DeviceInfo> devices)
    //    {

    //        foreach (var device in devices)
    //        {
    //            if (device.IsSelected)
    //            {
    //                var connection = CommunicatorFactory.Create(device);
    //                var cts = new CancellationTokenSource();
    //                _cancellationTokens.Add(cts);
    //                Task.Run(async () =>
    //                {
    //                    try
    //                    {

    //                        await connection.StartListening(data =>
    //                        {
    //                            try
    //                            {
    //                                SaveToDatabase(device, data);
    //                            }
    //                            catch (Exception exp)
    //                            {
    //                                Debug.WriteLine($"Error listening to device {device.DeviceName}: {exp.Message}");
    //                            }


    //                        }, cts.Token);

    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        Debug.WriteLine($"Error listening to device {device.DeviceName}: {ex.Message}");
    //                    }
    //                });

    //            }
    //        }
    //    }

    //    public void StopMonitoring()
    //    {
    //        foreach (var cts in _cancellationTokens)
    //            cts.Cancel();
    //    }

    //    private void SaveToDatabase(DeviceInfo device, string data)
    //    {
    //        var timestamp = DateTime.Now;

    //        // Notify live graph subscribers
    //        DataReceived?.Invoke(this, new DataReceivedEventArgs
    //        {
    //            DeviceId = device.DeviceId,
    //            value = data,
    //            Timestamp = timestamp
    //        });
    //        SensorData sensorData = new SensorData();
    //        sensorData.DeviceId= device.DeviceId;
    //        sensorData.Value = Convert.ToInt32(data);
    //        sensorData.Timestamp = timestamp;

    //        // Save to SQL DB
    //        _deviceViewModel.OnDataReceived(sensorData); 
    //    }
    //}
}
