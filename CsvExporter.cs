using Robo_Sense_Monitor.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Robo_Sense_Monitor
{
    public static class CsvExporter
    {
        public static void ExportToCsv(List<SensorData> data, string filePath)
        {
            var lines = new List<string>
        {
            "ID,Timestamp,DeviceId,Value" // CSV header
        };

            lines.AddRange(data.Select(d => $"{d.Id},{d.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff")},{d.DeviceId},{d.Value}"));

            File.WriteAllLines(filePath, lines);
            MessageBox.Show(filePath + " has been generated");
        }
    }
}
