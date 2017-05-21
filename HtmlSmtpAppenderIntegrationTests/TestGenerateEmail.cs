/*
 *   (c) Greg Brackley, 2010
 *   
 *   $Id: TestGenerateEmail.cs 13 2010-10-06 01:01:29Z greg $ 
 */
using HtmlSmtpAppenderUnitTests.Utils;
using log4net;
using log4net.Config;
using log4net.Util;
using NUnit.Framework;

namespace HtmlSmtpAppenderIntegrationTests
{
    [TestFixture]
    public class TestGenerateEmail
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (TestGenerateEmail).Name);

        [SetUp]
        public void InitialiseLog4Net()
        {
            var appender = new log4net.Appender.HtmlSmtpAppender
            {
                Name = "TestInstance",
                To = "Greg Brackley <greg.brackley-log4net-htmlsmtpappender-test@lucidsolutions.co.nz>",
                From = "greg.brackley-log4net-htmlsmtpappender-test-nunit@lucidsolutions.co.nz",
                Transport = "smtp://smtp.lucidsolutions.co.nz",
                Subject = "[TEST] %events{triggering} of %events{total} [%events{class.unrecoverable},%events{class.recoverable},%events{class.information},%events{class.debug}] (lost %events{lost})",
            };
            appender.ActivateOptions();
            BasicConfigurator.Configure(appender);
            LogLog.InternalDebugging = true;
        }

        [Test]
        public void TestLogMessage()
        {
            NDC.Push("TestNdc");
            Log.ErrorFormat("This is a test message");
        }
        [Test]
        public void TestMultiLogMessage()
        {
            NDC.Push("TestNdc");
            for (int i = 0; i < 20; ++i)
            {
                Log.DebugFormat("debug message {0}", i);
            }
            Log.Trace("Trace message");
            Log.Debug("Debug message");
            Log.Info("Info message");
            Log.Notice("Notice message");
            Log.InfoFormat("Info message");
            Log.InfoFormat("Info message");
            Log.InfoFormat("This is a\nmultiline message\nthat should have\nhtml breaks");
            Log.InfoFormat("A message with <html> elements that should be rendered as text");
            Log.WarnFormat("Warning message (this should be a triggering message)");

            Log.InfoFormat("Info message");
            Log.ErrorFormat("Error message 1 (a non-trigger error, due to holddown after last warning)");
            Log.InfoFormat("Info message");
            Log.ErrorFormat("Error message 2");
            Log.InfoFormat("Info message");
            Log.Trace("Trace message");
            Log.Debug("Debug message");
            Log.Info("Info message");
            Log.Notice("Notice message");
            Log.Warn("Warn message");
            Log.Error("Error message");
            Log.Fatal("Fatal message");
            Log.ErrorFormat("Error message 3 (second triggering  due to appender being closed)");
        }
    }
}
