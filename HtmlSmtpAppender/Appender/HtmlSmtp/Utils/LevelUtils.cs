/*
 *   (c) Greg Brackley, 2010
 *   
 *   $Id: LevelUtils.cs 12 2010-10-05 09:54:31Z greg $ 
 */
using log4net.Core;

namespace log4net.Appender.HtmlSmtp.Utils
{
    public static class LevelUtils
    {
        /// <summary>
        ///   The list of predefined levels. 
        /// </summary>
        public static readonly Level[] All = new[]
        {
            Level.Off,

            // Unrecoverable errors
            Level.Emergency,
            Level.Fatal,
            Level.Alert,

            // Recoverable errors
            Level.Critical,
            Level.Severe,
            Level.Error,
            Level.Warn,

            // Information
            Level.Notice,
            Level.Info,

            // Debug
            Level.Debug,
            Level.Fine,
            Level.Trace,
            Level.Finer,
            Level.Verbose,
            Level.Finest,
            Level.All,
        };
    }
}
