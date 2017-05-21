/*
 *   (c) Greg Brackley, 2010
 *   
 *   $Id: TestMakeXmlLoggingEvents.cs 8 2010-09-30 01:59:10Z greg $ 
 */
using System;
using System.Reflection;
using System.Xml;
using HtmlSmtpAppenderUnitTests.Utils;
using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using NUnit.Framework;

namespace HtmlSmtpAppenderUnitTests
{
    [TestFixture]
    public class TestMakeXmlLoggingEvents
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (TestMakeXmlLoggingEvents).Name);

        [SetUp]
        public void Logging()
        {
            BasicConfigurator.Configure();
        }

        [Test]
        public void TestCreateSingleEventXml()
        {
            ILayout layout = log4net.Appender.HtmlSmtpAppender.MakeLayout();
            var events = new[]
            {
                new LoggingEvent(
                    Log.GetType(),
                    LogManager.GetRepository(Assembly.GetCallingAssembly()),
                    new LoggingEventData
                    {
                        Message = "This is a sample message",
                        Level = Level.Critical,
                        TimeStamp = DateTime.UtcNow,
                        ThreadName = "None",
                    }),
            };

            // Make the XML
            string xml = log4net.Appender.HtmlSmtpAppender.MakeBodyXml(layout, events);
            Log.InfoFormat("XML (in canonical form) is '{0}'", xml);

            // Verify the genrated XML can be loaded into a document
            new XmlDocument().LoadXml(xml);
            Log.Info(PrettyPrintXml.ToString(xml));
        }

        [Test]
        public void TestCreateSingleFixedEventXml()
        {
            ILayout layout = log4net.Appender.HtmlSmtpAppender.MakeLayout();
            var events = new[]
            {
                new LoggingEvent(
                    Log.GetType(),
                    LogManager.GetRepository(Assembly.GetCallingAssembly()),
                    new LoggingEventData
                    {
                        Message = "This is a sample message",
                        Level = Level.Critical,
                        TimeStamp = DateTime.UtcNow,
                        ThreadName = "None",
                    }),
            };
            events[0].Fix = FixFlags.All;

            // Make the XML
            string xml = log4net.Appender.HtmlSmtpAppender.MakeBodyXml(layout, events);
            Log.InfoFormat("XML (in canonical form) is '{0}'", xml);

            // Verify the genrated XML can be loaded into a document
            new XmlDocument().LoadXml(xml);
            Log.Info(PrettyPrintXml.ToString(xml));
        }

        [Test]
        public void TestCreateMultipleEventXml()
        {
            ILayout layout = log4net.Appender.HtmlSmtpAppender.MakeLayout();
            var events = new[]
            {
                new LoggingEvent(
                    Log.GetType(),
                    LogManager.GetRepository(Assembly.GetCallingAssembly()),
                    new LoggingEventData
                    {
                        Message = "This is a sample first message",
                        Level = Level.Critical,
                        TimeStamp = DateTime.UtcNow,
                        ThreadName = "None",
                    }),
                new LoggingEvent(
                    Log.GetType(),
                    LogManager.GetRepository(Assembly.GetCallingAssembly()),
                    new LoggingEventData
                    {
                        Message = "This is a sample second message",
                        Level = Level.Warn,
                        TimeStamp = DateTime.UtcNow,
                        ThreadName = "None",
                    }),
                new LoggingEvent(
                    Log.GetType(),
                    LogManager.GetRepository(Assembly.GetCallingAssembly()),
                    Log.Logger.Name,
                    Level.Emergency,
                    "This is the third same message",
                    new ArgumentException("Sample exception")),
            };

            // Make the XML
            string xml = log4net.Appender.HtmlSmtpAppender.MakeBodyXml(layout, events);
            Log.InfoFormat("XML (in canonical form) is '{0}'", xml);

            // Verify the genrated XML can be loaded into a document
            new XmlDocument().LoadXml(xml);
            Log.Info(PrettyPrintXml.ToString(xml));
        }


        [Test]
        public void TestCreateSingleEventWithNewlinesInMessage()
        {
            ILayout layout = log4net.Appender.HtmlSmtpAppender.MakeLayout();
            var events = new[]
            {
                new LoggingEvent(
                    Log.GetType(),
                    LogManager.GetRepository(Assembly.GetCallingAssembly()),
                    new LoggingEventData
                    {
                        Message = @"
                            This is a
                             sample event 
                              with new lines.",
                        Level = Level.Critical,
                        TimeStamp = DateTime.UtcNow,
                        ThreadName = "None",
                    }),
            };
            events[0].Fix = FixFlags.All;

            // Make the XML
            string xml = log4net.Appender.HtmlSmtpAppender.MakeBodyXml(layout, events);
            Log.InfoFormat("XML (in canonical form) is '{0}'", xml);
            Log.Info(PrettyPrintXml.ToString(xml));

            // Verify the genrated XML can be loaded into a document
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            // Check that the message XML contains a new line, and it's the 
            // same as the original message text
            var xmlMessage = xmlDoc.SelectSingleNode("log4net/events/event/message").InnerText;
            Assert.IsTrue(xmlMessage.Contains("\n"));
            Assert.AreEqual(events[0].GetLoggingEventData().Message, xmlMessage);

        }


        [Test]
        public void TestCreateSingleEventWithException()
        {
            ILayout layout = log4net.Appender.HtmlSmtpAppender.MakeLayout();
            var events = new[]
            {
                new LoggingEvent(
                    Log.GetType(),
                    LogManager.GetRepository(Assembly.GetCallingAssembly()),
                    new LoggingEventData
                    {
                        Message = @"
                            This is a
                             sample event 
                              with new lines.",
                        Level = Level.Critical,
                        TimeStamp = DateTime.UtcNow,
                        ThreadName = "None",
                    }),
            };
            events[0].Fix = FixFlags.All;

            // Make the XML
            string xml = log4net.Appender.HtmlSmtpAppender.MakeBodyXml(layout, events);
            Log.InfoFormat("XML (in canonical form) is '{0}'", xml);
            Log.Info(PrettyPrintXml.ToString(xml));

            // Verify the genrated XML can be loaded into a document
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            // Check that the message XML contains a new line, and it's the 
            // same as the original message text
            var xmlMessage = xmlDoc.SelectSingleNode("log4net/events/event/message").InnerText;
            Assert.IsTrue(xmlMessage.Contains("\n"));
            Assert.AreEqual(events[0].GetLoggingEventData().Message, xmlMessage);

        }
    }
}
