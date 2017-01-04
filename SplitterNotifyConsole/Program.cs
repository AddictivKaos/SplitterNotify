using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplitterNotifyCommon;

namespace SplitterNotifyConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            SplitterNotifier notifier  = new SplitterNotifier();

            notifier.Start();

            Console.ReadLine();
        }
    }
}
