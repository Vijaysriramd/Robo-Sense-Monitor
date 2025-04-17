using Robo_Sense_Monitor.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Robo_Sense_Monitor.Converter {

    public class BoolToPlayPauseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool playing && playing) ? "Pause" : "Play";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
//{
//    public class ImportParameters
//    {
//        public int DeviceId { get; set; }
//        public bool IsPlayback { get; set; }
//    }

//    public class ExportParameters
//    {
//        public DateTime? StartDateTime { get; set; }
//        public DateTime? EndDateTime { get; set; }
//        public DeviceInfo Device { get; set; }
//    }

//    public class PlaybackParameters
//    {
//        public DateTime? StartDateTime { get; set; }
//        public DateTime? EndDateTime { get; set; }
//    }
//    public class ImportParamsConverter : IMultiValueConverter
//    {
       
//        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
//        {
//            return new ImportParameters
//            {
//                DeviceId = values[0] is int id ? id : 0,
//                IsPlayback = values[1] is bool playback && playback
//            };
//        }

//        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public class ExportParamsConverter : IMultiValueConverter
//    {
//        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
//        {
//            DateTime? startDate = values[0] as DateTime?;
//            DateTime? startTime = values[1] as DateTime?;
//            DateTime? endDate = values[2] as DateTime?;
//            DateTime? endTime = values[3] as DateTime?;
//            DeviceInfo device = values[4] as DeviceInfo;

//            return new ExportParameters
//            {
//                StartDateTime = CombineDateTime(startDate, startTime),
//                EndDateTime = CombineDateTime(endDate, endTime),
//                Device = device
//            };
//        }

//        private DateTime? CombineDateTime(DateTime? date, DateTime? time)
//        {
//            if (!date.HasValue || !time.HasValue) return null;
//            return date.Value.Date + time.Value.TimeOfDay;
//        }

//        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public class PlaybackParamsConverter : IMultiValueConverter
//    {
//        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
//        {
//            DateTime? startDate = values[0] as DateTime?;
//            DateTime? startTime = values[1] as DateTime?;
//            DateTime? endDate = values[2] as DateTime?;
//            DateTime? endTime = values[3] as DateTime?;

//            return new PlaybackParameters
//            {
//                StartDateTime = CombineDateTime(startDate, startTime),
//                EndDateTime = CombineDateTime(endDate, endTime)
//            };
//        }

//        private DateTime? CombineDateTime(DateTime? date, DateTime? time)
//        {
//            if (!date.HasValue || !time.HasValue) return null;
//            return date.Value.Date + time.Value.TimeOfDay;
//        }

//        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
