/*
 *   (c) Greg Brackley, 2010
 *   
 *   $Id: TestLoggingEventFixOptions.cs 7 2010-09-29 05:55:12Z greg $ 
 */
using log4net.Config;
using log4net.Core;
using NUnit.Framework;

namespace HtmlSmtpAppenderIntegrationTests
{
    [TestFixture]
    public class TestLoggingEventFixOptions
    {
        [Test]
        public void TestFixNone()
        {
            PerformLoggingWithFixOption(FixFlags.None);
        }

        [Test]
        public void TestFixNdc()
        {
            PerformLoggingWithFixOption(FixFlags.Ndc);
        }

        [Test]
        public void TestFixMessage()
        {
            PerformLoggingWithFixOption(FixFlags.Message);
        }

        [Test]
        public void TestFixThreadName()
        {
            PerformLoggingWithFixOption(FixFlags.ThreadName);
        }

        [Test]
        public void TestFixLocationInfo()
        {
            PerformLoggingWithFixOption(FixFlags.LocationInfo);
        }

        [Test]
        public void TestFixUserName()
        {
            PerformLoggingWithFixOption(FixFlags.UserName);
        }

        [Test]
        public void TestFixDomain()
        {
            PerformLoggingWithFixOption(FixFlags.Domain);
        }

        [Test]
        public void TestFixIdentity()
        {
            PerformLoggingWithFixOption(FixFlags.Identity);
        }

        [Test]
        public void TestFixNoneException()
        {
            PerformLoggingWithFixOption(FixFlags.Exception);
        }

        [Test]
        public void TestFixNoneProperties()
        {
            PerformLoggingWithFixOption(FixFlags.Properties);
        }


        [Test]
        public void TestFixAll()
        {
            PerformLoggingWithFixOption(FixFlags.All);
        }

        private static void PerformLoggingWithFixOption(FixFlags fixFlags)
        {
            var appender = new log4net.Appender.HtmlSmtpAppender()
            {
                Name = "TestInstance",
                To = "homer@simpson.com",
                From = "homer@simpson.com",
                Fix = fixFlags,
            };
            appender.ActivateOptions();

            BasicConfigurator.Configure(appender);
        }
    }
}
