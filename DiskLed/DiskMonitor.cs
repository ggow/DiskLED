using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DiskLed
{
    public class DiskMonitorReportEventArgs : EventArgs
    {
        public DiskMonitorReportEventArgs(float sample)
        {
            TimeNow = DateTime.Now;
            Sample = sample;

            // this could be a post process function/filter
            // keep it simple for now
            if (Sample > 0)
                HasActivity = true;
        }
        private DateTime TimeNow;
        private float Sample;
        public bool HasActivity = false;
    }

    class DiskMonitor
    {
        private const string pcCategory = "PhysicalDisk";
        private const string pcName = "Disk Bytes/sec";
        private PerformanceCounter performanceCounter;

        public int Interval { get; set; }

        // Clients subscribe for reports through this event handler
        public event DiskMonitorReportHandler Report;
        public delegate void DiskMonitorReportHandler(object sender, DiskMonitorReportEventArgs e);

        private string findSystemDrive(string categoryName)
        {
            PerformanceCounterCategory category =
                PerformanceCounterCategory.GetCategories().First(c => c.CategoryName == categoryName);

            string[] instanceNames = category.GetInstanceNames();

            foreach (string instanceName in instanceNames)
            {
                if (instanceName.Contains("C:"))
                    return instanceName;
            }

            // shouldn't reach here because PhysicalDisk has multiple instances
            return string.Empty;
        }

        public DiskMonitor(int interval)
        {
            performanceCounter = new PerformanceCounter(pcCategory, pcName, findSystemDrive(pcCategory));
            Interval = interval;
        }

        public void Start(CancellationToken token)
        {
            Task newtask = Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(Interval);

                    if (Report != null)
                    {
                        DiskMonitorReportEventArgs e =
                            new DiskMonitorReportEventArgs(performanceCounter.NextValue());

                        Report(this, e);
                    }

                    token.ThrowIfCancellationRequested();
                }
            }, token);
        }
    }
}
