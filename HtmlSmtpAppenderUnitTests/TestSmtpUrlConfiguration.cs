/*
 *   (c) Greg Brackley, 2010
 *   
 *   $Id: TestSmtpUrlConfiguration.cs 12 2010-10-05 09:54:31Z greg $ 
 */
using System.Text.RegularExpressions;
using log4net.Appender.HtmlSmtp;
using log4net.Appender.HtmlSmtp.Utils;
using NUnit.Framework;

namespace HtmlSmtpAppenderUnitTests
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="HtmlSmtp.ConfigUrlMatch"/>
    [TestFixture]
    public class TestSmtpUrlConfiguration
    {
        [Test]
        public void TestEmpty()
        {
            var match = SmtpClientFactory.MatchConfig("");
            Assert.IsTrue(match.Success);
        }

        [Test]
        public void TestSimpleFile()
        {
            var match = SmtpClientFactory.MatchConfig("file:c:\\maildrop\\");
            Assert.IsTrue(match.Success);
            Assert.AreEqual("file", match.GetSingletonCapture("scheme"));
            Assert.AreEqual(@"c:\maildrop\", match.GetSingletonCapture("path"));
        }

        [Test]
        public void TestSimpleFilePath()
        {
            var match = SmtpClientFactory.MatchConfig("file:/my/mydrop/queue");
            Assert.IsTrue(match.Success);
            Assert.AreEqual("file", match.GetSingletonCapture("scheme"));
        }

        [Test]
        public void TestSimpleSmtp()
        {
            var match = SmtpClientFactory.MatchConfig("smtp://smtp.domain.com");
            Assert.IsTrue(match.Success);
            Assert.AreEqual("smtp", match.GetSingletonCapture("scheme"));
        }

        [Test]
        public void TestSimpleSmtps()
        {
            var match = SmtpClientFactory.MatchConfig("smtps://smtp.domain.com");
            Assert.IsTrue(match.Success);
            Assert.AreEqual("smtps", match.GetSingletonCapture("scheme"));
        }


        [Test]
        public void TestSmtp1()
        {
            var match = SmtpClientFactory.MatchConfig("smtp://uuuu:pppp@smtp.domain.com:smtp?authentication=ntlm");
            Assert.IsTrue(match.Success);
            Assert.AreEqual("smtp", match.GetSingletonCapture("scheme"));
            Assert.AreEqual("smtp.domain.com", match.GetSingletonCapture("host"));
            Assert.AreEqual("smtp", match.GetSingletonCapture("service"));
            Assert.AreEqual("uuuu", match.GetSingletonCapture("username"));
            Assert.AreEqual("pppp", match.GetSingletonCapture("password"));
        }
    }
}
