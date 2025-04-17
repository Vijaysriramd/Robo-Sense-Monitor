using Robo_Sense_Monitor.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace   Robo_Sense_Monitor.Repository
{
    internal class RepositoryRecorder
    {
        private readonly string _connectionString;

        public RepositoryRecorder(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<ThresholdStats> GetTodayThresholdStatsAsync(int deviceId)
        {
            const string query = @"DECLARE @TodayStart datetime = CAST(GETDATE() AS date)
            DECLARE @TodayEnd datetime = DATEADD(day, 1, @TodayStart)			
			SELECT
    COUNT(*) AS TotalReadings,
    ROUND(100.0 * COUNT(CASE WHEN Value > 50 THEN 1 END) / NULLIF(COUNT(*), 0), 2) AS AbovePercentage,
    ROUND(100.0 * COUNT(CASE WHEN Value BETWEEN 20 AND 50 THEN 1 END) / NULLIF(COUNT(*), 0), 2) AS NormalPercentage,
    ROUND(100.0 * COUNT(CASE WHEN Value < 20 THEN 1 END) / NULLIF(COUNT(*), 0), 2) AS BelowPercentage
FROM SensorData  sd
LEFT JOIN DeviceInfo d ON sd.DeviceId = d.Device_ID 
LEFT JOIN Thresholds tr ON d.Device_ID = tr.Device_ID
            WHERE sd.DeviceId = @DeviceId
            AND sd.Timestamp >= @TodayStart
            AND sd.Timestamp < @TodayEnd";

            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryFirstOrDefaultAsync<ThresholdStats>(query, new { DeviceId = deviceId });
            }
        }

       
        
        public void SaveSensorDataToDatabase(SensorData data)
        {
            string query = "INSERT INTO SensorData (Value, Timestamp,Deviceid) VALUES (@Value, @Timestamp,@Deviceid)";
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    if (connection.State == System.Data.ConnectionState.Closed) connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Value", data.Value );
                        command.Parameters.AddWithValue("@Timestamp", data.Timestamp);
                        command.Parameters.AddWithValue("@Deviceid", data.DeviceId);

                        command.ExecuteNonQuery();
                    }
                }
                Console.WriteLine($"Data saved: {data.Value} at {data.Timestamp}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occured DB Connection : {ex.Message} ");
            }
        }
        public async Task<List<SensorData>> GetOptimizedPlaybackDataAsync(int deviceId, DateTime start, DateTime end)
        {
            const int batchSize = 5000;
            var results = new List<SensorData>(batchSize);

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Use stored procedure with optimized query plan
                    var command = new SqlCommand("sp_GetSensorDataForPlayback", connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 300 // 5 minutes
                    };

                    command.Parameters.AddWithValue("@Device_id", deviceId);
                    command.Parameters.AddWithValue("@StartDateTime", start);
                    command.Parameters.AddWithValue("@EndDateTime", end);

                    using (var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        while (await reader.ReadAsync())
                        {
                            results.Add(new SensorData
                            {
                                Id = reader.GetInt32(0),
                                Timestamp = reader.GetDateTime(1),
                                DeviceId = reader.GetInt32(2),
                                Value = reader.GetDouble(3)
                            });

                            if (results.Count >= batchSize)
                            {
                                // Process batch if needed
                                results.Clear();
                            }
                        }
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                // Log error and consider returning partial results
                Debug.WriteLine($"Error loading playback data: {ex.Message}");
                return results; // Return whatever we got before the error
            }
        }
        public List<SensorData> LoadPlaybackDataFromDatabase(string deviceId)
        {
            var history = new List<SensorData>();
            string query = "SELECT Id, Timestamp, DeviceId, Value FROM SensorData WHERE DeviceId = @DeviceId ORDER BY Timestamp";
            using (SqlConnection _connection = new SqlConnection(_connectionString))
            {
                if (_connection.State == System.Data.ConnectionState.Closed) _connection.Open();
                using SqlCommand command = new SqlCommand(query, _connection);
                command.Parameters.AddWithValue("@DeviceId", deviceId);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    history.Add(new SensorData
                    {
                        Id = reader.GetInt32(0),
                        Timestamp = reader.GetDateTime(1),
                        DeviceId = reader.GetInt32(2),
                        Value = reader.GetDouble(3)
                    });
                }
            }
            return history;
        }
        public bool InsertorUpdateThreshold(Threshold tr)
        {
            int deviceid = Convert.ToInt16(tr.DeviceId);


            using (SqlConnection _connection = new SqlConnection(_connectionString))
            {
                if (_connection.State == System.Data.ConnectionState.Closed) _connection.Open();
                using (SqlCommand command = new SqlCommand("Sp_InsertorUpdateThreshold", _connection))
                {
                   command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Device_id", tr.DeviceId));
                    command.Parameters.Add(new SqlParameter("@LowerLimit", tr.LowerLimit));
                    command.Parameters.Add(new SqlParameter("@UpperLimit", tr.UpperLimit));
                    Int32 rdr = command.ExecuteNonQuery();
                    if (rdr > 0) return true;

                }
                                   
                }
            return false;
        }

        public bool InsertorUpdateDeviceInfo(List<DeviceInfo> devices)
        {
           int updaterecordcount = 0;

            using (SqlConnection _connection = new SqlConnection(_connectionString))
            {
                if (_connection.State == System.Data.ConnectionState.Closed) _connection.Open();
                foreach (var device in devices)
                {
                    using (SqlCommand command = new SqlCommand("Sp_InsertorUpdateDeviceInfo", _connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        // Add parameters corresponding to the stored procedure
                        command.Parameters.AddWithValue("@Device_ID", device.DeviceId);
                        command.Parameters.AddWithValue("@DeviceName", device.DeviceName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@DeviceDesc", device.DeviceName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Type", "Serial");
                        command.Parameters.AddWithValue("@IPAddress", device.IPAddress ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@PortName", device.PortName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@baudRate", device.baudRate);
                        command.Parameters.AddWithValue("@DataBits", device.DataBits);
                        command.Parameters.AddWithValue("@StopBits", device.StopBits);
                        command.Parameters.AddWithValue("@Parity", device.Parity);

                        // Execute the stored procedure
                        Int32 rdr = command.ExecuteNonQuery();
                        if (rdr > 0) updaterecordcount++;

                    }
                }

            }
            if (updaterecordcount >  0)
            {
                return true;
            }
            else
            {
                return false;
            }
                
        }



        public List<Threshold> FetchThresholdSetup(int _deviceid)
        {
            var results = new List<Threshold>();
            string query = "select Device_ID,LowerLimit,UpperLimit from Thresholds WHERE Device_ID=@DeviceId";
            using (SqlConnection _connection = new SqlConnection(_connectionString))
            {
                if (_connection.State == System.Data.ConnectionState.Closed) _connection.Open();
                using (SqlCommand command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@DeviceId", _deviceid);
                   

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(new Threshold
                            {
                                DeviceId = reader.GetInt32(0),
                                LowerLimit = reader.GetInt32(1),
                                UpperLimit= reader.GetInt32(2)
                            });
                        }
                    }
                }
            }

            return results;

        }

        public List<DeviceInfo> FetchDeviceInfo()
        {
            var results = new List<DeviceInfo>();
            string query = "SELECT Device_ID, DeviceName, Type,IPAddress,PortName,baudRate,DataBits,StopBits,Parity FROM DeviceInfo ";
            using (SqlConnection _connection = new SqlConnection(_connectionString))
            {
                if (_connection.State == System.Data.ConnectionState.Closed) _connection.Open();
                using (SqlCommand command = new SqlCommand(query, _connection))
                {

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(new DeviceInfo
                            {
                                DeviceId = reader.GetInt32(0),
                                DeviceName = !reader.IsDBNull(1) ? reader.GetString(1) : null,
                                Type       = "Serial",
                                IPAddress = !reader.IsDBNull(3) ? reader.GetString(3) : null,
                                PortName = !reader.IsDBNull(4) ? reader.GetString(4) : null,
                                baudRate = !reader.IsDBNull(5) ? reader.GetInt32(5) : default,
                                DataBits = !reader.IsDBNull(6) ? reader.GetInt32(6) : default,
                                StopBits = !reader.IsDBNull(7) ? reader.GetInt32(7) : default,
                                Parity = !reader.IsDBNull(8) ? reader.GetInt32(8) : default
                            });
                        }
                    }
                }
            }

            return results;
        }


        public List<SensorData> FetchSensorData(int _deviceid,DateTime start, DateTime end)
        {
            var results = new List<SensorData>();
            string query = "SELECT Id, Timestamp, DeviceId, Value FROM SensorData WHERE DeviceId=@DeviceId and Timestamp BETWEEN @Start AND @End ORDER BY Timestamp";
            using (SqlConnection _connection = new SqlConnection(_connectionString))
            {
                if (_connection.State == System.Data.ConnectionState.Closed) _connection.Open();
                using (SqlCommand command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@DeviceId", _deviceid);
                    command.Parameters.AddWithValue("@Start", start);
                    command.Parameters.AddWithValue("@End", end);
                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(new SensorData
                            {
                                Id = reader.GetInt32(0),
                                Timestamp = reader.GetDateTime(1),
                                DeviceId = reader.GetInt32(2),
                                Value = reader.GetDouble(3)
                            });
                        }
                    }
                }
            }

            return results;
        }
    }

}
