using System;
using System.Globalization;
using System.Windows.Data;
using Robo_Sense_Monitor.Model;

namespace Robo_Sense_Monitor.Converter
{
    public class ExportParamsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0]: StartDate (DateTime?)
            // values[1]: StartTime (DateTime?)
            // values[2]: EndDate (DateTime?)
            // values[3]: EndTime (DateTime?)
            // values[4]: SelectedDevice (DeviceInfo)
            return new ExportParameters
            {
                StartDateTime = CombineDateTime(values[0] as DateTime?, values[1] as DateTime?),
                EndDateTime = CombineDateTime(values[2] as DateTime?, values[3] as DateTime?),
                Device = values[4] as DeviceInfo
            };
        }

        private DateTime? CombineDateTime(DateTime? date, DateTime? time)
        {
            if (!date.HasValue || !time.HasValue) return null;
            return date.Value.Date + time.Value.TimeOfDay;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ExportParameters
    {
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public DeviceInfo Device { get; set; }
    }
}