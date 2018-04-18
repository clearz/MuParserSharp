using System.ComponentModel;
using System.Runtime.InteropServices;

namespace MuParserREPL
{
    internal class Timer
    {
        [DllImport("Kernel32")]
        private static extern bool QueryPerformanceCounter(
            out long lpPerformanceCount);

        [DllImport("Kernel32")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        private long _start;
        private long _stop;
        public bool Running { private set; get; }
        private readonly long _frequency;
        private readonly decimal _multiplier = new decimal(1.0e9);

        public Timer()
        {
            if (QueryPerformanceFrequency(out _frequency) == false)
            {
                // Frequency not supported
                throw new Win32Exception();
            }
        }

        public void Start()
        {
            QueryPerformanceCounter(out _start);
            Running = true;
        }

        public long Stop()
        {
            QueryPerformanceCounter(out _stop);
            Running = false;
            return _stop - _start;
        }

        public decimal Duration(int iterations)
        {
            return (_stop - _start) * _multiplier / _frequency / iterations;
        }
    }
}
