using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.Versioning;

namespace DiskLed
{
    enum eMenuSelections : int
    {
        E_CTX_MENU_SELECTION_EXIT = 0,
        E_CTX_MENU_SELECTION_MAX
    }

    class ContextMenuEventArgs : EventArgs
    {
        public ContextMenuEventArgs(int menuSelection)
        {
            if (menuSelection >= (int)eMenuSelections.E_CTX_MENU_SELECTION_MAX)
                throw new Exception("Context menu selection invalid!");

            MenuSelection = menuSelection;
        }

        private int MenuSelection { get; set; }
    }

    class HddLed : Control
    {
        private bool isOn;
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        public event ContextMenuHandler ContextMenuSelectionEvent;
        public delegate void ContextMenuHandler(object sender, ContextMenuEventArgs e);
        
        // LED state 
        private Icon iconOff = global::DiskLed.Properties.Resources.off;
        private Icon iconOn = global::DiskLed.Properties.Resources.on;

        [SupportedOSPlatform("windows")]
        public HddLed()
        {
            trayIcon = new NotifyIcon();
            trayMenu = new ContextMenuStrip();

            trayMenu.Items.Add("Exit", null, OnExit);
            trayIcon.Icon = iconOff;
            trayIcon.ContextMenuStrip = trayMenu;
            trayIcon.Visible = true;
            isOn = false;
        }

        [SupportedOSPlatform("windows")]
        private void OnExit(object sender, EventArgs e)
        {
            ContextMenuEventArgs cme =
                new ContextMenuEventArgs((int)eMenuSelections.E_CTX_MENU_SELECTION_EXIT);

            if (ContextMenuSelectionEvent != null)
            {
                #region remove systray icon
                trayIcon.Visible = false;
                trayIcon.Icon = null;
                trayIcon.Dispose();
                #endregion
                ContextMenuSelectionEvent(this, cme);
            }
        }

        [SupportedOSPlatform("windows")]
        public void turnOn()
        {
            if (isOn == true)
                return;

            trayIcon.Icon = iconOn;
            isOn = true;
        }

        [SupportedOSPlatform("windows")]
        public void turnOff()
        {
            if (isOn == false)
                return;

            trayIcon.Icon = iconOff;
            isOn = false;
        }
    }
}
