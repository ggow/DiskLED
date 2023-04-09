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
        public DiskMonitorReportEventArgs(bool hasActivity)
        {
            TimeNow = DateTime.Now;

            // this could be a post process function/filter
            // keep it simple for now
            HasActivity = hasActivity;
        }
        private DateTime TimeNow;
        //private float Sample;
        public bool HasActivity = false;
    }

    class DiskMonitor
    {
        private const string pcCategory = "PhysicalDisk";
        private const string pcName = "Disk Bytes/sec";
        private List<PerformanceCounter> performanceCounter;

        public int Interval { get; set; }

        // Clients subscribe for reports through this event handler
        public event DiskMonitorReportHandler Report;
        public delegate void DiskMonitorReportHandler(object sender, DiskMonitorReportEventArgs e);

        private string[] getPhysicalDrives(string categoryName)
        {
            PerformanceCounterCategory category =
                PerformanceCounterCategory.GetCategories().First(c => c.CategoryName == categoryName);

            string[] instanceNames = category.GetInstanceNames();

            return instanceNames;
        }

        public DiskMonitor(int interval)
        {
            performanceCounter = new List<PerformanceCounter>();

            foreach (string instanceName in getPhysicalDrives(pcCategory))
            {
                performanceCounter.Add(new PerformanceCounter(pcCategory, pcName, instanceName));
            }

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
                        bool isActivity = false;
                        foreach (PerformanceCounter pc in performanceCounter)
                        {
                            if (pc.NextValue() > 0)
                            {
                                isActivity = true;
                                break;
                            }
                        }
                        DiskMonitorReportEventArgs e =
                            new DiskMonitorReportEventArgs(isActivity);

                        Report(this, e);
                    }

                    token.ThrowIfCancellationRequested();
                }
            }, token);
        }
    }
}
