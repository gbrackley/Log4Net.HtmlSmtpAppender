/*
 *   (c) Greg Brackley, 2010
 *   
 *   $Id: TestQueryMxRecords.cs 9 2010-09-30 02:00:16Z greg $ 
 */
using System.Collections.Generic;
using System.ComponentModel;
using log4net;
using log4net.Appender.HtmlSmtp.Utils;
using log4net.Config;
using NUnit.Framework;
using System.Linq;

namespace HtmlSmtpAppenderUnitTests
{
    /// <summary>
    /// These tests perform basic exercise tests. The results from public 
    /// domains (like google) are likely to return geographic based results,
    /// so these tests aren't very restrictive.
    /// </summary>
    [TestFixture]
    public class TestQueryMxRecords
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TestQueryMxRecords).Name);

        [SetUp]
        public void Setup()
        {
            BasicConfigurator.Configure();
        }

        [Test]
        public void TestQueryMxGoogle()
        {
            const string domain = "google.com";
            Log.InfoFormat("Query domain '{0}'", domain);
            var mailExchangers = DnsQuery.QueryMx(domain);
            LogMxs(domain, mailExchangers);

            Assert.IsNotNull(mailExchangers);
            Assert.AreEqual(5, mailExchangers.Count());
        }

        [Test]
        public void TestQueryMxTrademe()
        {
            const string domain = "trademe.co.nz";
            Log.InfoFormat("Query domain '{0}'", domain);
            var mailExchangers = DnsQuery.QueryMx(domain);
            LogMxs(domain, mailExchangers);

            Assert.IsNotNull(mailExchangers);
            Assert.AreEqual(mailExchangers.Count(), 8);
        }

        [Test]
        public void TestQueryMxSlashdot()
        {
            const string domain = "slashdot.org";
            Log.InfoFormat("Query domain '{0}'", domain);
            var mailExchangers = DnsQuery.QueryMx(domain);
            LogMxs(domain, mailExchangers);

            Assert.IsNotNull(mailExchangers);
            Assert.AreEqual(mailExchangers.Count(), 1);
        }

        [Test]
        [ExpectedException(typeof(Win32Exception), ExpectedMessage="DNS name does not exist")]
        public void TestQueryInvalidDomain()
        {
            const string domain = "thisdomaindoesnotexist123456.net";
            DnsQuery.QueryMx(domain);
        }

        [Test]
        public void TestDoubleQueryMxTrademe()
        {
            const string domain = "trademe.co.nz";
            var mailExchangersQuery1 = DnsQuery.QueryMx(domain);
            var mailExchangersQuery2 = DnsQuery.QueryMx(domain);
            Log.Info("First");
            LogMxs(domain, mailExchangersQuery1);
            Log.Info("Second");
            LogMxs(domain, mailExchangersQuery2);

            Assert.IsNotNull(mailExchangersQuery1);
            Assert.IsNotNull(mailExchangersQuery2);
            Assert.AreEqual(mailExchangersQuery1.Count(), mailExchangersQuery2.Count);
        }

        private static void LogMxs(string domain, IEnumerable<MxRecord> mailExchangers)
        {
            Log.InfoFormat("{0} mail exchanger(s) for domain '{1}'", mailExchangers.Count(), domain);
            foreach (var mailExchanger in mailExchangers)
            {
                Log.InfoFormat(" MX {0} [{1}]", mailExchanger.Name, mailExchanger.Preference);
            }
        }
    }
}
