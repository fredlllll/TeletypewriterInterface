using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeletypewriterInterface
{
    public static class Util
    {
        public static void Sleep(double seconds)
        {
            Stopwatch sw = Stopwatch.StartNew();
            double threadSleepTime = seconds - 0.004; //sleep 4ms less to balance out inaccuracies of scheduler
            if (threadSleepTime > 0)
            {
                Thread.Sleep(TimeSpan.FromSeconds(threadSleepTime));
            }
            while(sw.Elapsed.TotalSeconds < seconds)
            {
                //busy waiting
            }
            sw.Stop();
        }
    }
}
