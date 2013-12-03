//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Xml;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace DNA.Web
{
    public abstract class PackageBase<T>
        where T : class
    {
        private XmlDocument configuationDocument = null;
        private NameTable nt = new NameTable();
        private XmlNamespaceManager nsmgr = null;
        private NameValueCollection _params = new NameValueCollection();
        private string prefixname = "c";
        private string configXml = "";
        private T model = null;
        protected string configurationFile = "";
        protected string configurationFileName = "config.xml";

        public PackageBase(string path)
        {
            Init(path);
        }

        protected virtual void Init(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException(path);
            var curDir = new DirectoryInfo(path);
            Name = curDir.Name;
            this.InstalledPath = path;
            configurationFile = path + "\\" + configurationFileName;
            if (!File.Exists(configurationFile))
                throw new FileNotFoundException("Package \"" + Name + "\" configuration file not found");
        }

        /// <summary>
        /// Gets/Sets the package name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the packages installed file path
        /// </summary>
        public string InstalledPath { get; protected set; }

        /// <summary>
        /// Getes the packages relative url
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Get the localized mode by specified language
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public abstract T Locale(string lang);

        /// <summary>
        /// Gets the Model contains in this package
        /// </summary>
        public virtual T Model
        {
            get
            {
                if (model == null)
                {
                    using (var stream = File.OpenRead(configurationFile))
                    {
                        model = this.Deserialize(stream);
                    }
                }
                return model;
            }
        }

        /// <summary>
        /// Gets the package configuation file
        /// </summary>
        public virtual string ConfigXml
        {
            get
            {
                if (string.IsNullOrEmpty(configXml))
                {
                    using (var reader = File.OpenText(configurationFile))
                    {
                        configXml = reader.ReadToEnd();
                        configXml = ReplaceParams(configXml);
                    }
                }
                return configXml;
            }
        }

        protected string ReplaceParams(string text)
        {
            var formattedText = text;
            if (this._params.Count > 0)
            {
                foreach (string key in this._params.Keys)
                {
                    var regex = new Regex("\\{" + key + "\\}", RegexOptions.Singleline);
                    formattedText = Format(regex, formattedText, this._params[key]);
                }
            }
            return formattedText;
        }

        private string Format(Regex regex, string text, string replacement)
        {
            var match = regex.Match(text);
            int startIndex = 0;
            int len = text.Length;
            string formatted = "";

            while (match.Success)
            {
                if (match.Index > 0)
                {
                    int length = match.Index - startIndex;
                    formatted += text.Substring(startIndex, length);
                }

                var target = text.Substring(match.Index, match.Length);

                formatted += regex.Replace(target, replacement);
                startIndex = (match.Index + match.Length);
                match = match.NextMatch();
            }

            if (startIndex != (len - 1))
                formatted += text.Substring(startIndex);

            if (!string.IsNullOrEmpty(formatted))
                return formatted;
            return text;
        }

        /// <summary>
        /// Receive all support language of current widget.
        /// </summary>
        /// <returns></returns>
        public string[] GetSupportLanguages()
        {
            var element = XElement.Parse(ConfigXml);
            var languages = new List<string>();
            var defaultLocale = element.StrAttr("defaultLocale");
            var elements = element.Descendants().Where(e => e.HasAttributes && e.Attribute(XNamespace.Xml + "lang") != null);

            if (string.IsNullOrEmpty(defaultLocale))
                defaultLocale = "en-us";

            if (elements != null && elements.Count() > 0)
            {
                foreach (var e in elements)
                {
                    if (!languages.Contains(e.Attribute(XNamespace.Xml + "lang").Value.ToString()))
                        languages.Add(e.Attribute(XNamespace.Xml + "lang").Value);
                }
            }

            languages.Add(defaultLocale);
            //var targetPath = InstalledPath + "\\locales";
            //if (Directory.Exists(targetPath))
            //{
            //    var dir = new DirectoryInfo(targetPath);
            //    var dirs = dir.GetDirectories();
            //    foreach (var d in dirs)
            //        languages.Add(d.Name);
            //}
            return languages.ToArray();
        }

        public virtual string DefaultNamespace
        {
            get
            {
                return "";
            }
        }

        public virtual string PrefixName
        {
            get
            {
                return prefixname;
            }
            set
            {
                prefixname = value;
            }
        }

        public void AddParam(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if (this._params.AllKeys.Contains(name))
                throw new Exception("The key : " + name + " already in param");

            this._params.Add(name, value);

            this.configXml = null;
        }

        public void RemoveParam(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (!this._params.AllKeys.Contains(name))
                throw new Exception("The key : " + name + " not in param");

            this._params.Remove(name);
        }

        public NameValueCollection GetParams()
        {
            return _params;
        }

        public XmlNamespaceManager NamespaceManager
        {
            get
            {
                if (nsmgr == null)
                {
                    if (!string.IsNullOrEmpty(DefaultNamespace))
                    {
                        nsmgr = new XmlNamespaceManager(nt);
                        nsmgr.AddNamespace(PrefixName, DefaultNamespace);
                    }
                }
                return nsmgr;
            }
        }

        public XmlDocument ConfiguationDocument
        {
            get
            {
                if (configuationDocument == null)
                {
                    configuationDocument = new XmlDocument();
                    configuationDocument.LoadXml(ConfigXml);
                }
                return configuationDocument;
            }
        }

        public bool HasSigned
        {
            get
            {
                return File.Exists(InstalledPath + "\\signed.key");
            }
        }

        /// <summary>
        /// Evaluates an XPath data-binding expression.
        /// </summary>
        /// <param name="expression">The XPath expression to evaluate. </param>
        /// <returns>An object that results from the evaluation of the data-binding expression.</returns>
        public XmlNode XPath(string expression)
        {
            if (!string.IsNullOrEmpty(DefaultNamespace))
                return this.ConfiguationDocument.SelectSingleNode(expression, NamespaceManager);
            else
                return ConfiguationDocument.SelectSingleNode(expression);
        }

        #region configuation file helper methods
        public XmlNodeList XPathSelect(string expression)
        {
            if (!string.IsNullOrEmpty(DefaultNamespace))
                return configuationDocument.SelectNodes(expression, nsmgr);
            else
                return configuationDocument.SelectNodes(expression);
        }

        public XmlNode GetLocalizedNode(string tagName, string lang)
        {
            return XPath(string.Format("//{0}:{1}[lang(\"{2}\")]", PrefixName, tagName, lang));
        }

        protected LocalizableElement GetLocalizableElement(string tagName, string lang)
        {
            return GetLocalizableElement(GetLocalizedNode(tagName, lang));
        }

        protected LocalizableElement GetLocalizableElement(XmlNode node)
        {
            if (node != null)
            {
                return new LocalizableElement()
                {
                    Language = node.Attributes["lang"] != null ? node.Attributes["lang"].Value : "",
                    Direction = node.Attributes["dir"] != null ? node.Attributes["dir"].Value : "ltr",
                    Text = node.InnerText
                };
            }
            return null;
        }

        protected IconElement GetIconElement(string tagName, string lang)
        {
            return GetIconElement(GetLocalizedNode(tagName, lang));
        }

        protected IconElement GetIconElement(XmlNode node)
        {
            if (node != null)
            {
                return new IconElement()
                {
                    Language = node.Attributes["lang"] != null ? node.Attributes["lang"].Value : "",
                    Direction = node.Attributes["dir"] != null ? node.Attributes["dir"].Value : "ltr",
                    Height = node.Attributes["height"] != null ? int.Parse(node.Attributes["height"].Value) : 0,
                    Width = node.Attributes["width"] != null ? int.Parse(node.Attributes["width"].Value) : 0,
                    Source = node.Attributes["src"] != null ? node.Attributes["src"].Value : ""
                };
            }
            return null;
        }

        protected ImageElement GetImageLinkElement(string tagName, string lang)
        {
            return GetImageLinkElement(this.GetLocalizedNode(tagName, lang));
        }

        protected ImageElement GetImageLinkElement(XmlNode node)
        {
            if (node != null)
            {
                return new ImageElement()
                {
                    Language = node.Attributes["lang"] != null ? node.Attributes["lang"].Value : "",
                    Direction = node.Attributes["dir"] != null ? node.Attributes["dir"].Value : "ltr",
                    Source = node.Attributes["src"] != null ? node.Attributes["src"].Value : ""
                };
            }
            return null;
        }

        protected LicenseElement GetLicenseElement(string tagName, string lang)
        {
            return GetLicenseElement(this.GetLocalizedNode(tagName, lang));

        }

        protected LicenseElement GetLicenseElement(XmlNode node)
        {
            if (node != null)
            {
                return new LicenseElement()
                {
                    Language = node.Attributes["lang"] != null ? node.Attributes["lang"].Value : "",
                    Direction = node.Attributes["dir"] != null ? node.Attributes["dir"].Value : "ltr",
                    Source = node.Attributes["src"] != null ? node.Attributes["src"].Value : "",
                    Text = node.InnerText
                };
            }
            return null;
        }

        #endregion

        public virtual string ResolveUri(string relativeFileName, string lang = "")
        {
            if (string.IsNullOrEmpty(relativeFileName))
                throw new ArgumentNullException("relativeFileName");

            if (!relativeFileName.StartsWith("http://") && !relativeFileName.StartsWith("ftp://") && !relativeFileName.StartsWith("https://"))
            {
                if (string.IsNullOrEmpty(lang) || relativeFileName.StartsWith("./"))
                {
                    return BaseUrl + Name + "/" + (relativeFileName.StartsWith("./") ? relativeFileName.Substring(2, relativeFileName.Length - 2) : relativeFileName);
                }
                else
                    return BaseUrl + Name + "/locales/" + lang + "/" + relativeFileName;

            }
            return relativeFileName;
        }

        public virtual string ResolveFileName(string relativeFileName, string lang = "")
        {
            if (string.IsNullOrEmpty(relativeFileName))
                throw new ArgumentNullException("relativeFileName");

            if (!relativeFileName.StartsWith("http://") && !relativeFileName.StartsWith("ftp://") && !relativeFileName.StartsWith("https://"))
            {
                if (string.IsNullOrEmpty(lang) || relativeFileName.StartsWith("./"))
                {
                    return InstalledPath + "\\" + (relativeFileName.StartsWith("./") ? relativeFileName.Substring(2, relativeFileName.Length - 2) : relativeFileName).Replace("/", "\\");
                }
                else
                    return InstalledPath + "\\locales\\" + lang + "\\" + relativeFileName;

            }
            return relativeFileName;
        }

        //public Boolean Verify(RSA Key, string signfile = "signature.xml")
        //{
        //    var signedXmlFile = InstalledPath + signfile;

        //    // Check arguments. 
        //    if (!File.Exists(signedXmlFile))
        //        throw new FileNotFoundException("signedXmlFile");

        //    if (Key == null)
        //        throw new ArgumentException("Key");

        //    var xmldoc = new XmlDocument();
        //    xmldoc.Load(signedXmlFile);

        //    // Create a new SignedXml object and pass it 
        //    // the XML document class.
        //    SignedXml signedXml = new SignedXml();
        //    signedXml.LoadXml(xmldoc.DocumentElement);

        //    // Check the signature and return the result. 
        //    return signedXml.CheckSignature(Key);
        //}

        //public Boolean Verify(string key, string signfile = "signature.xml")
        //{
        //    RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider();
        //    rsaKey.FromXmlString(key);
        //    return Verify(rsaKey, signfile);
        //}

        //public bool Verify()
        //{
        //    if (HasSigned)
        //    {
        //        var key = File.ReadAllText(InstalledPath + "\\signed.key");
        //        return Verify(key);
        //    }
        //    return false;
        //}

        //private void RecursiveAddRef(string absolutePath, SignedXml signedXml)
        //{
        //    var rootdir = new DirectoryInfo(absolutePath);
        //    var files = rootdir.GetFiles();
        //    foreach (var file in files)
        //    {
        //        // Create a reference to be signed.
        //        var relativeUri = file.FullName.Remove(0, this.InstalledPath.Length);
        //        Reference reference = new Reference(relativeUri);
        //        signedXml.AddReference(reference);
        //    }
        //    var subdirs = rootdir.GetDirectories();
        //    foreach (var dir in subdirs)
        //    {
        //        RecursiveAddRef(dir.FullName, signedXml);
        //    }
        //}

        //public void Sign(string key, string signfile = "signature.xml")
        //{
        //    var cspParams = new CspParameters();
        //    cspParams.KeyContainerName = "XML_DSIG_RSA_KEY";

        //    // Create a new RSA signing key and save it in the container. 
        //    var rsaKey = new RSACryptoServiceProvider(cspParams);
        //    Sign(rsaKey, signfile);
        //}

        //public void Sign(RSA key, string signfile = "signature.xml")
        //{
        //    var xmlDoc = new XmlDocument();

        //    // Create a SignedXml object.
        //    SignedXml signedXml = new SignedXml(xmlDoc);

        //    // Add the key to the SignedXml document.
        //    signedXml.SigningKey = key;
        //    signedXml.Signature.Id = "DistributorASignature";

        //    RecursiveAddRef(this.InstalledPath, signedXml);

        //    //#region distributor sign

        //    ////var dataFragment = "<Object Id=\"prop\" xmlns:dsp=\"http://www.w3.org/ns/widgets-digsig\"><dsp:Profile URI=\"http://www.dotnetage.com/ray\" /><dsp:Role URI=\"http://www.dotnetage.com\"/></Object>";
        //    //var dataDoc = new XmlDocument();
        //    //var profileNode = dataDoc.CreateNode(XmlNodeType.Element, "dsp", "Profile", "http://www.w3.org/ns/widgets-digsig");
        //    //var profileURIAttr = dataDoc.CreateAttribute("URI");
        //    //profileURIAttr.Value = "http://www.w3.org/ns/widgets-digsig#profile";
        //    //profileNode.Attributes.Append(profileURIAttr);
        //    //dataDoc.AppendChild(profileNode);

        //    //var dataObject = new DataObject();
        //    //dataObject.Data = dataDoc.ChildNodes;
        //    //dataObject.Id = "prop";

        //    //signedXml.AddObject(dataObject);

        //    //var dataReference = new Reference();
        //    //dataReference.Uri = "#prop";
        //    //signedXml.AddReference(dataReference);

        //    //#endregion

        //    #region Generate KeyInfo.

        //    KeyInfo keyInfo = new KeyInfo();
        //    keyInfo.AddClause(new RSAKeyValue(key));
        //    signedXml.KeyInfo = keyInfo;

        //    #endregion

        //    signedXml.ComputeSignature();
        //    XmlElement xmlDigitalSignature = signedXml.GetXml();
        //    File.WriteAllText(this.InstalledPath + (!this.InstalledPath.EndsWith("\\") ? "\\" : "") + signfile, xmlDigitalSignature.OuterXml, new UTF8Encoding(false));
        //}

        /// <summary>
        /// Copy all the files of this package to specified destination
        /// </summary>
        /// <param name="path"></param>
        public void Copy(string destination)
        {
            DNA.Utility.FileUtility.CopyDirectory(this.InstalledPath, destination);
        }

        ///// <summary>
        ///// Compress files to specified zip file name.
        ///// </summary>
        ///// <param name="file"></param>
        //public virtual void Compress(string file) { }

        ResourceManager resourceMan = null;

        protected ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    var resourceDir = InstalledPath + (InstalledPath.EndsWith("\\") ? "" : "\\") + "resources";
                    ResourceManager temp = ResourceManager.CreateFileBasedResourceManager("language", resourceDir, null);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        protected bool HasResouces
        {
            get
            {
                var resourceDir = InstalledPath + (InstalledPath.EndsWith("\\") ? "" : "\\") + "resources";
                return Directory.Exists(resourceDir) && Directory.GetFiles(resourceDir).Count() > 0;
            }
        }

        public virtual string Loc(string key, CultureInfo culture)
        {
            try
            {
                return this.ResourceManager.GetString(key, culture);
            }
            catch {
                return key;
            }
        }

        public string Loc(string key)
        {
            return this.Loc(key, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        public string Loc(string key, string locale)
        {
            return this.Loc(key, new CultureInfo(locale));
        }

        public bool Delete()
        {
            if (Directory.Exists(this.InstalledPath))
            {
                Directory.Delete(this.InstalledPath, true);
                return true;
            }
            return false;
        }

        public T Deserialize()
        {
            return this.Deserialize<T>(ConfigXml, this.DefaultNamespace);
        }

        public T Deserialize(Stream stream)
        {
            return this.Deserialize<T>(stream, this.DefaultNamespace);
        }

        public K Deserialize<K>(string xml, string @namespace = "")
        {
            using (var sr = new StringReader(xml))
            {
                var ser = string.IsNullOrEmpty(@namespace) ? new XmlSerializer(typeof(K)) : new XmlSerializer(typeof(K), @namespace);
                return (K)ser.Deserialize(sr);
            }
        }

        public K Deserialize<K>(Stream stream, string @namespace = "")
        {
            var ser = string.IsNullOrEmpty(@namespace) ? new XmlSerializer(typeof(K)) : new XmlSerializer(typeof(K), @namespace);
            return (K)ser.Deserialize(stream);
        }

    }
}
