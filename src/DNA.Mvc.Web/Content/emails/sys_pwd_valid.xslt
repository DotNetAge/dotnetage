<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes" encoding="utf-8" />
  <xsl:template match="/">
    <html>
      <head>
        <title>Password validation mail</title>
      </head>
      <body>
        <p>
          Hi {username}:
        </p>
        <p>
          You send a password receive request just now.Our security policy need to validate
          your request.
        </p>
        <p>
          If you not validate this request, it will be timeout and unvalidable at
        </p>
        <p>
          Please click the following link to validate your request:<a href="{validationUrl}">{validationUrl}</a>
        </p>
      </body>
    </html>

  </xsl:template>
</xsl:stylesheet>
