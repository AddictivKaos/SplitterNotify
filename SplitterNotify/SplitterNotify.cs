using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using SplitterNotifyCommon;

namespace SplitterNotify
{
    public partial class SplitterNotifyService : ServiceBase
    {
        SplitterNotifier notifier;
        BackgroundWorker worker = new BackgroundWorker();

        public SplitterNotifyService()
        {
            InitializeComponent();
            notifier = new SplitterNotifier();
        }

        protected override void OnStart(string[] args)
        {
            notifier.Start();
        }

        protected override void OnStop()
        {
            notifier.Stop();
        }
    }
}
