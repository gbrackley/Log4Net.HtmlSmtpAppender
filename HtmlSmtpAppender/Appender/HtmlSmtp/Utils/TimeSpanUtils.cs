/*
 *   (c) Greg Brackley, 2010
 *   
 *   $Id: TimeSpanUtils.cs 8 2010-09-30 01:59:10Z greg $ 
 */
using System;

namespace log4net.Appender.HtmlSmtp.Utils
{
    public class TimeSpanUtils
    {
        /// <summary>
        /// Return the maximum of two <see cref="TimeSpan"/>
        /// </summary>
        /// <seealso cref="Math.Max(long,long)"/>
        public static TimeSpan Max(TimeSpan a, TimeSpan b)
        {
            return a.Ticks > b.Ticks ? a : b;
        }

        /// <summary>
        /// Return the minimum of two <see cref="TimeSpan"/>
        /// </summary>
        /// <seealso cref="Math.Min(long,long)"/>
        public static TimeSpan Min(TimeSpan a, TimeSpan b)
        {
            return a.Ticks < b.Ticks ? a : b;
        }
    }
}
