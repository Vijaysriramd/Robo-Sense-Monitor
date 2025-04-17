using Robo_Sense_Monitor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robo_Sense_Monitor
{
  
    public class DeviceManager
    {
        public event Action<SensorData> DataReceived;

        public void Connect(DeviceInfo deviceInfo) { /* Serial/IP logic */ }
        public void Disconnect() { /* Cleanup */ }
        public void StartListening() { /* Read loop and raise DataReceived */ }
    }

}
