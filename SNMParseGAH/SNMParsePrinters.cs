using Canteen;
using SnmpSharpNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SNMParseGAH
{
    public partial class SNMParsePrinters : ServiceBase
    {
        Printers printers;
        public SNMParsePrinters()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            printers = new Printers();
            System.Threading.Thread printersThread = new System.Threading.Thread(new System.Threading.ThreadStart(printers.Start));
            printersThread.Start();
        }

        protected override void OnStop()
        {
            printers.Stop();
            System.Threading.Thread.Sleep(1000);
        }
    }
}
