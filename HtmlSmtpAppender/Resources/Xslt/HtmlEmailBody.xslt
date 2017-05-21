<?xml version="1.0" encoding="iso-8859-1"?>
<!--

  A stylesheet to convert Log4Net XmlLayout xml to html suitable
  for inclusion in a html email.

  (c) Greg Brackley, 2010
   
  $Id: HtmlEmailBody.xslt 14 2011-01-18 01:40:06Z greg $ 

-->
<xsl:stylesheet	
  version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
  xmlns:ms="urn:schemas-microsoft-com:xslt"
  xmlns:dt="urn:schemas-microsoft-com:datatypes">
  
  <xsl:output method="html" indent="yes" encoding="US-ASCII" />
  <xsl:decimal-format decimal-separator="." grouping-separator="," />

  <xsl:template match="/">
    <html>
      <head>
        <style type="text/css">
             .bannercell {
          border: 0px;
          padding: 0px;
          }
          body {
          margin-left: 10;
          margin-right: 10;
          font:normal 80% arial,helvetica,sanserif;
          background-color:#FFFFFF;
          color:#000000;
          }
          .a td {
          background: #efefef;
          }
          .b td {
          background: #fff;
          }
          th, td {
          text-align: left;
          vertical-align: top;
          }
          th {
          font-weight:bold;
          background: #ccc;
          color: black;
          }
          table, th, td {
          font-size:100%;
          border: none
          }
          table.log tr td, tr th {

          }
          h2 {
          font-weight:bold;
          font-size:140%;
          margin-bottom: 5;
          }
          h3 {
          font-size:100%;
          font-weight:bold;
          background: #525D76;
          color: white;
          text-decoration: none;
          padding: 5px;
          margin-right: 2px;
          margin-left: 2px;
          margin-bottom: 0;
          }

        </style>
      </head>
      <body>
       
        <!-- Summary part -->
        <xsl:apply-templates select="log4net" mode="summary" />
        <hr size="1" width="100%" align="left"/>

        <!-- Event Logging List -->
        <xsl:apply-templates select="log4net" mode="list" />
        <hr size="1" width="100%" align="left"/>
      </body>
    </html>
  </xsl:template>
  
  <xsl:template match="events" mode="summary">
    <h3>Messages</h3>
    <table class="log" border="0" cellpadding="5" cellspacing="2" width="100%">
      <xsl:call-template name="logging-event-header"/>
      <xsl:apply-templates select="event[ properties/data [ @name = 'IsTrigger' ][ @value = 'True' ] ]"  />
    </table>
  </xsl:template>

  <xsl:template match="events" mode="list">
    <h3>All Messages</h3>
    <table class="log" border="0" cellpadding="5" cellspacing="2" width="100%">
      <xsl:call-template name="logging-event-header"/>
      <xsl:apply-templates select="event" />
    </table>
  </xsl:template>

  <xsl:template name="logging-event-header">
    <tr>
      <th title="The log message level (FATAL, ERROR, WARN, NOTICE, INFO, DEBUG, TRACE)">Level</th>
      <th title="The UTC timestamp for the logging event">Time</th>
      <th title="The Log4net nested diagnostic context">Context</th>
      <th>Message</th>
    </tr>
  </xsl:template>

  <xsl:template match="event">
    <tr>
      <xsl:call-template name="alternated-row"/>
      
      <!-- Column 1: Level -->
      <td>
        <xsl:call-template name="event-level"/>
      </td>
      <!-- Column 2: Timestamp -->
      <td>
        <!--
              Note: Because the MS XSLT processor is v1.0 only, the following statement
              can't be used:
                 <xsl:value-of select="xs:datetime(@timestamp)"/>
              Instead we use the Microsoft specific 'format-time' function.
              
              see http://msdn.microsoft.com/en-us/library/ms256099.aspx
            -->
        <xsl:attribute name="title">
          <xsl:value-of 
            select="concat(
              ms:format-date(@timestamp, 'yyyy-MM-dd'), 
              ' ', 
              ms:format-time(@timestamp, 'HH:mm:ss'), 
              ' UTC')"/>
        </xsl:attribute>
        <xsl:value-of select="ms:format-time(@timestamp, 'HH:mm:ss')"/>
      </td>
      <!-- Column 3: Nested Diagnostic Context -->
      <td>
        <xsl:value-of select="properties/data[@name='NDC']/@value"/>
      </td>
      <!-- Column 4: Message Text -->
      <td>
        <xsl:apply-templates select="message"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template name="alternated-row">
    <xsl:attribute name="class">
      <xsl:if test="position() mod 2 = 1">a</xsl:if>
      <xsl:if test="position() mod 2 = 0">b</xsl:if>
    </xsl:attribute>
  </xsl:template>

  <xsl:template name="event-level">
    <xsl:choose>
      <xsl:when test="@level = 'FATAL'">
        <img src="/image/FatalIcon.gif" alt="F" title="Fatal"/>
      </xsl:when>
      <xsl:when test="@level = 'ERROR'">
        <img src="/image/ErrorIcon.gif" alt="E" title="Error"/>
      </xsl:when>
      <xsl:when test="@level = 'WARN'">
        <img src="/image/WarnIcon.gif" alt="W" title="Warning"/>
      </xsl:when>
      <xsl:when test="@level = 'NOTICE'">
        <img src="/image/NoticeIcon.gif" alt="N" title="Notice"/>
      </xsl:when>
      <xsl:when test="@level = 'INFO'">
        <img src="/image/InfoIcon.gif" alt="I" title="Information"/>
      </xsl:when>
      <xsl:when test="@level = 'DEBUG'">
        <img src="/image/DebugIcon.gif" alt="D" title="Debug"/>
      </xsl:when>
      <xsl:when test="@level = 'TRACE'">
        <span title="Trace">&#160;</span>
      </xsl:when>
      <xsl:otherwise>
        <span>
          <xsl:attribute name="title">
            <xsl:value-of select="@level"/>
          </xsl:attribute>
          &#160;
        </span>
      </xsl:otherwise>
      
    </xsl:choose>
  </xsl:template>

<!--
  <xsl:template match="node()|@*">
    <xsl:copy>
      <xsl:apply-templates select="node()|@*"/>
    </xsl:copy>
  </xsl:template>
-->

  <xsl:template match="text()" name="replaceNL">
    <xsl:param name="pText" select="."/>

    <xsl:choose>
      <xsl:when test="contains($pText, '&#xA;')">
        <xsl:value-of select="substring-before($pText, '&#xA;')"/>
        <br />
        <xsl:call-template name="replaceNL">
          <xsl:with-param name="pText" select="substring-after($pText, '&#xA;')"/>
        </xsl:call-template>
      </xsl:when>
      
      <xsl:otherwise>
        <xsl:value-of select="$pText"/>
      </xsl:otherwise>
      
    </xsl:choose>
  </xsl:template>

</xsl:stylesheet>
  
  
  