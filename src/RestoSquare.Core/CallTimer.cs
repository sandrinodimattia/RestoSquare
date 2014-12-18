using System;
using System.Diagnostics;
using System.Globalization;

namespace RestoSquare.Core
{
    public class CallTimer : IDisposable
    {
        private readonly Stopwatch _timer;

        public CallTimer()
        {
            _timer = new Stopwatch();
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public string TotalSeconds
        {
            get { return Math.Round(_timer.Elapsed.TotalSeconds, 2).ToString(CultureInfo.InvariantCulture); }
        }

        public void Dispose()
        {
            _timer.Stop();
        }

        public static CallTimer Start()
        {
            return new CallTimer();
        }
    }
}
