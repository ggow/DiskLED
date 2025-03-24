using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace DiskLed
{
    public partial class Form1 : Form
    {
        private HddLed hddLed;
        private DiskMonitor diskMonitor;
        private CancellationTokenSource tokenSource;
        private CancellationToken token;

        [SupportedOSPlatform("windows")]
        public Form1()
        {
            InitializeComponent();

            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;

            hddLed = new HddLed();
            hddLed.ContextMenuSelectionEvent += HddLed_ContextMenuSelectionEvent;

            diskMonitor = new DiskMonitor(1);
            diskMonitor.Report += DiskMonitorReport;
        }

        [SupportedOSPlatform("windows")]
        private void DiskMonitorReport(object sender, DiskMonitorReportEventArgs e)
        {
            if (e.HasActivity)
                hddLed.turnOn();
            else
                hddLed.turnOff();
        }

        [SupportedOSPlatform("windows")]
        private void HddLed_ContextMenuSelectionEvent(object sender, ContextMenuEventArgs e)
        {
            tokenSource.Cancel();
            Application.Exit();
        }

        [SupportedOSPlatform("windows")]
        private void Form1_Load(object sender, EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.
            diskMonitor.Start(token);
        }
    }
}
