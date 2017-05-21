/*
 *   (c) Greg Brackley, 2010
 *   
 *   $Id: TestStringFormatter.cs 12 2010-10-05 09:54:31Z greg $ 
 */
using System;
using log4net;
using log4net.Config;
using log4net.Formatter;
using NUnit.Framework;

namespace HtmlSmtpAppenderUnitTests
{
    [TestFixture]
    public class TestStringFormatter
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (TestStringFormatter).Name);

        [SetUp]
        public void Setup()
        {
            BasicConfigurator.Configure();
        }

        [Test]
        public void TestEmpty()
        {
            var stringFormatter = new StringFormatter();
            var str = stringFormatter.Format("", (name, parameters) => "");

            Assert.AreEqual("", str);
        }

        [Test]
        public void TestEmptyConstant()
        {
            var stringFormatter = new StringFormatter();
            var str = stringFormatter.Format("This is a test", (name, parameters) => "");

            Assert.AreEqual("This is a test", str);
        }

        [Test]
        public void TestPercentage()
        {
            var stringFormatter = new StringFormatter();
            var str = stringFormatter.Format("%%", (name, parameters) => "");

            Assert.AreEqual("%", str);
        }

        [Test]
        public void TestEmbeddedPercentage()
        {
            var stringFormatter = new StringFormatter();
            var str = stringFormatter.Format("GST is 15%%", (name, parameters) => "");

            Assert.AreEqual("GST is 15%", str);
        }

        [Test]
        public void TestSingleNoParameters()
        {
            var stringFormatter = new StringFormatter();
            var str = stringFormatter.Format(
                "Up is %UP",
                (name, parameters) =>
                {
                    if (name == "UP")
                    {
                        Assert.IsNull(parameters);
                        return "down";
                    }
                    throw new ArgumentException(string.Format("The replacement parameter '{0}' is not supported", name));
                });

            Assert.AreEqual("Up is down", str);
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void TestSingleInavalidName()
        {
            var stringFormatter = new StringFormatter();
            stringFormatter.Format(
                "Up is %InvalidName",
                (name, parameters) => { throw new ArgumentException(string.Format("The replacement parameter '{0}' is not supported", name)); });
        }

        /// <summary>
        /// Replacement parameters that don't match pass through without change
        /// </summary>
        [Test]
        public void TestNoAValidName()
        {
            var stringFormatter = new StringFormatter();
            var str = stringFormatter.Format(
                "Name is %#",
                (name, parameters) => { throw new ArgumentException(string.Format("The replacement parameter '{0}' is not supported", name)); });
            Log.Info(str);
            Assert.AreEqual("Name is %#", str);
        }


        [Test]
        public void TestSingleWithParameter()
        {
            var stringFormatter = new StringFormatter();
            var str = stringFormatter.Format(
                "Up is %GO{left}",
                (name, parameters) =>
                {
                    if (name == "GO")
                    {
                        Assert.IsNotNull(parameters);
                        return parameters;
                    }
                    throw new ArgumentException(string.Format("The replacement parameter '{0}' is not supported", name));
                });

            Assert.AreEqual("Up is left", str);
        }

        [Test]
        public void TestSingleWithParameters()
        {
            var stringFormatter = new StringFormatter();
            var str = stringFormatter.Format(
                "Up is %GO{left}, count is %count, size is %size{}",
                (name, parameters) =>
                {
                    if (name == "GO")
                    {
                        Assert.IsNotNull(parameters);
                        return parameters;
                    }
                    else if (name == "count")
                    {
                        return "23";
                    }
                    else if (name == "size")
                    {
                        return "99";
                    }
                    throw new ArgumentException(string.Format("The replacement parameter '{0}' is not supported", name));
                });

            Assert.AreEqual("Up is left, count is 23, size is 99", str);
        }
    }
}
