/*
 *   (c) Greg Brackley, 2010
 *   
 *   $Id: MatchUtils.cs 12 2010-10-05 09:54:31Z greg $ 
 */
using System.Text.RegularExpressions;

namespace log4net.Appender.HtmlSmtp.Utils
{
    /// <summary>
    /// Utilites in support of <see cref="Match"/>
    /// </summary>
    public static class MatchUtils
    {
        public static string GetSingletonCapture(this Match match, string groupName)
        {
            var capture = match.Groups[groupName].Captures.GetSingleton<Capture>(null);
            return (capture != null) ? capture.Value : null;
        }

        public static string GetSingletonOrDefaultCapture(this Match match, string groupName)
        {
            var capture = match.Groups[groupName].Captures.GetSingletonOrDefault<Capture>();
            return (capture != null) ? capture.Value : null;
        }
    }
}
