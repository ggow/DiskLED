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

namespace DiskLed
{
    public partial class Form1 : Form
    {
        private HddLed hddLed;
        private DiskMonitor diskMonitor;
        private CancellationTokenSource tokenSource;
        private CancellationToken token;

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

        private void DiskMonitorReport(object sender, DiskMonitorReportEventArgs e)
        {
            if (e.HasActivity)
                hddLed.turnOn();
            else
                hddLed.turnOff();
        }

        private void HddLed_ContextMenuSelectionEvent(object sender, ContextMenuEventArgs e)
        {
            tokenSource.Cancel();
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.
            diskMonitor.Start(token);
        }
    }
}
