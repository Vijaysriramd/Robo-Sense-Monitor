using Robo_Sense_Monitor.Communicators;
using Robo_Sense_Monitor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robo_Sense_Monitor
{
    public static class CommunicatorFactory
    {
        public static IDeviceCommunicator Create(DeviceInfo deviceInfo)
        {
            return deviceInfo.Type switch
            {
                "Serial" => new SerialCommunicator(deviceInfo),
                "TCP" => new TcpCommunicator(),
                "GRPC" => new GrpcCommunicator(),
                _ => throw new NotSupportedException("Unsupported device type")
            };
        }
    }

}
