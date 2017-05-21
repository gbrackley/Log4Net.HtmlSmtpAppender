/*
 *   (c) Greg Brackley, 2010
 *   
 *   $Id: TestInstantiate.cs 7 2010-09-29 05:55:12Z greg $ 
 */
using NUnit.Framework;

namespace HtmlSmtpAppenderUnitTests
{
    [TestFixture]
    public class TestInstantiate
    {
        [Test]
        public void TestInstantiateAppender()
        {
            var appender = new log4net.Appender.HtmlSmtpAppender();
            Assert.IsNotNull(appender);
        }

        /// <summary>
        /// This tests the <b>unactivated</b> defaults
        /// </summary>
        [Test]
        public void TestInstantiateAppenderDefaults()
        {
            var appender = new log4net.Appender.HtmlSmtpAppender();
            Assert.IsNotNull(appender);
            Assert.IsNull(appender.From);
            Assert.IsNull(appender.To);
            Assert.IsNull(appender.Transport);
        }


        [Test]
        public void TestInstantiateAndActicvateAppenderSampleManditoryFields()
        {
            var appender = new log4net.Appender.HtmlSmtpAppender()
            {
                To = "homer@simpson.com",
                From = "homer@simpson.com",
            };
            appender.ActivateOptions();

            Assert.IsNotNull(appender);
        }
    }
}
