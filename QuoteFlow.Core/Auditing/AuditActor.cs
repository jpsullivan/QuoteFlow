﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace QuoteFlow.Core.Auditing
{
    public class AuditActor
    {
        public string MachineName { get; set; }
        public string MachineIP { get; set; }
        public string UserName { get; set; }
        public string AuthenticationType { get; set; }
        public DateTime TimestampUtc { get; set; }

        public AuditActor OnBehalfOf { get; set; }

        public AuditActor(string machineName, string machineIP, string userName, string authenticationType, DateTime timeStampUtc, AuditActor onBehalfOf = null)
        {
            MachineName = machineName;
            UserName = userName;
            AuthenticationType = authenticationType;
            TimestampUtc = timeStampUtc;
            OnBehalfOf = onBehalfOf;
        }

        public static async Task<AuditActor> GetCurrentMachineActor(AuditActor onBehalfOf = null)
        {
            // Try to get local IP
            string ipAddress = await GetLocalIP();

            return new AuditActor(
                Environment.MachineName,
                ipAddress,
                $@"{Environment.UserDomainName}\{Environment.UserName}",
                "MachineUser",
                DateTime.UtcNow,
                onBehalfOf);
        }

        public static async Task<string> GetLocalIP()
        {
            string ipAddress = null;
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                var entry = await Dns.GetHostEntryAsync(Dns.GetHostName());
                if (entry != null)
                {
                    ipAddress =
                        TryGetAddress(entry.AddressList, AddressFamily.InterNetworkV6) ??
                        TryGetAddress(entry.AddressList, AddressFamily.InterNetwork);
                }
            }
            return ipAddress;
        }

        private static string TryGetAddress(IEnumerable<IPAddress> addrs, AddressFamily family)
        {
            return addrs.Where(a => a.AddressFamily == family).Select(a => a.ToString()).FirstOrDefault();
        }
    }
}
