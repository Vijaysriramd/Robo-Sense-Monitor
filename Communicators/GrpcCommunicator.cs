using Robo_Sense_Monitor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robo_Sense_Monitor.Communicators
{
    public class GrpcCommunicator : IDeviceCommunicator
    {
        public event Action<SensorData> DataReceived;

        public void Connect(DeviceInfo deviceInfo)
        {
            //  Set up gRPC client
        }

        public async Task StartListening(Action<string> onDataReceived, CancellationToken token)
        {
            //  gRPC Streaming logic
        }

        public void Disconnect()
        {
            //  Disconnect gRPC
        }
    }

}
