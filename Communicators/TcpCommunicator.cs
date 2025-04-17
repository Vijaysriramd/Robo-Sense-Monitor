using Robo_Sense_Monitor.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Robo_Sense_Monitor.Communicators
{
    public class TcpCommunicator : IDeviceCommunicator
    {
        public event Action<SensorData> DataReceived;
        private TcpClient _client;

        public void Connect(DeviceInfo deviceInfo)
        {
            _client = new TcpClient();
            _client.Connect(deviceInfo.IPAddress, Int32.Parse(deviceInfo.PortName) );
           // await StartListening();
        }

        public async Task StartListening(Action<string> onDataReceived, CancellationToken token)
        {
            using var stream = _client.GetStream();
            using var reader = new StreamReader(stream);
            while (true)
            {
                var line = await reader.ReadLineAsync();
                if (line != null)
                {
                    var data = ParseData(line);
                    DataReceived?.Invoke(data);
                }
            }
        }

        public void Disconnect() => _client?.Close();

        private SensorData ParseData(string raw)
        {
            return new SensorData { Value = double.Parse(raw), Timestamp = DateTime.Now };
        }
    }

}
