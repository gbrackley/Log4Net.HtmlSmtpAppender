/*
 *   (c) Greg Brackley, 2010
 *   
 *   $Id: TestXmlConfigure.cs 12 2010-10-05 09:54:31Z greg $ 
 */
using System.Xml;
using log4net.Config;
using log4net.Util;
using NUnit.Framework;

namespace HtmlSmtpAppenderIntegrationTests
{
    [TestFixture]
    public class TestXmlConfigure
    {
        [Test]
        public void TestLoadConfiguration()
        {
            LogLog.InternalDebugging = true;

            var doc = new XmlDocument();
            doc.LoadXml(@"<?xml version=""1.0"" encoding=""iso-8859-1""?>
                   <log4net>
                        <root>
                          <level value='TRACE' />
                          <appender-ref ref='HtmlSmtpAppender' />
                        </root>

                        <appender name='HtmlSmtpAppender' type='log4net.Appender.HtmlSmtpAppender, HtmlSmtpAppender'>
                          <To value='homer@simpson.com' />
                          <From value='marg@simpson.com' />
                          <ReplyTo value='bart@simpson.com' />
                          <Subject value='Message' />
                          <Transport value='smtp://smtp.simpson.com' />
                          <HolddownPeriod value='00:15' />
                          <MaximumEventsPerMessage value='4096' />
                          <EventBacklog value='4096' />
                          <PreTriggerMessages value='32' />
                          <Trigger type='log4net.Core.LevelEvaluator'>
                             <Threshold value='NOTICE' />
                          </Trigger>
                          <Threshold value='DEBUG' />
                          <Layout type='log4net.Layout.XmlLayout' >
                              <header value='&lt;?xml version=&quot;1.0&quot; ?&gt;&lt;log4net&gt;&lt;events&gt;' />
                              <footer value='&lt;/events&gt;&lt;/log4net&gt;' />
                          </Layout>
                        </appender>
                    </log4net>
                ");

            XmlConfigurator.Configure(doc.DocumentElement);
        }


        [Test]
        public void TestLoadConfigurationPatternStringSubject()
        {
            LogLog.InternalDebugging = true;

            var doc = new XmlDocument();
            doc.LoadXml(@"<?xml version='1.0' encoding='iso-8859-1'?>
                   <log4net>
                        <root>
                          <level value='TRACE' />
                          <appender-ref ref='HtmlSmtpAppender' />
                        </root>

                        <appender name='HtmlSmtpAppender' type='log4net.Appender.HtmlSmtpAppender, HtmlSmtpAppender'>
                          <To value='homer@simpson.com' />
                          <From value='marg@simpson.com' />
                          <Subject type='log4net.Util.PatternString' value='Message %count{level.error} %env{ProgramData}' />
                        </appender>
                    </log4net>
                ");

            XmlConfigurator.Configure(doc.DocumentElement);
        }
    }
}
