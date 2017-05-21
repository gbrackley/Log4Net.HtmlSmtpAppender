/*
 *   (c) Greg Brackley, 2010
 *   
 *   $Id: LinkedResources.cs 18 2012-12-20 06:20:43Z greg $ 
 */
using System;
using System.IO;
using System.Net.Mail;
using log4net.Properties;
using log4net.Util;

namespace log4net.Appender.HtmlSmtp
{
    public static class LinkedResources
    {
        public static void AddAttachmentIffGifFound(
            AlternateView view,
            string resourceName,
            string cidName)
        {
            LinkedResource linkedResource = MakeGifResourceAttachment(resourceName, cidName);
            if (linkedResource != null)
            {
                view.LinkedResources.Add(linkedResource);
            }
        }

        private static LinkedResource MakeGifResourceAttachment(
            string resourceName,
            string cidName)
        {
            string resourcePath = string.Format("log4net.Resources.Icons.{0}.gif", resourceName);
            Stream stream = typeof (Resources).Assembly.GetManifestResourceStream(resourcePath);
            if (stream != null)
            {
                return new LinkedResource(stream, "image/gif")
                {
                    ContentLink = new Uri(cidName, UriKind.Relative),
                };
            }
            else
            {
                LogLog.Debug(typeof(LinkedResource), string.Format("Failed to load resource '{0}', the resources are:", resourcePath));
                foreach (string aResourceName in typeof (Resources).Assembly.GetManifestResourceNames())
                {
                    LogLog.Debug(typeof(LinkedResource), aResourceName);
                }
            }
            return null;
        }
    }
}
