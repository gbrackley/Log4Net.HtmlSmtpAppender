/*
 *   (c) Greg Brackley, 2010
 *   
 *   $Id: LogExtensions.cs 12 2010-10-05 09:54:31Z greg $ 
 */
using System;
using System.Globalization;
using log4net;
using log4net.Core;
using log4net.Util;

namespace HtmlSmtpAppenderUnitTests.Utils
{
   public static  class LogExtensions
    {
       private readonly static Type ThisDeclaringType = typeof(LogImpl);

        public static void TraceFormat(this ILog logger, string format, params object[] args)
        {
            logger.Logger.Log(ThisDeclaringType, Level.Trace, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null /* no exception */);
        }

        public static void Trace(this ILog logger, object message)
        {
            logger.Logger.Log(ThisDeclaringType, Level.Trace, message, null /* no exception */);
        }

        public static void Trace(this ILog logger, object message, Exception e)
        {
            logger.Logger.Log(ThisDeclaringType, Level.Trace, message, e);
        }

        public static void NoticeFormat(this ILog logger, string format, params object[] args)
        {
            logger.Logger.Log(ThisDeclaringType, Level.Notice, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null /* no exception */);
        }

        public static void Notice(this ILog logger, object message)
        {
            logger.Logger.Log(ThisDeclaringType, Level.Notice, message, null /* no exception */);
        }

        public static void Notice(this ILog logger, object message, Exception e)
        {
            logger.Logger.Log(ThisDeclaringType, Level.Notice, message, e);
        }
    }
}
