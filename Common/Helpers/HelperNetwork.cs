using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class HelperNetwork
    {
        /// <summary>
        /// Returns the remote machine name from the specificated ip
        /// </summary>
        public string GetMachineRemoteName(string ip)
        {
            var isValidIP = IPAddress.TryParse(ip, out var ipAddress);
            if (isValidIP)
            {
                var GetIPHost = Dns.GetHostEntry(ipAddress);
                var computerNames = GetIPHost.HostName.ToString().Split('.').ToList();
                var machineNameRemote = computerNames[0];

                return machineNameRemote;
            }

            return string.Empty;
        }
    }
}
