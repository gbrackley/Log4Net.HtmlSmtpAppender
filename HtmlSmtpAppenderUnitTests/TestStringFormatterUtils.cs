/*
 *   (c) Greg Brackley, 2010
 *   
 *   $Id: TestStringFormatterUtils.cs 12 2010-10-05 09:54:31Z greg $ 
 */
using System;
using System.Reflection;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Formatter;
using NUnit.Framework;

namespace HtmlSmtpAppenderUnitTests
{
    [TestFixture]
    internal class TestStringFormatterUtils
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (TestStringFormatterUtils).Name);

        [Test]
        public void TestEmpty()
        {
            Assert.AreEqual("", StringFormatterUtil.Format("", null, 0));
        }

        [Test]
        public void TestPercent()
        {
            Assert.AreEqual("single %, and two %%", StringFormatterUtil.Format("single %%, and two %%%%", null, 0));
        }


        [Test]
        public void TestPath()
        {
            Assert.AreEqual(
                string.Format("Path is {0}", Environment.GetEnvironmentVariable("Path")),
                StringFormatterUtil.Format("Path is %env{Path}", null, 0));
        }


        [Test]
        public void TestEvents()
        {
            var loggingEvents = new[]
            {
                new LoggingEvent(
                    Log.GetType(),
                    LogManager.GetRepository(Assembly.GetCallingAssembly()),
                    Log.Logger.Name,
                    Level.Emergency,
                    "An emergency",
                    new ArgumentException("Sample exception")),
                new LoggingEvent(
                    Log.GetType(),
                    LogManager.GetRepository(Assembly.GetCallingAssembly()),
                    Log.Logger.Name,
                    Level.Info,
                    "Some information",
                    new ArgumentException("Sample exception")),
                new LoggingEvent(
                    Log.GetType(),
                    LogManager.GetRepository(Assembly.GetCallingAssembly()),
                    Log.Logger.Name,
                    Level.Debug,
                    "Debug 1",
                    new ArgumentException("Sample exception")),
                new LoggingEvent(
                    Log.GetType(),
                    LogManager.GetRepository(Assembly.GetCallingAssembly()),
                    Log.Logger.Name,
                    Level.Debug,
                    "Debug 2",
                    null),
            };
            loggingEvents[0].Properties[HtmlSmtpAppender.IsTriggerLoggingEvent] = true;

            Assert.AreEqual(
                "total=4 trig=1 ~trig=3 f=0 f=1 i=1 d=2 lost=45 total=4",
                StringFormatterUtil.Format(
                    "total=%events{total} trig=%events{triggering} ~trig=%events{nontriggering} f=%events{fatal} f=%events{emergency} i=%events{info} d=%events{debug} lost=%events{lost} total=%events",
                    loggingEvents,
                    45));
        }

        [Test]
        public void TestEventsClass()
        {
            var loggingEvents = new[]
            {
                new LoggingEvent(
                    Log.GetType(),
                    LogManager.GetRepository(Assembly.GetCallingAssembly()),
                    Log.Logger.Name,
                    Level.Emergency,
                    "An emergency",
                    new ArgumentException("Sample exception")),
                new LoggingEvent(
                    Log.GetType(),
                    LogManager.GetRepository(Assembly.GetCallingAssembly()),
                    Log.Logger.Name,
                    Level.Info,
                    "Some information",
                    new ArgumentException("Sample exception")),
                new LoggingEvent(
                    Log.GetType(),
                    LogManager.GetRepository(Assembly.GetCallingAssembly()),
                    Log.Logger.Name,
                    Level.Debug,
                    "Debug 1",
                    new ArgumentException("Sample exception")),
                new LoggingEvent(
                    Log.GetType(),
                    LogManager.GetRepository(Assembly.GetCallingAssembly()),
                    Log.Logger.Name,
                    Level.Debug,
                    "Debug 2",
                    null),
            };
            loggingEvents[0].Properties[HtmlSmtpAppender.IsTriggerLoggingEvent] = true;

            Assert.AreEqual(
                "total=4 u=1 r=0 i=1 d=2",
                StringFormatterUtil.Format(
                    "total=%events{total} u=%events{class.unrecoverable} r=%events{class.recoverable} i=%events{class.information} d=%events{class.debug}",
                    loggingEvents,
                    45));
        }
    }
}
