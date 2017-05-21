/*
 *   $Id: DnsQuery.cs 8 2010-09-30 01:59:10Z greg $ 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace log4net.Appender.HtmlSmtp.Utils
{
    public class MxRecord
    {
        public string Name { get; set; }
        public short Preference { get; set; }
    }

    /// <summary>
    /// Provide a wrapper around 'DnsApi' to provide basic query support.
    /// </summary>
    /// <seealso cref="http://www.eggheadcafe.com/PrintSearchContent.asp?LINKID=889"/>
    /// <seealso cref="http://www.eggheadcafe.com/articles/20050129.asp "/>
    /// <seealso cref="http://social.msdn.microsoft.com/Forums/en-US/csharpgeneral/thread/89b21138-596f-4efc-8e86-d440d260c41e/"/>
    public static class DnsQuery
    {
        /// <summary>
        /// Query a name for MX records.
        /// </summary>
        /// <returns>The list of MX records records unordered (natural ordering)</returns>
        public static IList<MxRecord> QueryMx(string domain)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                throw new NotSupportedException();
            }

            const QueryTypes queryType = QueryTypes.DNS_TYPE_MX;

            IntPtr queryResults = IntPtr.Zero;
            int win32Code = DnsApiDnsQuery(
                ref domain,
                queryType,
                QueryOptions.DNS_QUERY_STANDARD,
                0,
                ref queryResults,
                0);
            if (win32Code != 0)
            {
                throw new Win32Exception(win32Code);
            }

            var mxRecords = new List<MxRecord>();
            WinApiMxRecord recWinApiMx;
            for (IntPtr nextResult = queryResults; !nextResult.Equals(IntPtr.Zero); nextResult = recWinApiMx.pNext)
            {
                recWinApiMx = (WinApiMxRecord) Marshal.PtrToStructure(nextResult, typeof (WinApiMxRecord));
                if (recWinApiMx.wType == (short)queryType)
                {
                    string exchangerName = Marshal.PtrToStringAuto(recWinApiMx.pNameExchange);
                    mxRecords.Add(
                        new MxRecord
                        {
                            Name = exchangerName,
                            Preference = recWinApiMx.wPreference,
                        });
                }
            }

            // Free the original query results 
            DnsApiDnsRecordListFree(queryResults, 0);
            return mxRecords;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <seealso cref="http://msdn.microsoft.com/en-us/library/cc982162%28VS.85%29.aspx"/>
        private enum QueryOptions
        {
            DNS_QUERY_ACCEPT_TRUNCATED_RESPONSE = 1,
            DNS_QUERY_BYPASS_CACHE = 8,
            DNS_QUERY_DONT_RESET_TTL_VALUES = 0x100000,
            DNS_QUERY_NO_HOSTS_FILE = 0x40,
            DNS_QUERY_NO_LOCAL_NAME = 0x20,
            DNS_QUERY_NO_NETBT = 0x80,
            DNS_QUERY_NO_RECURSION = 4,
            DNS_QUERY_NO_WIRE_QUERY = 0x10,
            DNS_QUERY_RESERVED = -16777216,
            DNS_QUERY_RETURN_MESSAGE = 0x200,
            DNS_QUERY_STANDARD = 0,
            DNS_QUERY_TREAT_AS_FQDN = 0x1000,
            DNS_QUERY_USE_TCP_ONLY = 2,
            DNS_QUERY_WIRE_ONLY = 0x100
        }


        /// <summary>
        /// The DNS_MX_DATA structure represents a DNS mail exchanger (MX) record as specified in section 3.3.9 of RFC 1035.
        /// </summary>
        /// <remarks>
        ///   Use a  sequential layout. This handles 32bit/64bit environments better than an 'Explicit' layout.
        /// </remarks>
        /// <seealso cref="http://msdn.microsoft.com/en-us/library/ms682082%28VS.85%29.aspx"/>
        /// <seealso cref="http://msdn.microsoft.com/en-us/library/ms682070%28VS.85%29.aspx"/>
        [StructLayout(LayoutKind.Sequential)]
        private struct WinApiMxRecord
        {
            // DnsRecord fields
            public IntPtr pNext;
            public string pName;
            public short wType;
            public short wDataLength;
            public int flags;
            public int dwTtl;
            public int dwReserved;

            // Mx Record fields
            public IntPtr pNameExchange;
            public short wPreference;
            public short Pad;
        }

        public enum ErrorReturnCode
        {
            DNS_ERROR_RCODE_NO_ERROR = 0,
            DNS_ERROR_RCODE_FORMAT_ERROR = 9001,
            DNS_ERROR_RCODE_SERVER_FAILURE = 9002,
            DNS_ERROR_RCODE_NAME_ERROR = 9003,
            DNS_ERROR_RCODE_NOT_IMPLEMENTED = 9004,
            DNS_ERROR_RCODE_REFUSED = 9005,
            DNS_ERROR_RCODE_YXDOMAIN = 9006,
            DNS_ERROR_RCODE_YXRRSET = 9007,
            DNS_ERROR_RCODE_NXRRSET = 9008,
            DNS_ERROR_RCODE_NOTAUTH = 9009,
            DNS_ERROR_RCODE_NOTZONE = 9010,
            DNS_ERROR_RCODE_BADSIG = 9016,
            DNS_ERROR_RCODE_BADKEY = 9017,
            DNS_ERROR_RCODE_BADTIME = 9018
        }

        public enum QueryTypes
        {
            DNS_TYPE_A = 1,
            DNS_TYPE_CNAME = 5,
            DNS_TYPE_MX = 15,
            // DNS_TYPE_TEXT = 16,
            DNS_TYPE_SRV = 33
        }

        /// <summary>
        /// 
        /// </summary>
        /// <seealso cref="http://msdn.microsoft.com/en-us/library/ms682016%28VS.85%29.aspx"/>
        [DllImport("Dnsapi", EntryPoint = "DnsQuery_W", CharSet = CharSet.Unicode, SetLastError = true,
            ExactSpelling = true)]
        private static extern Int32 DnsApiDnsQuery(
            [MarshalAs(UnmanagedType.VBByRefStr)] ref string sName,
            QueryTypes wType,
            QueryOptions options,
            UInt32 aipServers,
            ref IntPtr ppQueryResults,
            UInt32 pReserved);

        /// <summary>
        /// The DnsRecordListFree function frees memory allocated for DNS records 
        /// obtained using the DnsQuery function.
        /// </summary>
        /// <seealso cref="http://msdn.microsoft.com/en-us/library/ms682021%28VS.85%29.aspx"/>
        [DllImport("Dnsapi", EntryPoint = "DnsRecordListFree", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void DnsApiDnsRecordListFree(IntPtr pRecordList, int FreeType);
    }
}
