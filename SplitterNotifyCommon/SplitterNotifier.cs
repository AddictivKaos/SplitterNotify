using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImapX;
using System.ComponentModel;

namespace SplitterNotifyCommon
{
    public class SplitterNotifier
    {
        ImapClient imap;
        string serverName = "imap.gmail.com";
        string serverHostname = string.Empty;
        string username = string.Empty;
        string password = string.Empty;
        string fromFilter;
        string subjectFilter;
        BackgroundWorker worker = new BackgroundWorker();

        public SplitterNotifier()
        {
            StreamReader sr = new StreamReader("p.txt");
            string line = sr.ReadLine();
            if (line != null)
            {
                password = StringCipher.Decrypt(ConfigurationManager.AppSettings["EncryptedPassword"], line);
            }
            fromFilter = ConfigurationManager.AppSettings["fromFilter"];
            subjectFilter = ConfigurationManager.AppSettings["subjectFilter"];
            username = ConfigurationManager.AppSettings["username"];
            serverHostname = ConfigurationManager.AppSettings["serverHostname"];

        }

        public void Start()
        {
            worker.DoWork += new DoWorkEventHandler(StartIdleProcess);

            if (worker.IsBusy)
                worker.CancelAsync();

            worker.RunWorkerAsync();
        }

        private void StartIdleProcess(object sender, DoWorkEventArgs e)
        {
            imap = new ImapClient(serverName, true);
            imap.Behavior.ExamineFolders = false; // TODO not sure if this has a positive impact
            imap.Behavior.NoopIssueTimeout = 120;
            if (imap.Connect())
            {
                if (imap.Login(username, password))
                {

                    var inbox = imap.Folders.Inbox;

                    inbox.OnNewMessagesArrived += Inbox_OnNewMessagesArrived;
                    inbox.StartIdling();
                }
            }
        }

        private void Inbox_OnNewMessagesArrived(object sender, IdleEventArgs e)
        {
            var messagesToParse = new List<Message>();
            foreach (var message in e.Messages)
            {
                // TODO filters are OR'd at the moment
                if (message.From.Address.Contains(fromFilter)
                    || message.Subject.Contains(subjectFilter))
                {
                    messagesToParse.Add(message);
                }

            }

            Parse(messagesToParse);
        }

        private void Parse(List<Message> messagesToParse)
        {
            foreach (var message in messagesToParse)
            {
                Parse(message);
            }
        }

        private void Parse(Message message)
        {
            //if (message.Body.HasText)
            //{
            //    Debug.WriteLine(message.Body.Text);
            //}
            if (message.Body.HasHtml)
            {
                Debug.WriteLine(message.Body.Html);

            }
            //throw new NotImplementedException();
        }

        public void Stop()
        {
            if (imap != null)
            {
                imap.Disconnect();
            }
        }
    }
}
