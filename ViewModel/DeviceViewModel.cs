using Robo_Sense_Monitor.Model;
using Robo_Sense_Monitor.Model;
using Robo_Sense_Monitor.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robo_Sense_Monitor.ViewModel
{
    public class DeviceViewModel
    {
        private IDeviceCommunicator _communicator;
        //private SensorDataViewModel _viewModel = new();
        private SqlConnection _connection;
        private readonly RepositoryRecorder _RepositoryRecorderr;
        DeviceInfo dev = new DeviceInfo();

        public DeviceViewModel()
        {

            string connectionString = ConfigurationSettings.AppSettings["ConnectionString"];
            _RepositoryRecorderr = new RepositoryRecorder(connectionString);
        }
        public void ConnectDevice(DeviceInfo info)
        {
            try
            {
                //_communicator = CommunicatorFactory.Create(info);
                //_communicator.DataReceived += OnDataReceived;
                //_communicator.Connect(info);
                //_communicator.StartListening();
            }
            catch (Exception ex)
            {
                //_logger.Log("MainWindow", "Failed to connect to device", ex);
            }
        }

        public void OnDataReceived(SensorData data)
        {
            //_viewModel.AddSensorData(data);
            _RepositoryRecorderr.SaveSensorDataToDatabase(data);
            //DrawGraph();
        }
        public List<SensorData> GetPlaybackdata(int _deviceid, DateTime start, DateTime end)
        {
            return _RepositoryRecorderr.FetchSensorData(_deviceid, start, end);
        }
        public async Task<List<SensorData>> GetOptimizedPlaybackDataAsync(int deviceId, DateTime start, DateTime end)
        {
            // Validate input parameters
            if (deviceId <= 0)
                throw new ArgumentException("Device ID must be positive", nameof(deviceId));

            if (start > end)
                throw new ArgumentException("Start time must be before end time", nameof(start));

            try
            {
                // Call repository method and await the result
                var result = await _RepositoryRecorderr.GetOptimizedPlaybackDataAsync(deviceId, start, end);

                // Return empty list rather than null if no data found
                return result ?? new List<SensorData>();
            }
            catch (Exception ex)
            {
                // Log the error (implementation depends on your logging framework)
                //  _logger.LogError(ex, $"Error retrieving playback data for device {deviceId} between {start} and {end}");

                // Consider throwing a custom exception or returning empty list
                return new List<SensorData>();
            }
        }

        public bool InsertorUpdateThreshold(Threshold tr)
        {
            return _RepositoryRecorderr.InsertorUpdateThreshold(tr);
        }
        public List<Threshold> FetchThresholdSetup(int _deviceid)
        {
            return _RepositoryRecorderr.FetchThresholdSetup(_deviceid);
        }
        public List<DeviceInfo> FetchDeviceInfo()
        {
            return _RepositoryRecorderr.FetchDeviceInfo();
        }
        public bool InsertorUpdateDeviceInfo(List<DeviceInfo> devices)
        {
            return _RepositoryRecorderr.InsertorUpdateDeviceInfo(devices);
        }
        public async Task<ThresholdStats> GetTodayThresholdStatsAsync(int deviceId)
        {

            if (deviceId <= 0)
                throw new ArgumentException("Device ID must be positive", nameof(deviceId));
            try
            {
                var result = await _RepositoryRecorderr.GetTodayThresholdStatsAsync(deviceId);

                return result ?? new ThresholdStats();
            }
            catch (Exception ex)
            {

                return new ThresholdStats();
            }

        }
    }
}
