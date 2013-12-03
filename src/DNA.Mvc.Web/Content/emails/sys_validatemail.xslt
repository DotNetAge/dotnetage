<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes" encoding="utf-8" />
  <xsl:template match="/">
    <html>
      <head>
        <title></title>
      </head>
      <body>
        <xsl:for-each select="//modal">
          <p>
            Hi <xsl:value-of select="to" />:
          </p>
          <p>
            <xsl:element name="a">
              <xsl:attribute name="target">
                <xsl:text>_blank</xsl:text>
              </xsl:attribute>
              <xsl:attribute name="href" >
                <xsl:value-of select="confirmUrl"/>
              </xsl:attribute>
              <xsl:text>Click here to confirm your email address.</xsl:text>
            </xsl:element>
          </p>
          <p>If clicking the link above does not work, you can type or copy and paste this URL into your browser.</p>
          <p>
            <xsl:value-of select="confirmUrl"/>
          </p>
          <div>Thanks,</div>
          <div>
            <xsl:value-of select="web"/>
          </div>
        </xsl:for-each>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
