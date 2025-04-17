using Robo_Sense_Monitor.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robo_Sense_Monitor
{
    public static class CsvImporter
    {
        public  static List<SensorData> LoadSensorDataFromCsv(string filePath)
        {
            var sensorDataList = new List<SensorData>();

            using (var reader = new StreamReader(filePath))
            {
                string line;

                // Skip the header row if it exists
                line = reader.ReadLine();


                while ((line = reader.ReadLine()) != null)
                {
                    var values = line.Split(',');

                    var sensorData = new SensorData
                    {
                        Id = int.Parse(values[0]),
                        // Timestamp = Convert.ToDateTime( values[1]),
                        Timestamp = DateTime.ParseExact(values[1], "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture),
                        DeviceId = int.Parse(values[2]),
                        Value = double.Parse(values[3])
                    };

                    sensorDataList.Add(sensorData);
                }
            }

            return sensorDataList;
        }
    }
}
