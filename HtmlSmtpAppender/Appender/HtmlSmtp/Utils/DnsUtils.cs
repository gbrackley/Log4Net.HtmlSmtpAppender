/*
 *   (c) Greg Brackley, 2010
 *   
 *   $Id: DnsUtils.cs 12 2010-10-05 09:54:31Z greg $ 
 */
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace log4net.Appender.HtmlSmtp.Utils
{
    public static class DnsUtils
    {

        public static string GetDomainName(string fqdn)
        {
            return Regex.Replace(
                fqdn, 
                @"^ 
                    [^\.]+                  # hostname
                    (
                        \.                  # dot 
                        (?<domain> .*  )    # the domain name
                    )?                      # the domain name is optional
                  $
                 ", 
                 @"${domain}", 
                 RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant);
        }

        /// <summary>
        /// Get the domain name of the computer (without the host name part).
        /// </summary>
        /// <remarks>
        /// The 'Primary DNS name for this computer' setting is part of the computer
        /// properties for a standalone computer. For a computer which is part of a Windows
        /// domain, the DNS domain name is likely to be configured to use a name
        /// related to the Windows domain.
        /// </remarks>
        public static string GetDomainName()
        {
            return Registry.GetValue(
                    @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", 
                    "Domain", 
                    "") as string;
        }

    }
}
