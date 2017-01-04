using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SplitterNotifyCommon
{
    public class HtmlAddressParser
    {
        const string AddressMatchGroup = "address";
        Regex addressPattern;
        public HtmlAddressParser()
        {
            addressPattern = new Regex("<a href[^>]+title=\"View this property on realestate\\.com\\.au\"[^<]*>(?<" + AddressMatchGroup + ">[^< \t\r\n][^<]+)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public List<string> GetAddressMatches(string input)
        {
            var matchedAddresses = new List<string>();
            var matches = addressPattern.Matches(input);
            foreach (Match match in matches)
            {
                var group = match.Groups[AddressMatchGroup];
                if (group.Success)
                {
                    matchedAddresses.Add(group.Value);
                }
            }
            return matchedAddresses;
        }
    }
}
