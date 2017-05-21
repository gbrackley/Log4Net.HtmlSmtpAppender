/*
 *   (c) Greg Brackley, 2010
 *   
 *   $Id: PrettyPrintXml.cs 7 2010-09-29 05:55:12Z greg $ 
 */
using System;
using System.IO;
using System.Text;
using System.Xml;
using log4net;

namespace HtmlSmtpAppenderUnitTests.Utils
{
    public static class PrettyPrintXml
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (PrettyPrintXml).Name);

        /// <summary>
        /// Format an XML string as pretty formatted XML (usually for diagnostic logging).
        /// </summary>
        /// <seealso cref="http://www.personalmicrocosms.com/Pages/dotnettips.aspx?c=14&t=16"/>
        public static String ToString(String xml)
        {
            var document = new XmlDocument();
            try
            {
                // Load the XmlDocument with the XML.
                document.LoadXml(xml);
                return ToString(document);
            }
            catch (XmlException e)
            {
                Log.ErrorFormat("Error formatting XML : {0}", e.Message);
            }
            return string.Empty;
        }

        public static string ToString(XmlDocument document)
        {
            return ToString(document, Encoding.Unicode);
        }

        public static string ToString(XmlDocument document, Encoding encoding)
        {
            var stream = new MemoryStream();
            var writer = new XmlTextWriter(stream, encoding)
            {
                Formatting = Formatting.Indented
            };
            try
            {
                // Write the XML into a formatting XmlTextWriter
                document.WriteContentTo(writer);
                writer.Flush();
                stream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                stream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                var streamReader = new StreamReader(stream);

                // Extract the text from the StreamReader.
                return streamReader.ReadToEnd();
            }
            finally
            {
                stream.Close();
                writer.Close();
            }
        }
    }
}
