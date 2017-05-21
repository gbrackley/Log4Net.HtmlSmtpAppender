/*
 *   (c) Greg Brackley, 2010
 *   
 *   $Id: TestGetDomainName.cs 8 2010-09-30 01:59:10Z greg $ 
 */
using log4net;
using log4net.Appender.HtmlSmtp;
using log4net.Appender.HtmlSmtp.Utils;
using log4net.Config;
using NUnit.Framework;
using Dns = System.Net.Dns;

namespace HtmlSmtpAppenderUnitTests
{
    [TestFixture]
    public class TestGetDomainName
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TestMakeXmlLoggingEvents).Name);

        [SetUp]
        public void Setup()
        {
            BasicConfigurator.Configure();
        }

        [Test]
        public void TestParseDomainNameHostNameOnly()
        {
            Assert.AreEqual("", DnsUtils.GetDomainName("bob"));
        }

        [Test]
        public void TestParseDomainNameHostNameWithTrailingDot()
        {
            Assert.AreEqual("", DnsUtils.GetDomainName("bart."));
        }

        [Test]
        public void TestParseDomainNameFullyQualifiedDomainName()
        {
            Assert.AreEqual("simpson.com", DnsUtils.GetDomainName("homer.simpson.com"));
        }

        [Test]
        public void TestParseLocalName()
        {
            Assert.AreEqual("local", DnsUtils.GetDomainName("host.local"));
        }


        /// <summary>
        /// The machine running the test <b>must</b> have a DNS domain name configured for this
        /// test to complete successfully.
        /// </summary>
        [Test]
        public void TestParseLocalDomainName()
        {
            Log.InfoFormat("domain is '{0}'", DnsUtils.GetDomainName());
            Assert.IsNotEmpty(DnsUtils.GetDomainName());
        }
    }
}
