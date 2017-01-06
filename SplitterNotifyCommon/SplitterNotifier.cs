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
using System.Collections.Specialized;

namespace SplitterNotifyCommon
{
    public class SplitterNotifier
    {
        ImapClient imap;
        string serverName = "imap.gmail.com";
        string serverHostname = string.Empty;
        string username = string.Empty;
        string password = string.Empty;
        string dbUsername = string.Empty;
        string dbPassword = string.Empty;
        string dbConnectionString = string.Empty;
        string fromFilter;
        string subjectFilter;
        BackgroundWorker worker = new BackgroundWorker();
        HtmlAddressParser htmlAddressParser;
        SplitterData splitterData;

        public SplitterNotifier()
        {
            StreamReader sr = new StreamReader("p.txt");
            string line = sr.ReadLine();
            if (line != null)
            {
                password = StringCipher.Decrypt(ConfigurationManager.AppSettings["emailEncryptedPassword"], line);
                dbPassword = StringCipher.Decrypt(ConfigurationManager.AppSettings["dbEncryptedPassword"], line);
            }
            fromFilter = ConfigurationManager.AppSettings["fromFilter"];
            subjectFilter = ConfigurationManager.AppSettings["subjectFilter"];
            username = ConfigurationManager.AppSettings["emailUsername"];
            dbUsername = ConfigurationManager.AppSettings["dbUsername"];
            serverHostname = ConfigurationManager.AppSettings["emailServerHostname"];

            string dbConnectionTemplate = ConfigurationManager.AppSettings["dbConnectionStringTemplate"];
            dbConnectionString = string.Format(dbConnectionTemplate, dbUsername, dbPassword);

            // add settings that needed to be built-up
            ConfigurationManager.AppSettings.Set("dbConnectionString", dbConnectionString);

            // construct sub-objects
            splitterData = new SplitterData(ConfigurationManager.AppSettings);
            htmlAddressParser = new HtmlAddressParser(ConfigurationManager.AppSettings);
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

            var uniqueAddressesForSale = GetUniqueAddresses(messagesToParse);

            var possibleSplitters = MatchWithKnownSplitters(uniqueAddressesForSale);

            // TODO
        }

        private HashSet<string> MatchWithKnownSplitters(HashSet<string> uniqueAddressesForSale)
        {
            return splitterData.ExplicitMatch(uniqueAddressesForSale);
        }

        private HashSet<string> GetUniqueAddresses(List<Message> messagesToParse)
        {
            HashSet<string> addresses = new HashSet<string>();
            foreach (var message in messagesToParse)
            {
                var addressesInMessage = Parse(message);
                foreach (string address in addressesInMessage)
                {
                    addresses.Add(address);
                }
            }
            return addresses;
        }

        private List<string> Parse(Message message)
        {
            if (message.Body.HasHtml)
            {
                Debug.WriteLine(message.Body.Html);
                return htmlAddressParser.GetAddressMatches(message.Body.Html);
            }
            return new List<string>();
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
