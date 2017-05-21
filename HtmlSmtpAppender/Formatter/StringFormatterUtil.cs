using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;
using log4net.Appender;
using log4net.Appender.HtmlSmtp.Utils;
using log4net.Core;
using log4net.Util;

namespace log4net.Formatter
{
    /// <summary>
    /// </summary>
    /// <seealso cref="IStringFormatter" />
    /// <seealso cref="StringFormatter" />
    public static class StringFormatterUtil
    {
        public static string Format(
            string format,
            IEnumerable<LoggingEvent> events,
            long loggingEventsLost)
        {
            return new StringFormatter().Format(
                format,
                (name, parameters) =>
                {
                    if ("env" == name)
                    {
                        return Environment(parameters);
                    }
                    else if ("events" == name)
                    {
                        return EventCount(parameters, events, loggingEventsLost);
                    }
                    else
                    {
                        LogLog.Error(
                            typeof (StringFormatterUtil),
                            string.Format(
                                "The replacement parameter '{0}' with parameters '{1}' is not supported",
                                name,
                                parameters));
                        return "";
                    }
                });
        }

        /// <summary>
        ///     Get an environment variable.
        /// </summary>
        public static string Environment(string parameters)
        {
            try
            {
                if (!string.IsNullOrEmpty(parameters))
                {
                    // Lookup the environment variable
                    return System.Environment.GetEnvironmentVariable(parameters);
                }
            }
            catch (SecurityException secEx)
            {
                // This security exception will occur if the caller does not have 
                // unrestricted environment permission. If this occurs the expansion 
                // will be skipped with the following warning message.
                LogLog.Debug(
                    typeof (StringFormatterUtil),
                    "EnvironmentPatternConverter: Security exception while trying to expand environment variables. Error Ignored. No Expansion.",
                    secEx);
            }
            catch (Exception ex)
            {
                LogLog.Error(
                    typeof (StringFormatterUtil),
                    "EnvironmentPatternConverter: Error occurred while converting environment variable.",
                    ex);
            }
            return "";
        }

        /// <summary>
        ///     Get the count of events
        /// </summary>
        public static string EventCount(
            string parameters,
            IEnumerable<LoggingEvent> events,
            long loggingEventsLost)
        {
            if (string.IsNullOrEmpty(parameters) ||
                "total".Equals(parameters, StringComparison.InvariantCultureIgnoreCase))
            {
                return events.LongCount().ToString();
            }
            else if ("triggering".Equals(parameters, StringComparison.InvariantCultureIgnoreCase))
            {
                return events
                    .Where(e => e.Properties[HtmlSmtpAppender.IsTriggerLoggingEvent] != null)
                    .LongCount()
                    .ToString();
            }
            else if ("lost".Equals(parameters, StringComparison.InvariantCultureIgnoreCase))
            {
                return loggingEventsLost.ToString();
            }
            else if ("nontriggering".Equals(parameters, StringComparison.InvariantCultureIgnoreCase))
            {
                return events
                    .Where(e => e.Properties[HtmlSmtpAppender.IsTriggerLoggingEvent] == null)
                    .LongCount()
                    .ToString();
            }
            else if (Regex.IsMatch(parameters, @"\d+"))
            {
                int level = int.Parse(parameters);
                return events.Where(e => e.Level.Value == level).LongCount().ToString();
            }
            else if (Enumerable.Any<Level>(LevelUtils.All,
                e => e.Name.Equals(parameters, StringComparison.InvariantCultureIgnoreCase)))
            {
                return events
                    .Where(e => e.Level.Name.Equals(parameters, StringComparison.InvariantCultureIgnoreCase))
                    .LongCount()
                    .ToString();
            }
            else if ("class.unrecoverable".Equals(parameters, StringComparison.InvariantCultureIgnoreCase))
            {
                // Critical < level <= Off
                return events.Where(e => e.Level.Value > Level.Critical.Value).LongCount().ToString();
            }
            else if ("class.recoverable".Equals(parameters, StringComparison.InvariantCultureIgnoreCase))
            {
                // Notice < level <= Critical
                return events
                    .Where(e => e.Level.Value > Level.Notice.Value && e.Level.Value <= Level.Critical.Value)
                    .LongCount()
                    .ToString();
            }
            else if ("class.information".Equals(parameters, StringComparison.InvariantCultureIgnoreCase))
            {
                // Debug < level <= Notice
                return events
                    .Where(e => e.Level.Value > Level.Debug.Value && e.Level.Value <= Level.Notice.Value)
                    .LongCount()
                    .ToString();
            }
            else if ("class.debug".Equals(parameters, StringComparison.InvariantCultureIgnoreCase))
            {
                // level <= Debug
                return events.Where(e => e.Level.Value <= Level.Debug.Value).LongCount().ToString();
            }
            else
            {
                LogLog.Error(
                    typeof (StringFormatterUtil),
                    string.Format("Invalid event count parameter '{0}'", parameters));
                return "";
            }
        }
    }
}
