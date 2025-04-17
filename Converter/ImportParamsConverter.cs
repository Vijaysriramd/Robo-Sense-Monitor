using System;
using System.Globalization;
using System.Windows.Data;
using Robo_Sense_Monitor.Model;

namespace Robo_Sense_Monitor.Converter
{
    public class ImportParamsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // First value: DeviceId (int)
            // Second value: IsPlaybackMode (bool)
            return new ImportParameters
            {
                DeviceId = values[0] is int id ? id : 0,
                IsPlayback = values[1] is bool playback && playback
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ImportParameters
    {
        public int DeviceId { get; set; }
        public bool IsPlayback { get; set; }
    }
}