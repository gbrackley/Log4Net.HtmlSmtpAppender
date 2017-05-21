/*
 *   (c) Greg Brackley, 2010
 *   
 *   $Id: HtmlSmtpAppender.cs 22 2015-11-09 07:52:47Z greg $ 
 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Xsl;
using log4net.Appender.HtmlSmtp;
using log4net.Appender.HtmlSmtp.Utils;
using log4net.Core;
using log4net.Formatter;
using log4net.Layout;
using log4net.Properties;
using log4net.Util;

namespace log4net.Appender
{
    /// <summary>
    ///   A log4net appender that will send an email to a list of recipients, with
    ///   the messages formatted as HTML.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The following configuration variables are supported:
    ///     <ul>
    ///       <li><see cref = "AppenderSkeleton.ThreshHold" /></li>
    ///       <li><see cref = "LayOut" /></li>
    ///       <li><see cref = "Smtp" /></li>
    ///       <li><see cref = "From" /></li>
    ///       <li><see cref = "To" /></li>
    ///       <li><see cref = "Subject" /></li>
    ///       <li><see cref = "Priority" /></li>
    ///       <li><see cref = "Fix" /></li>
    ///     </ul>
    ///   </para>
    /// </remarks>
    /// <seealso cref = "http://logging.apache.org/log4net/" />
    public class HtmlSmtpAppender : AppenderSkeleton
    {
        public const string IsTriggerLoggingEvent = "IsTrigger";

        /// <summary>
        /// A XSLT parameter with the number of events that have been discarded.
        /// </summary>
        const string LostEventsParameterName = "logging_events_lost";

        private readonly Thread _worker;
        private BlockingCollection<LoggingEvent> _queue;
        private readonly CancellationTokenSource _cancelTokenSource;
        private long _lostEvents;

        public HtmlSmtpAppender()
        {
            _cancelTokenSource = new CancellationTokenSource();
            _worker = new Thread(Worker)
            {
                Name = "HTML SMTP Logger",
                IsBackground = true,
            };
            PreTriggerMessages = 16;
            EventBacklog = 8192;
        }

        /// <summary>
        ///   The SMTP smart host used for mail forwarding.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This parameter takes an URL of the form:
        ///     <code>
        ///       smtp://username:password@hostname:port
        ///     </code>
        ///   </para>
        /// </remarks>
        /// <example>
        ///   <code>
        ///     smtp://smtp.isp.com
        ///   </code>
        /// </example>
        public string Transport { get; set; }

        public string To { get; set; }
        public string From { get; set; }
        public string ReplyTo { get; set; }
        public string Subject { get; set; }
        public MailPriority Priority { get; set; }

        /// <seealso cref = "LoggingEvent.Fix" />
        public virtual FixFlags Fix { get; set; }

        /// <summary>
        ///   The period of time before the next email message is able to be delivered 
        ///   (unless the <see cref = "MaximumEventsPerMessage" /> criteria is exceeded).
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     By default the standard parsing from a XML configuration will set
        ///     this in the units of <b>days</b>. e.g. to set 15 minutes, use a string of
        ///     the form '00:15'.
        ///     <code>
        ///       d | [d.]hh:mm[:ss[.ff]]
        ///     </code>
        ///   </para>
        /// </remarks>
        /// <seealso cref = "TimeSpan.Parse" />
        public virtual TimeSpan HolddownPeriod { get; set; }

        /// <summary>
        ///   The maximum events that should be backlogged in the queue between the event
        ///   source and the SMTP appender worker thread.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This value shouldn't need to be changed unless the logger is under significant
        ///     logging stress.
        ///   </para> 
        ///   <para>
        ///     If the rate at which the SMTP logger can consume events is 
        ///     exceeded then this buffer will overflow and the <see cref = "LostEventsParameterName" />
        ///     paramter to the style sheet will be set with the count of events discarded.
        ///   </para>
        /// </remarks>
        public int EventBacklog { get; set; }

        /// <summary>
        ///   The maximum number of event messages to include in a single message 
        ///   before it is sent (prior to the holddown expiring). When the logger
        ///   is under stress this allows the logger to send messages at rate that
        ///   exceeds the <see cref = "HolddownPeriod" />. It also helps to ensure
        ///   that individual messages don't get too large.
        /// </summary>
        public int MaximumEventsPerMessage { get; set; }

        /// <summary>
        ///   The number of logging events to show prior to the triggering logging event 
        ///   so that the context in which the triggering event can be determined.
        /// </summary>
        public int PreTriggerMessages { get; set; }

        /// <summary>
        ///   The level which causes an email to be sent.
        /// </summary>
        /// <seealso cref = "Threshold" />
        public ITriggeringEventEvaluator Trigger { get; set; }

        public override void ActivateOptions()
        {
            LogLog.Debug(typeof(HtmlSmtpAppender), "HTML SMTP Appender options activated");
            base.ActivateOptions();
            _queue = (EventBacklog > 0)
                ? new BlockingCollection<LoggingEvent>(EventBacklog)
                : new BlockingCollection<LoggingEvent>();

            if (Layout == null)
            {
                var layout = MakeLayout();
                layout.ActivateOptions();
                Layout = layout;
            }
            if (MaximumEventsPerMessage <= 1)
            {
                MaximumEventsPerMessage = 1024;
            }
            if (HolddownPeriod.Ticks <= 0)
            {
                HolddownPeriod = new TimeSpan(0, 15, 0);
            }
            if (Trigger == null)
            {
                Trigger = new LevelEvaluator(Level.Warn);
            }
            if (Subject == null)
            {
                Subject = "Log4net";
            }
            if (string.IsNullOrEmpty(From))
            {
                From = string.Format("log4net <htmlsmtpappender@{0}>", System.Net.Dns.GetHostName());
            }
            if (string.IsNullOrEmpty(Transport))
            {
                Transport = SmtpClientFactory.MakeDefaultTransport();
            }
            if (Fix == FixFlags.None)
            {
                Fix = FixFlags.Partial | FixFlags.Ndc;
            }

            _worker.Start();
        }

     

        protected override void Append(LoggingEvent loggingEvent)
        {
            // Because we are caching the LoggingEvent beyond the lifetime of 
            // the Append() method we must fix any volatile data in the event.
            loggingEvent.Fix = Fix;

            // Try to add it to the worker thread queue. If the arrival rate of 
            // logging events exceeds the delivery rate, then discard the events,
            // but keep a count of the events lost (so that we can report lost
            // events as a summary).
            if (!_queue.TryAdd(loggingEvent))
            {
                Interlocked.Increment(ref _lostEvents);
            }
        }

        protected override void OnClose()
        {
            LogLog.Debug(typeof(HtmlSmtpAppender), "Closing HTML SMTP appender");
            _cancelTokenSource.Cancel();
            if (_worker.IsAlive) // Check that the thread was started successfully
            {
                if (!_worker.Join(60*1000))
                {
                    LogLog.Warn(typeof(HtmlSmtpAppender), "SMTP worker failed to shutdown in a timely manner");
                }
            }
        }

        private class WorkerState
        {
            public WorkerState()
            {
                Buffer = new Queue<LoggingEvent>();
                NonTriggeringEvents = new Queue<LoggingEvent>();
                LastSend = DateTime.MinValue;
            }

            /// <summary>
            ///   The list of events to email
            /// </summary>
            public Queue<LoggingEvent> Buffer { get; private set; }

            // The non-triggering noisy events
            /// <summary>
            ///   A circular queue implemented using a Queue, where items over
            ///   a queue count are manually removed.
            /// </summary>
            /// <remarks>
            ///   Consider migrating to using a 
            ///   <see cref = "http://en.wikipedia.org/wiki/Circular_buffer">circular buffer</see> like
            ///   <see cref = "http://code.google.com/p/ngenerics/wiki/CircularQueue">CircularQueue</see>
            ///   from the nGenerics library.
            /// </remarks>
            /// <seealso cref = "http://code.google.com/p/ngenerics/wiki/CircularQueue" />
            public Queue<LoggingEvent> NonTriggeringEvents { get; private set; }

            // When the last html email message was sent
            public DateTime LastSend { get; set; }
        }

        private void Worker()
        {
            var state = new WorkerState();
            try
            {
                while (!_cancelTokenSource.IsCancellationRequested)
                {
                    LoggingEvent loggingEvent;
                    if (_queue.TryTake(
                        out loggingEvent,
                        ToMillseconds(GetWaitTimeOut(state)),
                        _cancelTokenSource.Token))
                    {
                        AddLoggingEvent(state, loggingEvent);
                    }

                    var now = DateTime.UtcNow;
                    if ((state.Buffer.Count > 0 && (state.LastSend + HolddownPeriod) < now) ||
                        state.Buffer.Count > MaximumEventsPerMessage)
                    {
                        SendEvents(state);
                    } // else we won't sent the events
                }
            }
            catch (OperationCanceledException)
            {
                LogLog.Debug(typeof(HtmlSmtpAppender), "Cancellation request for html smtp worker thread");
                return;
            }
            catch (Exception e)
            {
                LogLog.Error(typeof(HtmlSmtpAppender), string.Format("Unexpected excpetion in html smtp worker: {0}", e.Message), e);
            }
            finally
            {
                // Flush any outstanding events
                LoggingEvent loggingEvent;
                while(_queue.TryTake(out loggingEvent))
                {
                    AddLoggingEvent(state, loggingEvent);
                }

                if (state.Buffer.Count > 0)
                {
                    LogLog.Debug(typeof(HtmlSmtpAppender), "Sending residual html email events");
                    SendEvents(state);
                }

                LogLog.Debug(typeof(HtmlSmtpAppender), "HtmlSmtpAppender worker thread exit");
            }
        }

        public static int ToMillseconds(TimeSpan t)
        {
            var milli = t.Ticks/TimeSpan.TicksPerMillisecond;
            return (milli < int.MaxValue) ? (int) milli : int.MaxValue;
        }

        /// <summary>
        ///   Get the timeout to wait for the next possible logging event, ensuring that
        ///   the time doesn't cause events in the buffer to be sent from being delayed
        ///   once the holddown timeout expires.
        /// </summary>
        private TimeSpan GetWaitTimeOut(WorkerState state)
        {
            DateTime now = DateTime.UtcNow;
            var maxHolddownInterval = TimeSpanUtils.Max(state.LastSend + HolddownPeriod - now, TimeSpan.Zero);
            return new TimeSpan(0, 0, 30);
        }


        private void SendEvents(WorkerState state)
        {
            try
            {
                var now = DateTime.UtcNow;
                long loggingEventsLost = Interlocked.Read(ref _lostEvents);
                SendBuffer(state.Buffer, loggingEventsLost);

                Interlocked.Add(ref _lostEvents, -loggingEventsLost);
                state.Buffer.Clear();
                state.LastSend = now;
            }
            catch (Exception e)
            {
                // Failed to send the email. The buffer will still hold the messages.
                // Although not ideal, the next email will get larger.
                LogLog.Error(typeof(HtmlSmtpAppender), e.Message, e);
            }
        }

        private void AddLoggingEvent(WorkerState state, LoggingEvent loggingEvent)
        {
            var isTrigger = Trigger.IsTriggeringEvent(loggingEvent);
            if (isTrigger)
            {
                // Add a property for the XML so that the transform can 
                // determine which logging events met the triggering criteria.
                loggingEvent.Properties[IsTriggerLoggingEvent] = true;

                // Move the events from the non-triggering event queue to the buffer
                while (state.NonTriggeringEvents.Count > 0)
                {
                    state.Buffer.Enqueue(state.NonTriggeringEvents.Dequeue());
                }
                state.Buffer.Enqueue(loggingEvent);
            }
            else
            {
                // Keep the logging events in a small bounded queue
                while (state.NonTriggeringEvents.Count > 0 &&
                    state.NonTriggeringEvents.Count >= PreTriggerMessages)
                {
                    state.NonTriggeringEvents.Dequeue();
                }
                state.NonTriggeringEvents.Enqueue(loggingEvent);
            }
        }

        private void SendBuffer(
            IEnumerable<LoggingEvent> buffer,
            long loggingEventsLost)
        {
            var xml = MakeBodyXml(Layout, buffer);
            LogLog.Debug(typeof(HtmlSmtpAppender), xml);
            var html = MakeBodyHtml(xml, loggingEventsLost);
            LogLog.Debug(typeof(HtmlSmtpAppender), html);

            SendEmail(
                StringFormatterUtil.Format(Subject, buffer, loggingEventsLost), 
                html);
        }

        public static LayoutSkeleton MakeLayout()
        {
            return new XmlLayout()
            {
                Header = "<?xml version='1.0' ?>\n<log4net><events>\n",
                Footer = "</events></log4net>",
                Prefix = null,
            };
        }

        public static string MakeBodyXml(ILayout layout, IEnumerable<LoggingEvent> buffer)
        {
            TextWriter writer = new StringWriter();
            writer.Write(layout.Header);
            foreach (var loggingEvent in buffer)
            {
                layout.Format(writer, loggingEvent);
            }
            writer.Write(layout.Footer);
            writer.Flush();
            return writer.ToString();
        }


        public static string MakeBodyHtml(string xml, long loggingEventsLost)
        {
            // Load and compile the XSLT
            var transform = new XslCompiledTransform();
            var settings = XsltSettings.TrustedXslt;
            var resolver = new XmlUrlResolver();
            transform.Load(
                XmlReader.Create(new StringReader(Resources.HtmlEmailBody)),
                settings,
                resolver);

            // Feed the lost event count as a parameter (c.f. in the XML content)
            var arguments = new XsltArgumentList();
            arguments.AddParam(LostEventsParameterName, string.Empty, loggingEventsLost);

            // Transform the logging events XML to html
            var htmlWriter = new StringWriter();
            transform.Transform(
                XmlReader.Create(new StringReader(xml)),
                arguments,
                htmlWriter);
            return htmlWriter.ToString();
        }


        private void SendEmail(string subject, string htmlMessageBody)
        {
            // Create and configure the smtp client
            var smtpClient = SmtpClientFactory.ParseConfiguration(Transport);

            var mailMessage = new MailMessage
            {
                From = new MailAddress(From),
                Subject = subject,
            };
            mailMessage.To.Add(To);
            mailMessage.BodyEncoding = Encoding.ASCII;
            var htmlView = AlternateView.CreateAlternateViewFromString(
                htmlMessageBody,
                new ContentType("text/html;charset=utf-8"));

            // Add icons as attachments to the email.
            LinkedResources.AddAttachmentIffGifFound(htmlView, "fatal", "/image/FatalIcon.gif");
            LinkedResources.AddAttachmentIffGifFound(htmlView, "error", "/image/ErrorIcon.gif");
            LinkedResources.AddAttachmentIffGifFound(htmlView, "warn", "/image/WarnIcon.gif");
            LinkedResources.AddAttachmentIffGifFound(htmlView, "notice", "/image/NoticeIcon.gif");
            LinkedResources.AddAttachmentIffGifFound(htmlView, "info", "/image/InfoIcon.gif");
            LinkedResources.AddAttachmentIffGifFound(htmlView, "debug", "/image/DebugIcon.gif");
            LinkedResources.AddAttachmentIffGifFound(htmlView, "trace", "/image/TraceIcon.gif");

            mailMessage.AlternateViews.Add(htmlView);
            if (!string.IsNullOrEmpty(ReplyTo))
            {
                mailMessage.ReplyTo = new MailAddress(ReplyTo);
            }


            smtpClient.Send(mailMessage);
        }

      

    }
}
