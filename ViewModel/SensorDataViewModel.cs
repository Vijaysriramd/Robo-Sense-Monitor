using Microsoft.Win32;
using Robo_Sense_Monitor.Commands;
using Robo_Sense_Monitor.Converter;
using Robo_Sense_Monitor.Model;
using Robo_Sense_Monitor.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;


namespace Robo_Sense_Monitor.ViewModel
{
    public class SensorDataViewModel : INotifyPropertyChanged
    {
        // Properties for data binding
        private bool _isPlaybackMode;
        public bool IsPlaybackMode
        {
            get => _isPlaybackMode;
            set
            {
                _isPlaybackMode = value;
                if (value) IsLiveMode = false;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsLiveMode)); // Inverse of playback
            }
        }

       // public bool IsLiveMode => !IsPlaybackMode;

        private bool _isLiveMode = true;
        public bool IsLiveMode
        {
            get => _isLiveMode;
            set
            {
                if (_isLiveMode != value)
                {
                    _isLiveMode = value;
                    OnPropertyChanged(nameof(IsLiveMode));
                }
            }
        }

        private DateTime? _playbackStartDate;
        public DateTime? PlaybackStartDate
        {
            get => _playbackStartDate;
            set
            {
                _playbackStartDate = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _playbackStartTime;
        public DateTime? PlaybackStartTime
        {
            get => _playbackStartTime;
            set
            {
                _playbackStartTime = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _playbackEndDate;
        public DateTime? PlaybackEndDate
        {
            get => _playbackEndDate;
            set
            {
                _playbackEndDate = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _playbackEndTime;
        public DateTime? PlaybackEndTime
        {
            get => _playbackEndTime;
            set
            {
                _playbackEndTime = value;
                OnPropertyChanged();
            }
        }

        private DeviceViewModel _deviceViewModel = new DeviceViewModel();
        private TimeSpan _displayDuration = TimeSpan.FromMinutes(1);
        private DateTime _currentEndTime = DateTime.Now;
        private ObservableCollection<SensorData> _visibleSensorData = new();
        public ObservableCollection<SensorData> VisibleSensorData => _visibleSensorData;

        private ObservableCollection<SensorData> _runningData = new(); // Live running data
       
        public ObservableCollection<SensorData> RunningData => _runningData; // Live data

        public ObservableCollection<DataPoint> DataPoints { get; set; } = new();
        private int _currentIndex = 0; // Tracks the starting point of the visible window
        private int _windowSize = 200; // Number of data points to display

        // Add these commands:
        public ICommand ZoomInCommand { get; }
        public ICommand ZoomOutCommand { get; }
        public ICommand ScrollLeftCommand { get; }
        public ICommand ScrollRightCommand { get; }
        public ICommand ResumeLiveCommand { get; }
        public ICommand ExportDataCommand { get; }
        public ICommand ImportDataCommand { get; }
        public ICommand LoadDataCommand { get; }
        public ICommand UpdateThresholdCommand { get; }
        public ICommand PlayDataCommand { get; }


        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                if (_currentIndex != value)
                {
                    _currentIndex = value;
                    OnPropertyChanged(nameof(CurrentIndex));
                }
            }
        }

        public int WindowSize
        {
            get => _windowSize;
            set
            {
                if (_windowSize != value)
                {
                    _windowSize = value;
                    OnPropertyChanged(nameof(WindowSize));
                }
            }
        }


    

        private DispatcherTimer _liveUpdateTimer;

        public void StartLiveUpdates()
        {
            _liveUpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500) // Adjust interval as needed
            };

            _liveUpdateTimer.Tick += (s, e) =>
            {
                if (IsLiveMode) // Only update in live mode
                    UpdateVisibleData();
            };

            _liveUpdateTimer.Start();
        }



        // Zoom level controls the range of visible data
        //private int _zoomLevel = 10; // Default zoom level: 10 data points
        private double _zoomLevel = 1.0; // Default zoom level
        public double ZoomLevel
        {
            get => _zoomLevel;
            set
            {
                if (_zoomLevel != value)
                {
                    _zoomLevel = value;
                    UpdateVisibleData(); // Refresh visible data on zoom change
                }
            }
        }


        public void StopLiveUpdates()
        {
            _liveUpdateTimer?.Stop();
        }


        //public ObservableCollection<SensorData> SensorDataList { get; private set; } = new();
        public Dictionary<string, int> HistogramBins
        {
            get
            {
                var bins = new Dictionary<string, int>
            {
                { "Below", 0 },
                { "Within", 0 },
                { "Above", 0 }
            };

                foreach (var data in SensorDataList)
                {
                    if (data.Value < Threshold.LowerLimit)
                        bins["Below"]++;
                    else if (data.Value > Threshold.UpperLimit)
                        bins["Above"]++;
                    else
                        bins["Within"]++;
                }

                return bins;
            }
        }
        private ObservableCollection<SensorData> _sensorDataList = new();
        public ObservableCollection<SensorData> SensorDataList => _sensorDataList;

        private bool _isplayback;
        public bool Isplayback
        {
            get => _isplayback;
            set
            {
                if (_isplayback != value)
                {
                    _isplayback = value;
                    OnPropertyChanged(nameof(Isplayback));
                }
            }
        }


        private string _ThresHoldValue;
        public string ThresHoldValue
        {
            get => _ThresHoldValue;
            set
            {
                if (_ThresHoldValue != value)
                {
                    _ThresHoldValue = value;
                    OnPropertyChanged(nameof(ThresHoldValue));
                }
            }
        }



        private double _timeScale = 1.0; // Time scale factor (1.0 = default scale)
        public double TimeScale
        {
            get => _timeScale;
            set
            {
                if (_timeScale != value)
                {
                    _timeScale = value;
                    OnPropertyChanged(nameof(TimeScale));
                }
            }
        }

        public Threshold Threshold { get; set; }

        private bool _isPlayingBack;
        private int _playbackIndex = 0;
        private List<SensorData> _playbackData;
        private DispatcherTimer _playbackTimer;
        private DeviceInfo Devicedet;

        private TimeSpan _playbackInterval = TimeSpan.FromSeconds(1);
        public bool  isDrawgraph { get; set; }
        private readonly DataCollectorService _collectorService;
        public TimeSpan PlaybackInterval
        {
            get => _playbackInterval;
            set
            {
                _playbackInterval = value;
                _playbackTimer.Interval = value;
            }
        }

        private PlaybackController _playbackController;
        private double _playbackSpeed = 1.0; // Default speed (1x)

        public double PlaybackSpeed
        {
            get => _playbackSpeed;
            //set => SetProperty(ref _playbackSpeed, Math.Clamp(value, 0.1, 10.0)); // Limit between 0.1x and 10x
            set
            {
                _playbackSpeed = Math.Clamp(value, 0.1, 10.0); // Limit between 0.1x and 10x
                OnPropertyChanged(nameof(PlaybackSpeed));
            }

        }

        private string _DeviceInformation;
        public string DeviceInformation { get => _DeviceInformation; 
            set {
                _DeviceInformation = value; 
                OnPropertyChanged("DeviceInformation"); 
            } }
        private SensorData sdata;
        public SensorDataViewModel( DataCollectorService collectorService, DeviceInfo deV)
        {
            
            Devicedet = deV;
            //Threshold = new Threshold { DeviceId = Devicedet.DeviceId, LowerLimit = 30, UpperLimit = 70 }; // Example thresholds
            Threshold = _deviceViewModel.FetchThresholdSetup(Devicedet.DeviceId).FirstOrDefault();
            if (Threshold != null && Threshold != null)
            {
                ThresHoldValue = $"{Threshold.LowerLimit}-{Threshold.UpperLimit}";
            }
            else
            {
                Threshold = new Threshold { DeviceId = Devicedet.DeviceId, LowerLimit = 30, UpperLimit = 70 };
            }
                // Initialize commands
                ImportDataCommand = new RelayCommand<ImportParameters>(ImportData, CanImportData);
            ExportDataCommand = new RelayCommand<ExportParameters>(ExportDataAsync, CanExportData);
            PlayDataCommand = new RelayCommand<PlaybackParameters>(PlayData, CanPlayData);

            //ZoomInCommand = new RelayCommand(ZoomIn);
            //ZoomOutCommand = new RelayCommand(ZoomOut);
            //ScrollLeftCommand = new RelayCommand(ScrollLeft);
            //ScrollRightCommand = new RelayCommand(ScrollRight);
            //ResumeLiveCommand = new RelayCommand(ResumeLiveData);

            //LoadDataCommand = new RelayCommand(LoadData);
            //UpdateThresholdCommand = new RelayCommand(UpdateThreshold);

            // Parameterless commands
            ZoomInCommand = new RelayCommand(ZoomIn);
            ZoomOutCommand = new RelayCommand(ZoomOut);
            ScrollLeftCommand = new RelayCommand(ScrollLeft);
            ScrollRightCommand = new RelayCommand(ScrollRight);
            ResumeLiveCommand = new RelayCommand(ResumeLiveData);

            // Commands with parameters
            LoadDataCommand = new RelayCommand<object>(LoadData);
            UpdateThresholdCommand = new RelayCommand<string>(UpdateThreshold);

            _playbackTimer = new DispatcherTimer
            {
                Interval = _playbackInterval
            };
            _playbackTimer.Tick += PlaybackStep;


            PlaybackStartDate = DateTime.Today.AddDays(-1);
            PlaybackEndDate = DateTime.Today;
            PlaybackStartTime = DateTime.Today.AddHours(9); // 9 AM
            PlaybackEndTime = DateTime.Today.AddHours(17); // 5 PM


            _collectorService = collectorService;
            AddDeviceInfo(Devicedet);
            _collectorService.DataReceived += OnDataReceived;


            //  _currentEndTime = DateTime.Now + TimeSpan.FromMinutes(0.5); ;

            // DeviceInformation = "Plot for Device 1";
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (IsLiveMode && e.DeviceId == Devicedet.DeviceId)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    
                    sdata = new SensorData();
                    sdata.DeviceId=Devicedet.DeviceId;
                    sdata.Timestamp = e.Timestamp;
                    sdata.Value = ParseValue(e.value);
                    
                    AddSensorData(sdata);
                    


                });
            }

         
        }
        private double ParseValue(string raw)
        {
            return double.TryParse(raw, out var val) ? val : 0;
        }
        private bool CanImportData(ImportParameters parameters)
        {
            return true;
        }

        private void ImportData(ImportParameters parameters)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                Title = $"Import Data for Device {parameters.DeviceId}"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var importedData = CsvImporter.LoadSensorDataFromCsv(openFileDialog.FileName);

                    if (parameters.IsPlayback)
                    {
                        LoadPlaybackData(importedData);
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            SensorDataList.Clear();
                            foreach (var item in importedData)
                            {
                                AddSensorData(item);
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to import data: {ex.Message}", "Import Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanExportData(ExportParameters parameters)
        {
            return parameters?.StartDateTime != null &&
                   parameters?.EndDateTime != null &&
                   parameters.StartDateTime < parameters.EndDateTime ;
        }

        private async void ExportDataAsync(ExportParameters parameters)
        {
            try
            {
                var data = await Task.Run(() =>
                    GetDataInRange(parameters.StartDateTime.Value, parameters.EndDateTime.Value));

                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV Files (*.csv)|*.csv",
                    FileName = $"Export_{Devicedet.DeviceName}_{DateTime.Now:yyyyMMddHHmmss}.csv"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    await Task.Run(() => CsvExporter.ExportToCsv(data, saveFileDialog.FileName));
                    MessageBox.Show("Export completed successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                _playbackTimer.Stop();
                _isPlayingBack = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export failed: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //private void ExportData(ExportParameters parameters)
        //{
        //    try
        //    {
        //       // var data = GetDataInRange(parameters.StartDateTime.Value, parameters.EndDateTime.Value);
        //        string defaultFileName = $"Export_{parameters.Device.DeviceName}_{DateTime.Now:yyyyMMddHHmmss}.csv";

        //        var saveFileDialog = new SaveFileDialog
        //        {
        //            Filter = "CSV Files (*.csv)|*.csv",
        //            FileName = defaultFileName
        //        };

        //        if (saveFileDialog.ShowDialog() == true)
        //        {
        //           // CsvExporter.ExportToCsv(data, saveFileDialog.FileName);
        //            MessageBox.Show("Data exported successfully!", "Export Complete",
        //                MessageBoxButton.OK, MessageBoxImage.Information);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Failed to export data: {ex.Message}", "Export Error",
        //            MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        private bool CanPlayData(PlaybackParameters parameters)
        {
            return parameters?.StartDateTime != null &&
                   parameters?.EndDateTime != null &&
                   parameters.StartDateTime < parameters.EndDateTime;
        }
        private async void PlayData(PlaybackParameters parameters)
        {
            if (parameters?.StartDateTime == null || parameters?.EndDateTime == null)
            {
                MessageBox.Show("Invalid date range parameters", "Playback Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                IsPlaybackMode = true;

                // Get data asynchronously
                var playbackData = await Task.Run(() =>
                    GetDataInRange(parameters.StartDateTime.Value, parameters.EndDateTime.Value));

                // Load the data (assuming LoadPlaybackData expects List<SensorData>)
                LoadPlaybackData(playbackData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to play data: {ex.Message}", "Playback Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                IsPlaybackMode = false;
            }
        }
      

        private async Task<List<SensorData>> GetDataInRange(DateTime start, DateTime end)
        {
            int deviceid=Devicedet.DeviceId;
            //return _deviceViewModel.GetPlaybackdata(deviceid, start, end)
            //                       .OrderBy(d => d.Timestamp)
            //                       .ToList(); //SelectedDevice.DeviceId
            //return _deviceViewModel.GetPlaybackdata(deviceid, start, end)
            //                       .OrderBy(d => d.Timestamp)
            //
            // .ToList(); //SelectedDevice.DeviceId
            // Validate input parameters
            

            if (start > end)
                throw new ArgumentException("Start time must be before end time", nameof(start));

            try
            {
                // Call repository method and await the result
                var result = await _deviceViewModel.GetOptimizedPlaybackDataAsync(deviceid, start, end);

                // Return empty list rather than null if no data found
                return result ?? new List<SensorData>();
            }
            catch (Exception ex)
            {
                // Log the error (implementation depends on your logging framework)
                //_logger.LogError(ex, $"Error retrieving playback data for device {deviceId} between {start} and {end}");

                // Consider throwing a custom exception or returning empty list
                return new List<SensorData>();
            }
        }

        private void ZoomIn(object parameter)
        { 
        }
        private void ZoomOut(object parameter)
        {
        }
        private void ScrollLeft(object parameter)
        {
            if (_currentIndex - _windowSize >= 0)  
            {
                IsLiveMode = false; 
                _currentIndex -= _windowSize;
                //UpdateVisibleDataByIndex();
                if (_currentIndex - (int)(_zoomLevel * 10) >= 0) // Ensure scrolling doesn't exceed the start of the collection
                {
                    _currentIndex -= (int)(_zoomLevel * 10); // Move left by the zoom level range
                    UpdateVisibleData(); // Refresh data
                }
            }
        }
        private void ScrollRight()
        {
            if (_currentIndex + _windowSize < _sensorDataList.Count) // Ensure we don't go out of bounds
            {
                IsLiveMode = false; // Disable live mode while scrolling
                _currentIndex += _windowSize;
                // UpdateVisibleDataByIndex();
                if (_currentIndex + (int)(_zoomLevel * 10) < SensorDataList.Count) // Ensure scrolling doesn't exceed the end of the collection
                {
                    _currentIndex += (int)(_zoomLevel * 10); // Move right by the zoom level range
                    UpdateVisibleData(); // Refresh data
                }
            }
            else
            {
                ResumeLiveData(); // Automatically return to live mode if scrolling reaches live data
            }
        }
        private void ResumeLiveData(object parameter)

        {
            //IsLiveMode = true; // Enable live mode
            //_currentIndex = Math.Max(0, _sensorDataList.Count - _windowSize); // Show the latest data
            //UpdateVisibleDataByIndex();
            IsLiveMode = true;

            // Clear the historical data settings and focus on live data
            _currentIndex = Math.Max(0, _runningData.Count - (int)(_zoomLevel * 10)); // Determine the starting point for live data
            UpdateVisibleData();
        }
 
        private void LoadData(Object parameters)
        {
        }
        private void UpdateThreshold(string thresholdText)
        {
            if (string.IsNullOrEmpty(thresholdText)) return;

            string[] parts = thresholdText.Split('-');
            if (parts.Length == 2 &&
                int.TryParse(parts[0], out int lowerLimit) &&
                int.TryParse(parts[1], out int upperLimit))
            {
                Threshold = new Threshold
                {
                    DeviceId=Devicedet.DeviceId,
                    LowerLimit = lowerLimit,
                    UpperLimit = upperLimit
                };
                _deviceViewModel.InsertorUpdateThreshold(Threshold);
                
                OnPropertyChanged(nameof(Threshold));
            }
        }
        private void ExportData()
        {
            // Move export logic here from code-behind
            if (StartDate == null || EndDate == null)
            {
                // Handle error
                return;
            }

           // var exportData = GetDataInRange(StartDate.Value, EndDate.Value);
           // CsvExporter.ExportToCsv(exportData, $"SensorDataExport_{DateTime.Now:yyyyMMddHHmmss}.csv");
        }

        private void ImportData(object parameter)
        {
            // Move import logic here
            var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var importedData = CsvImporter.LoadSensorDataFromCsv(openFileDialog.FileName);
                LoadPlaybackData(importedData);
            }
        }

        // Add these properties for date binding
        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }

        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged(nameof(EndDate));
            }
        }


      
        public void AddSensorData(SensorData data)
        {
            
            if (data == null) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                
                SensorDataList.Add(data);

                
                _runningData.Add(data);

               
                const int maxRunningDataPoints = 100;
                if (_runningData.Count > maxRunningDataPoints)
                {
                    _runningData.RemoveAt(0);
                }

               
                if (IsLiveMode)
                {
                    // Skip update if not at the latest data
                    //bool shouldUpdateVisibleData = (CurrentIndex + WindowSize >= SensorDataList.Count);
                    //int liveDataRange = Math.Max(1, (int)(_zoomLevel * 10)); // Limit live data points based on zoom level
                    
                    //if (shouldUpdateVisibleData)
                    //{
                    // Optimized: Directly append instead of full refresh if possible
                    if (_visibleSensorData.Count < WindowSize)
                        {
                            _visibleSensorData.Add(data);
                        }
                        else
                        {
                            // Shift window by 1 (faster than full rebuild)
                            _visibleSensorData.RemoveAt(0);
                            _visibleSensorData.Add(data);
                        }
                        OnPropertyChanged(nameof(VisibleSensorData));

                    //}
                    _currentIndex = Math.Max(0, _runningData.Count - (int)(_zoomLevel * 10));
                }

                // Notify changes (only if actually modified)
                OnPropertyChanged(nameof(RunningData));
            });
        }
      

        private void UpdateVisibleDataByIndex()
        {
            // Early exit if no data
            if (SensorDataList.Count == 0)
            {
                _visibleSensorData.Clear();
                OnPropertyChanged(nameof(VisibleSensorData));
                return;
            }

            // Clamp index to valid range
            CurrentIndex = Math.Clamp(CurrentIndex, 0, SensorDataList.Count - 1);

            // Calculate window bounds
            int endIndex = Math.Min(CurrentIndex + WindowSize, SensorDataList.Count);
            int itemsToTake = endIndex - CurrentIndex;

            // Optimize: Only update if the visible range actually changed
            if (_visibleSensorData.Count == itemsToTake &&
                itemsToTake > 0 &&
                ReferenceEquals(_visibleSensorData[0], SensorDataList[CurrentIndex]))
            {
                return; // Already showing correct data
            }

            // Efficient bulk update
            _visibleSensorData.Clear();
            for (int i = CurrentIndex; i < endIndex; i++)
            {
                _visibleSensorData.Add(SensorDataList[i]);
            }
            // Even faster for large WindowSize
           

            OnPropertyChanged(nameof(VisibleSensorData));
        }
        private void UpdateVisibleDataWithZoom()
        {
            if (!SensorDataList.Any())
            {
                Console.WriteLine("SensorDataList is empty. No data to display.");
                _visibleSensorData.Clear();
                return;
            }

            // Adjust the time range based on TimeScale
            var startTime = _currentEndTime - (_displayDuration / TimeScale);
            var filteredData = SensorDataList
                .Where(d => d.Timestamp >= startTime && d.Timestamp <= _currentEndTime)
                .ToList();

            _visibleSensorData.Clear();
            foreach (var data in filteredData)
                _visibleSensorData.Add(data);

            Console.WriteLine($"VisibleSensorData Count: {_visibleSensorData.Count}");
            OnPropertyChanged(nameof(VisibleSensorData));
        }
        public void ZoomIn()
        {
            ZoomLevel = Math.Min(10, ZoomLevel + 1); // Increment zoom level (max limit is 10)
            UpdateVisibleData();
        }

        public void ZoomOut()
        {
            ZoomLevel = Math.Max(1, ZoomLevel - 1); // Decrement zoom level (min limit is 1)
            UpdateVisibleData();
        }

     

        public void ScrollLeft()
        {
            if (_currentIndex - _windowSize >= 0) // Ensure we don't go out of bounds
            {
                IsLiveMode = false; // Disable live mode while scrolling
                _currentIndex -= _windowSize;
                //UpdateVisibleDataByIndex();
                if (_currentIndex - (int)(_zoomLevel * 10) >= 0) // Ensure scrolling doesn't exceed the start of the collection
                {
                    _currentIndex -= (int)(_zoomLevel * 10); // Move left by the zoom level range
                    UpdateVisibleData(); // Refresh data
                }
            }
        }

        

        public void ResumeLiveData()
        {
            IsLiveMode = true;

            
            _currentIndex = Math.Max(0, _runningData.Count - (int)(_zoomLevel * 10)); // Determine the starting point for live data
            UpdateVisibleData();
           
        }


        private void UpdateVisibleData()
        {
            _visibleSensorData.Clear();

            if (IsLiveMode)
            {
                // Show running data
                int liveDataRange = Math.Max(1, (int)(_zoomLevel * 10)); // Limit live data points based on zoom level

                var liveDataSubset = _runningData
                    .Skip(Math.Max(0, _runningData.Count - liveDataRange)) // Get the latest live data points
                    .Take(liveDataRange)
                    .ToList();

                foreach (var data in liveDataSubset)
                    _visibleSensorData.Add(data);

                Console.WriteLine($"Running data count after applying zoom: {_visibleSensorData.Count}");
            }
            else
            {
                // Show historical data based on scroll and zoom
                if (!SensorDataList.Any())
                {
                    Console.WriteLine("SensorDataList is empty. No data to display.");
                    return;
                }

                // Calculate scroll range
                int startIndex = Math.Max(0, _currentIndex);
                int visibleDataRange = Math.Max(1, (int)(_zoomLevel * 10)); // Apply zoom level to determine range size

                var historicalDataSubset = SensorDataList
                    .Skip(startIndex) // Apply scrolling logic
                    .Take(visibleDataRange) // Apply zoom logic
                    .ToList();

                foreach (var data in historicalDataSubset)
                    _visibleSensorData.Add(data);

                Console.WriteLine($"Historical data count after applying zoom and scroll: {_visibleSensorData.Count}");
            }

            OnPropertyChanged(nameof(VisibleSensorData));
        }

   
 
        public void AddDeviceInfo(DeviceInfo data)
        {
            DeviceInformation = $"Device information : ID {data.DeviceId}; Name {data.DeviceName}; IP Address {data.IPAddress}: Port Name {data.PortName}; BaudRate {data.baudRate}; DataBits {data.DataBits}; Parity {data.Parity}; StopBits {data.StopBits}";
        }
        public void LoadPlaybackData(List<SensorData> history)
        {
            _sensorDataList.Clear();
            _playbackData = history.OrderBy(d => d.Timestamp).ToList();
            _playbackIndex = 0;
            _isPlayingBack = true;
            _playbackTimer.Start();
        }

        public void StopPlayback()
        {
            _playbackTimer.Stop();
            _isPlayingBack = false;
        }

        //public void LoadPlaybackData(List<SensorData> history)
        //{
        //    _playbackController = new PlaybackController();
        //    _playbackController.LoadData(history);

        //    _playbackController.OnNewData += data =>
        //    {
        //        Application.Current.Dispatcher.Invoke(() =>
        //        {
        //            _sensorDataList.Add(data);
        //            UpdateVisibleData();
        //            OnPropertyChanged(nameof(MinValue));
        //            OnPropertyChanged(nameof(MaxValue));
        //            OnPropertyChanged(nameof(AvgValue));
        //        });
        //    };

        //    _playbackController.OnPlaybackComplete += () =>
        //    {
        //        Application.Current.Dispatcher.Invoke(() =>
        //        {
        //            IsPlaybackMode = false;
        //            OnPropertyChanged(nameof(IsPlaybackMode));
        //        });
        //    };
        //}
        //public void StartPlayback()
        //{
        //    _sensorDataList.Clear();
        //    _playbackController.StartPlayback(_playbackSpeed);
        //    IsPlaybackMode = true;
        //    OnPropertyChanged(nameof(IsPlaybackMode));
        //}

    
        public bool HasNewData()
        {
                    if(IsPlaybackMode)
                    {
                        return SensorDataList.Any();
                    }            
                return SensorDataList.Any() && SensorDataList.Max(d => d.Timestamp) > _currentEndTime;
        }

        private void PlaybackStep(object sender, EventArgs e)
        {
            if (_playbackIndex < _playbackData.Count)
            {
                _sensorDataList.Add(_playbackData[_playbackIndex]);
               
                _visibleSensorData = _sensorDataList;
                _playbackIndex++;
               //_currentEndTime = Convert.ToDateTime(PlaybackEndTime);
                OnPropertyChanged(nameof(MinValue));
                OnPropertyChanged(nameof(MaxValue));
                OnPropertyChanged(nameof(AvgValue));
               // UpdateVisibleData();
                
            }
            else
            {
                _playbackTimer.Stop();
                _isPlayingBack = false;
            }
        }

        public double MinValue => _sensorDataList.Any() ? _sensorDataList.Min(s => s.Value) : 0;
        public double MaxValue => _sensorDataList.Any() ? _sensorDataList.Max(s => s.Value) : 0;
        public double AvgValue => _sensorDataList.Any() ? _sensorDataList.Average(s => s.Value) : 0;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
