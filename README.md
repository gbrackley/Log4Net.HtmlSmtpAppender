# HtmlSmtpAppender

A SMTP appender for Log4Net that will send a small history of messages when a threshold is reached.

The appender works by keeping a ring buffer of the last few messages. When a triggering log
message is received (e.f warning or more severe), then the entire buffer including the triggering
message is emailed to the recipients. This means the previous messages with context information 
will be received.

So as to not flood recipients with emails, a hysteresis is introduced so that subsequent triggering
events are batched up into the next delivery window.  

# Configuration

Add an appender to you log4net configuration:

 ```
    <appender name="HtmlSmtpAppender" type="log4net.Appender.HtmlSmtpAppender, HtmlSmtpAppender">
        <to value="user@example.com" />
        <from value="smtp-appender@example.com" />
        <subject value="[Program] %events{triggering} of %events{total} [%events{class.unrecoverable},%events{class.recoverable},%events{class.information},%events{class.debug}] (lost %events{lost})" />
        <transport value="smtp://localhost" />
    </appender>
    <root>
        <appender-ref ref="HtmlSmtpAppender" />
    </root>

 ```
 
 The transport setting is a URI of the form `smtp://username:password@hostname:port`