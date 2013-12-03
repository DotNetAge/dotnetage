<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes" encoding="utf-8" />
  <xsl:template match="/">
    <html>
      <head>
        <title>Password changed</title>
        <style type="text/css">blockquote { border-left: 15px solid #cccccc; background-color: #eeeeee; padding: 5px; }</style>
      </head>
      <body>
        <xsl:for-each select="//User">
          <p>
            Hi <xsl:value-of select="Name"/>:
          </p>
          <p>
            Your password has been changed:
          </p>
          <blockquote>
            <p>
              <b>User:</b>
              <i>
                <xsl:value-of select="Name"/>
              </i>
            </p>
            <p>
              <b>Password:</b>
              <i>
                <xsl:value-of select="Password"/>
              </i>
            </p>
          </blockquote>
        </xsl:for-each>
        You should use this username and password to <a>login</a> our website next time.
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
