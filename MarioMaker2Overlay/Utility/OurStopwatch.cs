using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioMaker2Overlay.Utility
{
    internal class OurStopwatch
    {
        private Stopwatch _stopwatch = new Stopwatch();
        private TimeSpan _offset = TimeSpan.Zero;

        public long ElapsedTicks
        {
            get 
            {
                long result = 0;

                result += _offset.Ticks;
                result += _stopwatch?.ElapsedTicks ?? 0;

                return result;
            }
        }

        public TimeSpan Elapsed
        {
            get 
            { 
                TimeSpan result = _offset;

                if (_stopwatch != null)
                {
                    result = result.Add(_stopwatch.Elapsed);
                }

                return result;
            }
        }

        public void Start(TimeSpan offset)
        {
            _offset = offset;
            _stopwatch.Start();
        }

        public void Stop()
        {
            _stopwatch.Stop();
        }

        public void Restart()
        {
            _offset = TimeSpan.Zero;
            _stopwatch.Restart();
        }
    }
}
