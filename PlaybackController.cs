using Robo_Sense_Monitor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Robo_Sense_Monitor
{
    public class PlaybackController
    {
        private DispatcherTimer _timer;
        private List<SensorData> _data;
        private int _currentIndex;
        private double _playbackSpeed = 1.0; // 1x normal speed

        public event Action<SensorData> OnNewData;
        public event Action OnPlaybackComplete;

        public PlaybackController()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            _timer.Tick += PlaybackStep;
        }

        public void LoadData(List<SensorData> data)
        {
            _data = data.OrderBy(d => d.Timestamp).ToList();
            _currentIndex = 0;
        }

        public void StartPlayback(double speedMultiplier = 1.0)
        {
            _playbackSpeed = speedMultiplier;
            _timer.Interval = TimeSpan.FromMilliseconds(100 / _playbackSpeed);
            _timer.Start();
        }

        public void PausePlayback() => _timer.Stop();

        private void PlaybackStep(object sender, EventArgs e)
        {
            if (_currentIndex >= _data.Count)
            {
                _timer.Stop();
                OnPlaybackComplete?.Invoke();
                return;
            }

            var currentData = _data[_currentIndex];
            OnNewData?.Invoke(currentData);
            _currentIndex++;
        }
    }
}
