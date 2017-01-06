using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SplitterNotifyCommon;
using System.IO;

namespace SplitterNotifyCommonTests
{
    [TestClass]
    public class SplitterDataTest
    {
        private static string line;

        [ClassInitialize]
        public static void StaticInitialise(TestContext context)
        {
            StreamReader sr = new StreamReader("p.txt");
            line = sr.ReadLine();
        }

        [TestMethod]
        public void TestExplicitMatch()
        {
            NameValueCollection settings = new NameValueCollection();
            settings["connectionString"] = "Server=tcp:addictivkaos.database.windows.net,1433;" +
                "Initial Catalog=PropertyData;Persist Security Info=False;" +
                "User ID=ReadOnly;" +
                "Password=" +
                StringCipher.Decrypt("7AiMMCiVkH9aVBdWnezVWg / RfqYQ2k1N7ccITWTNtn4qqitovqKciuNMvNQfOntZ1HEFurOSvk4K0NUkgAmuqsGGM1KMqUvbaF08NkASlAbPQou2Cr9jOVZUfsoTdIMW", line) + ";" +
                "MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            settings["searchQueryTemplate"] = "SELECT DISTINCT [Address] FROM [dbo].[SplittersView] WHERE [Address] = '{0}'";

            SplitterData sd = new SplitterData(settings);
            var addressesToSearch = new HashSet<string>();
            addressesToSearch.Add("10 FAUCETT ST, MITCHELTON, QLD 4053");
            addressesToSearch.Add("10 FAUCETTasdwe ST, MITCHELTON, QLD 4053");
            var results = sd.ExplicitMatch(addressesToSearch);
            Assert.AreEqual(1, results.Count);
        }
    }
}
