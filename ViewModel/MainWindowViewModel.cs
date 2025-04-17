using Robo_Sense_Monitor.Commands;
using Robo_Sense_Monitor.Model;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace Robo_Sense_Monitor.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        string fileTransferType = "";
        //DataModel dataCollection = new DataModel();

        private ObservableCollection<DeviceInfo> _dataList;
        public ObservableCollection<DeviceInfo> DataList
        {
            get => _dataList;
            set
            {
                _dataList = value;
                OnPropertyChanged(); // Critical for collection replacement
                OnPropertyChanged("IsListening");
            }
        }
        private ThresholdStats _currentThresholdStats;
        public ThresholdStats CurrentThresholdStats
        {
            get => _currentThresholdStats;
            set => SetProperty(ref _currentThresholdStats, value);
        }

        private bool _isThresholdPopupVisible;
        public bool IsThresholdPopupVisible
        {
            get => _isThresholdPopupVisible;
            set => SetProperty(ref _isThresholdPopupVisible, value);
        }


        // private SensorDataViewModel _viewMode ;
        List<DeviceInfo> Devices;
        XmlDocument xml;
        private DeviceViewModel _deviceViewModel = new DeviceViewModel();
        private DataCollectorService dataservice= new DataCollectorService();
        // Commands
        public ICommand LoadMenuCommand { get; }
        public ICommand AddRowCommand { get; }
        public ICommand StartMonitorCommand { get; }
        public ICommand EndMonitorCommand { get; }
        public ICommand CheckConnectivityCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand InfoCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand LoadFromDbCommand { get; }
        public ICommand SaveToDbCommand { get; }
        public ICommand LoadConfigCommand { get; }
        public ICommand SaveConfigCommand { get; }
        public ICommand ShowPlotsCommand { get; }
        public ICommand PlaybackCommand { get; }

        public ICommand ShowAllPlotCommand { get; }

        public ICommand CloseThresholdPopupCommand => new RelayCommand(() =>
        {
            IsThresholdPopupVisible = false;
        });

        public MainWindowViewModel()
        {
            // Initialize commands
            DataList = new ObservableCollection<DeviceInfo>();
            LoadMenuCommand = new RelayCommand<object>(ExecuteLoadMenu);
            AddRowCommand = new RelayCommand<object>(ExecuteAddRow);
            StartMonitorCommand = new RelayCommand<object>(ExecuteStartMonitor, CanExecuteStartMonitor);
            EndMonitorCommand = new RelayCommand<object>(ExecuteEndMonitor, CanExecuteEndMonitor);
            CheckConnectivityCommand = new RelayCommand<object>(ExecuteCheckConnectivity);
            DeleteCommand = new RelayCommand<object>(ExecuteDelete, CanExecuteDelete);
            InfoCommand = new RelayCommand<object>(ExecuteInfo);
            ResetCommand = new RelayCommand<object>(ExecuteReset);
            LoadFromDbCommand = new RelayCommand<object>(ExecuteLoadFromDb);
            SaveToDbCommand = new RelayCommand<object>(ExecuteSaveToDb);
            LoadConfigCommand = new RelayCommand<object>(ExecuteLoadConfig);
            SaveConfigCommand = new RelayCommand<object>(ExecuteSaveConfig);
            ShowPlotsCommand = new RelayCommand<object>(ExecuteShowPlots, CanExecuteShowPlots);
            PlaybackCommand = new RelayCommand<object>(ExecutePlayback, CanExecutePlayback);
            ShowAllPlotCommand = new RelayCommand<object>(ExecuteShowAllPlot, CanExecuteShowAllPlot);
        }
       
        //public DataModel DataCollection { get; set; } = new DataModel();



        private void ExecuteLoadMenu(object parameter)
        {
            // Implementation from LoadMenu_Click
        }

        private void ExecuteAddRow(object parameter)
        {
            DataList.Add(new DeviceInfo
            {
                DeviceId = (DataList.Count + 1),
                DeviceName = "",
                IPAddress = "",
                PortName = "COM3",
                Type = "Serial"
            });
        }

        [STAThread]
        private void ExecuteStartMonitor(object parameter)
        {
            
            foreach (var item in DataList) { if (item.IsSelected) { item.IsListening = true; } }
            dataservice = new DataCollectorService();
            dataservice.StartMonitoringDevices(DataList);
            Console.WriteLine("UI instances launched!");
        }

        private void ExecuteEndMonitor(object parameter)
        {

          
            dataservice = new DataCollectorService();
            dataservice.StopMonitoring();// (DataList);
            foreach (var item in DataList) { if (item.IsSelected) { item.IsListening = false; } }
            Console.WriteLine("UI instances launched!");
        }
        private bool CanExecuteStartMonitor(object parameter)
        {
            //return DataList.Any(dev => dev.IsSelected);
            return true;
        }
        [STAThread]
        private void ExecuteShowAllPlot(object parameter)
        {
            //List<Thread> uiThreads = new List<Thread>();

            //foreach (var dev in DataList)
            //{
            //    if (dev.IsSelected)
            //    {
            //        Thread uiThread = new Thread(() =>
            //        {
            //            dataservice = new DataCollectorService();

            //            dataservice.StartMonitoringDevices(dev);
            //            System.Windows.Threading.Dispatcher.Run();
            //        });

            //        uiThread.SetApartmentState(ApartmentState.STA);
            //        uiThreads.Add(uiThread);
            //        uiThread.Start();
            //    }
            //}
                List<DeviceInfo> IEdevices = new List<DeviceInfo>();
                foreach (var item in DataList)
                {
                    if (item.IsSelected && item.IsListening)
                    {


                        IEdevices.Add(item);


                    }
                }
                if (IEdevices != null && IEdevices.Count > 0)
                {
                    DeviceGraph objgrp = new DeviceGraph(IEdevices, dataservice, false);
                    objgrp.Show();
                }

            
            //dataservice = new DataCollectorService();
            //dataservice.StartMonitoringDevices(DataList);
            //Console.WriteLine("UI instances launched!");
        }
        private bool CanExecuteShowAllPlot(object parameter)
        {
            //return DataList.Any(dev => dev.IsSelected);
            return true;
        }
        public bool CanExecuteEndMonitor(object parameter)
        {
            return true;
        }
        private void ExecuteCheckConnectivity(object parameter)
        {
            //Load_Deviceinfo();
            //Refresh_DataList();
            //if ((bool)rBtnParallel.IsChecked)
            //{
            //    Task.WaitAll(Task.Run(async () => await ParallelCheckConnectivityAsync()));
            //}
            //else
            //{
            //    Thread threadCon = new Thread(() =>
            //    {
            //        while (true)
            //        {
            //            try
            //            {
            //                Task.WaitAll(Task.Run(async () => { await CheckConnectivityAsync(); }));
            //            }
            //            catch (Exception ex) { }
            //        }
            //    });

            //    threadCon.IsBackground = true;
            //    threadCon.Start();
            //}
        }

        private void ExecuteDelete(object parameter)
        {

            if (parameter is DeviceInfo device)
            {

                //int index = maingrid.SelectedIndex;
                DataList.Remove(device);
                UpdateRowNumber();
            }
        }

        private bool CanExecuteDelete(object parameter)
        {
            if (parameter is DeviceInfo device)
            {
                return DataList.Contains(device);
            }
            return false;
        }

        private async void ExecuteInfo(object parameter)
        {
            // Implementation from BtnInfo_Click
            if (parameter is DeviceInfo device)
            {
                if (device == null) return;

                try
                {
                    CurrentThresholdStats = await _deviceViewModel.GetTodayThresholdStatsAsync(device.DeviceId);
                    IsThresholdPopupVisible = true;
                }
                catch (Exception ex)
                {
                    // Handle error
                    Debug.WriteLine($"Error loading threshold stats: {ex.Message}");
                }
            }
        }

        private void ExecuteReset(object parameter)
        {
                DataList.Clear();
            //rBtnSequential.IsChecked = true;
            DataList.Clear();
            //maingrid.Items.Refresh();
        }

        private void ExecuteLoadFromDb(object parameter)
        {
            List<DeviceInfo> list = _deviceViewModel.FetchDeviceInfo();
            if ((DataList.Count > 0) && DataList.Count >= list.Count)
            {
                int i = 0;
                foreach (var devicelst in list)
                {
                    var dataRow = DataList[i];
                    dataRow.DeviceId = Convert.ToInt16(devicelst.DeviceId);
                    dataRow.DeviceName = devicelst.DeviceName;
                    dataRow.IPAddress = devicelst.IPAddress;
                    dataRow.PortName = devicelst.PortName;
                    dataRow.baudRate = devicelst.baudRate;
                    dataRow.DataBits = devicelst.DataBits;
                    dataRow.Parity = devicelst.Parity;
                    dataRow.StopBits = devicelst.StopBits;
                    i++;
                }
               // maingrid.Items.Refresh();
            }
            else
            {
                DataList.Clear();
                foreach (var devicelst in list)
                {
                    DataList.Add(new DeviceInfo
                    {
                        DeviceId = devicelst.DeviceId,
                        DeviceName = devicelst.DeviceName,
                        IPAddress = devicelst.IPAddress,
                        PortName = devicelst.PortName,
                        baudRate = devicelst.baudRate,
                        DataBits = devicelst.DataBits,
                        Parity = devicelst.Parity,
                        StopBits = devicelst.StopBits,
                        Type = devicelst.Type
                    });
                }
            }
        }

        private void ExecuteSaveToDb(object parameter)
        {
            _deviceViewModel.InsertorUpdateDeviceInfo(DataList.ToList());
        }

        private void ExecuteLoadConfig(object parameter)
        {
            ExtractXmlData();
        }

        private void ExecuteSaveConfig(object parameter)
        {
            SaveConfig("Config.xml");
        }

        private void ExecuteShowPlots(object parameter)
        {
            if (parameter is DeviceInfo device)                
            {
                List<DeviceInfo> IEdevices =new List<DeviceInfo>();
                IEdevices.Add(device);

                DeviceGraph objgrp = new DeviceGraph(IEdevices, dataservice,false);

                objgrp.Show();


                //int index = maingrid.SelectedIndex;

            }
            //if (Datarow.objplot != null)
            //{
            //    Datarow.objplot.WindowState = WindowState.Normal;
            //    Datarow.objplot.Activate();
            //}
        }

        private bool CanExecuteShowPlots(object parameter)
        {
            return true;
            //return maingrid.SelectedIndex >= 0 &&
            //       //dataCollection.DataList[maingrid.SelectedIndex].objplot != null;
        }

        private void ExecutePlayback(object parameter)
        {
            //int index = maingrid.SelectedIndex;
            //var Datarow = dataCollection.DataList[index];
            //Datarow.Type = "Serial";
            //WcSendorPloting objplots = new WcSendorPloting();
            //objplots.Isplayback = true;
            //objplots.DeviceDetails = Datarow;
            //objplots.setplayback();
            //objplots.Title = Datarow.DeviceName;
            //objplots.Show();

            if (parameter is DeviceInfo device)
            {
                List<DeviceInfo> IEdevices = new List<DeviceInfo>();
                IEdevices.Add(device);

                DeviceGraph objgrp = new DeviceGraph(IEdevices, dataservice,true);

                objgrp.Show();


                //int index = maingrid.SelectedIndex;

            }
        }

        private bool CanExecutePlayback(object parameter)
        {
            if (parameter is DeviceInfo device)
            {
                return DataList.Contains(device);
            }
            return false;
        }



        private async Task ParallelCheckConnectivityAsync()
        {
            // Implementation remains the same
        }

        void UpdateRowNumber()
        {
            //int rowCount = maingrid.Items.Count;
            //for (int count = 0; count < maingrid.Items.Count; count++)
            //{
            //    var Datarow = dataCollection.DataList[count];
            //    Datarow.DeviceId = count + 1;
            //}
            //maingrid.Items.Refresh();
        }

        private void Load_Deviceinfo()
        {
            Devices = new List<DeviceInfo>();
            foreach (var dr in DataList)
            {
                DeviceInfo H = new DeviceInfo();
                H.PortName = dr.PortName;
                H.IPAddress = dr.IPAddress;
                H.DeviceId = dr.DeviceId;
                H.baudRate = dr.baudRate;
                H.DataBits = dr.DataBits;
                H.StopBits = dr.StopBits;
                Devices.Add(H);
            }
        }

        private void Refresh_DataList()
        {
            foreach (var dr in Devices)
            {
                foreach (var dsdr in DataList.Where(w => w.IPAddress == dr.IPAddress))
                {
                    // Implementation remains the same
                }
            }
        }

        private async Task CheckConnectivityAsync()
        {
            foreach (var h in Devices)
            {
                //h.Type = "Serial";
                //var communicator = CommunicatorFactory.Create(h);
                //communicator.DataReceived += OnDataReceived;
                //communicator.Connect(h);
            }
        }

        //private void SaveConfig()
        //{
        //    string filename = "Config.xml";
        //    fileTransferType =  "Parallel";
        //    XmlTextWriter writer = new XmlTextWriter(filename, Encoding.UTF8);
        //    writer.Formatting = Formatting.Indented;
        //    writer.WriteStartDocument();
        //    writer.WriteStartElement("SerialSensor");

        //    writer.WriteElementString("FileTransferType", fileTransferType);

        //    int end = DataList.Count;
        //    for (int i = 0; i <= end - 1; i++)
        //    {
        //        var datarow = DataList[i];
        //        writer.WriteStartElement("Device");
        //        writer.WriteElementString("DeviceID", datarow.DeviceId.ToString());
        //        writer.WriteElementString("DeviceName", datarow.DeviceName.ToString());
        //        writer.WriteElementString("IPAddress", datarow.IPAddress.ToString());
        //        writer.WriteElementString("PortName", datarow.PortName.ToString());
        //        writer.WriteElementString("baudRate", datarow.baudRate.ToString());
        //        writer.WriteElementString("DataBits", datarow.DataBits.ToString());
        //        writer.WriteElementString("Parity", datarow.Parity.ToString());
        //        writer.WriteElementString("StopBits", datarow.StopBits.ToString());
        //        writer.WriteEndElement();
        //    }

        //    writer.WriteEndElement();
        //    writer.WriteEndDocument();
        //    writer.Flush();
        //    writer.Close();
        //    System.Windows.MessageBox.Show("Data saved sucessfully", "Sensor Config", MessageBoxButton.OK, MessageBoxImage.Information);
        //}

        public void SaveConfig(string filePath)
        {
            using (var writer = XmlWriter.Create(filePath, new XmlWriterSettings { Indent = true }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Devices");

                foreach (var device in DataList)
                {
                    writer.WriteStartElement("Device");

                    WriteElementSafe(writer, "DeviceID", device.DeviceId);
                    WriteElementSafe(writer, "DeviceName", device.DeviceName);
                    WriteElementSafe(writer, "IPAddress", device.IPAddress);
                    WriteElementSafe(writer, "PortName", device.PortName);
                    WriteElementSafe(writer, "baudRate", device.baudRate);
                    WriteElementSafe(writer, "DataBits", device.DataBits);
                    WriteElementSafe(writer, "Parity", device.Parity);
                    WriteElementSafe(writer, "StopBits", device.StopBits);

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        // Overloaded helper for different types
        private void WriteElementSafe(XmlWriter writer, string name, int? value) =>
            writer.WriteElementString(name, (value ?? 0).ToString());

        private void WriteElementSafe(XmlWriter writer, string name, string value) =>
            writer.WriteElementString(name, value ?? string.Empty);
        void ExtractXmlData()
        {
            xml = new XmlDocument();
            xml.Load("Config.xml");

            XmlNodeList FileTransferTypeNode = xml.DocumentElement.SelectNodes("FileTransferType");
       

            XmlNodeList nodes = xml.DocumentElement.SelectNodes("Device");
            ConvertXmlData(nodes);
        }

        void ConvertXmlData(XmlNodeList list)
        {
            if (list == null || list.Count == 0) return;

            try
            {
                if (DataList.Count > 0 && DataList.Count >= list.Count)
                {
                    // Update existing items
                    for (int i = 0; i < list.Count; i++)
                    {
                        var node = list.Item(i);
                        if (node == null) continue;

                        var dataRow = DataList[i];
                        dataRow.DeviceId = SafeConvertToInt16(node.SelectSingleNode("DeviceID")?.InnerText, 1);
                        dataRow.DeviceName = SafeGetString(node.SelectSingleNode("DeviceName"));
                        dataRow.IPAddress = SafeGetString(node.SelectSingleNode("IPAddress"), "0.0.0.0");
                        dataRow.PortName = SafeGetString(node.SelectSingleNode("PortName"), "COM1");
                        dataRow.baudRate = SafeConvertToInt16(node.SelectSingleNode("baudRate")?.InnerText, 9600);
                        dataRow.DataBits = SafeConvertToInt16(node.SelectSingleNode("DataBits")?.InnerText, 8);
                        dataRow.Parity = SafeConvertToInt16(node.SelectSingleNode("Parity")?.InnerText, 0);
                        dataRow.StopBits = SafeConvertToInt16(node.SelectSingleNode("StopBits")?.InnerText, 1);
                        dataRow.Type = "Serial";
                    }
                }
                else
                {
                    // Create new items
                    DataList.Clear();
                    for (int row = 0; row < list.Count; row++)
                    {
                        var node = list.Item(row);
                        if (node == null) continue;

                        DataList.Add(new DeviceInfo
                        {
                            DeviceId = SafeConvertToInt16(node.SelectSingleNode("DeviceID")?.InnerText, row + 1),
                            DeviceName = SafeGetString(node.SelectSingleNode("DeviceName")),
                            IPAddress = SafeGetString(node.SelectSingleNode("IPAddress"), "0.0.0.0"),
                            PortName = SafeGetString(node.SelectSingleNode("PortName"), "COM1"),
                            baudRate = SafeConvertToInt16(node.SelectSingleNode("baudRate")?.InnerText, 9600),
                            DataBits = SafeConvertToInt16(node.SelectSingleNode("DataBits")?.InnerText, 8),
                            Parity = SafeConvertToInt16(node.SelectSingleNode("Parity")?.InnerText, 0),
                            StopBits = SafeConvertToInt16(node.SelectSingleNode("StopBits")?.InnerText, 1),
                            Type = "Serial"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error or show message
                Debug.WriteLine($"Error converting XML data: {ex.Message}");
                // Consider partial loading or recovery strategy
            }
        }

        // Helper methods for safe conversion
        private string SafeGetString(XmlNode node, string defaultValue = "")
        {
            return node?.InnerText?.Trim() ?? defaultValue;
        }

        private int SafeConvertToInt16(string value, int defaultValue)
        {
            if (short.TryParse(value, out Int16 result))
                return result;
            return defaultValue;
        }
        //void ConvertXmlData(XmlNodeList list)
        //{
        //    if ((DataList.Count > 0) && DataList.Count >= list.Count)
        //    {
        //        for (int i = 0; i < list.Count; i++)
        //        {
        //            var dataRow = DataList[i];
        //            dataRow.DeviceId = Convert.ToInt16(list.Item(i).SelectSingleNode("DeviceID").InnerText);
        //            dataRow.DeviceName = (list.Item(i).SelectSingleNode("DeviceName").InnerText);
        //            dataRow.IPAddress = (list.Item(i).SelectSingleNode("IPAddress").InnerText);
        //            dataRow.PortName = (list.Item(i).SelectSingleNode("PortName").InnerText);
        //            dataRow.baudRate = Convert.ToInt16(list.Item(i).SelectSingleNode("baudRate").InnerText);
        //            dataRow.DataBits = Convert.ToInt16(list.Item(i).SelectSingleNode("DataBits").InnerText);
        //            dataRow.Parity = Convert.ToInt16(list.Item(i).SelectSingleNode("Parity").InnerText);
        //            dataRow.StopBits = Convert.ToInt16(list.Item(i).SelectSingleNode("StopBits").InnerText);

        //        }
        //        //maingrid.Items.Refresh();
        //    }
        //    else
        //    {
        //        DataList.Clear();
        //        for (int row = 0; row < list.Count; row++)
        //        {
        //            DataList.Add(new DeviceInfo
        //            {
        //                DeviceId = row + 1,
        //                DeviceName = list.Item(row).SelectSingleNode("DeviceName").InnerText,
        //                IPAddress = list.Item(row).SelectSingleNode("IPAddress").InnerText,
        //                PortName = list.Item(row).SelectSingleNode("PortName").InnerText,
        //                baudRate = Convert.ToInt16(list.Item(row).SelectSingleNode("baudRate").InnerText),
        //                DataBits = Convert.ToInt16(list.Item(row).SelectSingleNode("DataBits").InnerText),
        //                Parity = Convert.ToInt16(list.Item(row).SelectSingleNode("Parity").InnerText),
        //                StopBits = Convert.ToInt16(list.Item(row).SelectSingleNode("StopBits").InnerText)
        //            });
        //        }
        //    }
        //}
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
    //public class DataModel : INotifyPropertyChanged
    //{
    //    private ObservableCollection<DeviceInfo> _dataList;
    //    public ObservableCollection<DeviceInfo> DataList
    //    {
    //        get => _dataList;
    //        set
    //        {
    //            _dataList = value;
    //            OnPropertyChanged(); // Critical for collection replacement
    //        }
    //    }

    //    public DataModel()
    //    {
    //        DataList = new ObservableCollection<DeviceInfo>();
    //    }
    //    public event PropertyChangedEventHandler PropertyChanged;
    //    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }
    //}
}
