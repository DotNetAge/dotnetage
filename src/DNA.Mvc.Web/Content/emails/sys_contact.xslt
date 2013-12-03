<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" 
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
                exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes" encoding="utf-8" />
  <xsl:template match="/">
    <html>
      <body>
        <xsl:for-each select="//modal">
          <p>
            Hi <xsl:value-of select="toName" />:
          </p>
          <p>
            <xsl:value-of select="fromName" /> ( <xsl:value-of select="from" /> ) send you a message via <xsl:value-of select="appUrl" />
          </p>
          <p>
            <xsl:value-of select="message" />
          </p>
        </xsl:for-each>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
