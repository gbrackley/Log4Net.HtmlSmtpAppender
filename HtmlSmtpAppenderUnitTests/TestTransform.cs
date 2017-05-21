/*
 *   (c) Greg Brackley, 2010
 *   
 *   $Id: TestTransform.cs 8 2010-09-30 01:59:10Z greg $ 
 */
using log4net;
using log4net.Config;
using NUnit.Framework;

namespace HtmlSmtpAppenderUnitTests
{
    [TestFixture]
    public class TestTransform
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TestTransform).Name);
        
        [SetUp]
        public void Logging()
        {
            BasicConfigurator.Configure();
        }


        private const string SampleXml = @"<?xml version=""1.0""?>
                <log4net>
                  <events>
                    <event logger="""" timestamp=""2010-09-24T19:52:50.031077+12:00"" level=""CRITICAL"" thread=""None"" domain=""IsolatedAppDomainHost: HtmlSmtpAppenderUnitTests"" username=""balls\greg"">
                      <message>This is a sample first message</message>
                      <properties>
                        <data name=""log4net:HostName"" value=""balls"" />
                      </properties>
                    </event>
                    <event logger="""" timestamp=""2010-09-24T19:52:50.0320771+12:00"" level=""WARN"" thread=""None"" domain=""IsolatedAppDomainHost: HtmlSmtpAppenderUnitTests"" username=""balls\greg"">
                      <message>This is a sample second message</message>
                      <properties>
                        <data name=""log4net:HostName"" value=""balls"" />
                      </properties>
                    </event>
                    <event logger=""TestMakeXmlLoggingEvents"" timestamp=""2010-09-24T19:52:50.0320771+12:00"" level=""EMERGENCY"" thread=""7"" domain=""IsolatedAppDomainHost: HtmlSmtpAppenderUnitTests"" username=""balls\greg"">
                      <message>This 
is the third sample 
message.</message>
                      <properties>
                        <data name=""log4net:HostName"" value=""balls"" />
                      </properties>
                      <exception>System.ArgumentException: Sample exception</exception>
                    </event>
                  </events>
                </log4net>
                ";



        [Test]
        public void TestTransformXmlToHtml()
        {
           var html = log4net.Appender.HtmlSmtpAppender.MakeBodyHtml(SampleXml, 7);
            Log.Info(html);
            Assert.IsNotEmpty(html);
        }
    }
}
