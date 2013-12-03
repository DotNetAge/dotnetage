//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web;
using System.IO;
using System.Linq;

namespace DNA.Xml.Widgets
{
    public class WidgetPackage:PackageBase<WidgetElement>
    {
        private string category = "";

        public string Category { get { return category; } }

        public WidgetPackage(string path):base(path){}

        protected override void Init(string path)
        {
            base.Init(path);
            var curDir = new DirectoryInfo(path);
            var parentDir = curDir.Parent;
            this.category = parentDir.Name;
        }

        public override string DefaultNamespace
        {
            get
            {
                return "http://www.w3.org/ns/widgets";
            }
        }

        /// <summary>
        /// Get localized widget element instance
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public override WidgetElement Locale(string lang)
        {
            var widget = this.Model.Clone();

            if (!string.IsNullOrEmpty(lang))
            {
                //Get name
                var name = GetLocalizedNode("name", lang);
                if (name != null)
                {
                    if (name.Attributes["short"] != null && !string.IsNullOrEmpty(name.Attributes["short"].Value))
                    {
                        widget.Name.ShortName = name.Attributes["short"].Value;
                        widget.Name.FullName = name.InnerText;
                    }
                }
                else {
                    if (!string.IsNullOrEmpty(widget.Name.ResKey))
                    {
                        widget.Name.FullName = Loc(widget.Name.ResKey);
                    }
                   //var resKey=this.Name
                }

                //Get desc
                var desc = GetLocalizedNode("description", lang);
                if (desc != null)
                    widget.Description = desc.InnerText;

                //Get icons
                ///TODO:Replace the default icons

                //Get content
                var content = GetLocalizedNode("content", lang);
                if (content != null)
                    widget.Content = this.Deserialize<ContentElement>(content.OuterXml);
            }

            if (widget.Icons != null && widget.Icons.Count() > 0)
            {
                foreach (var icon in widget.Icons)
                    ResolveFileName(icon.Source, lang);
            }

            if (widget.Content != null && !string.IsNullOrEmpty(widget.Content.Source) && !string.IsNullOrEmpty(lang))
            {
                var contentFile =ResolveFileName(widget.Content.Source,lang);
                if (File.Exists(contentFile))
                    widget.Content.Source = contentFile;
            }

            return widget;
        }

        //private string ResolveBasePath
        //{
        //    get
        //    {
        //        return Path + (!Path.EndsWith("\\") ? "\\" : "");
        //    }
        //}

        public override string ResolveUri(string relativeFileName, string lang = "")
        {
            if (!relativeFileName.StartsWith("http://") && !relativeFileName.StartsWith("ftp://") && !relativeFileName.StartsWith("https://"))
            {
                if (string.IsNullOrEmpty(lang))
                    return BaseUrl + this.Category+"/"+this.Name  + "/" + relativeFileName;
                else
                    return BaseUrl + this.Category + "/" + this.Name + "/locales/" + lang + "/" + relativeFileName;

            }
            return relativeFileName;
        }

        //private void RecursiveAddRef(string relativePath, string absolutePath, SignedXml signedXml)
        //{
        //    var rootdir = new DirectoryInfo(absolutePath);
        //    var files = rootdir.GetFiles();
        //    foreach (var file in files)
        //    {
        //        // Create a reference to be signed.
        //        var relativeUri = file.FullName.Replace(Path, relativePath);
        //        Reference reference = new Reference(relativeUri);
        //        //reference.AddTransform(new XmlDsigExcC14NTransform());
        //        signedXml.AddReference(reference);
        //    }
        //    var subdirs = rootdir.GetDirectories();
        //    foreach (var dir in subdirs)
        //    {
        //        RecursiveAddRef(relativePath, dir.FullName, signedXml);
        //    }
        //}

        //public void Sign(string relativePath, string key, string signfile = "signature.xml")
        //{
        //    // Create a new CspParameters object to specify 
        //    // a key container.
        //    //            CspParameters cspParams = new CspParameters();
        //    //         cspParams.KeyContainerName = "XML_DSIG_RSA_KEY";
        //    // Create a new RSA signing key and save it in the container. 
        //    //      RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider(cspParams);

        //    RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider();
        //    rsaKey.FromXmlString(key);
        //    Sign(relativePath, rsaKey, signfile);
        //}

        //public Boolean Verify(string key, string signfile = "signature.xml")
        //{
        //    RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider();
        //    rsaKey.FromXmlString(key);
        //    return Verify(rsaKey, signfile);
        //}

        //public void Sign(string relativePath, RSA key, string signfile = "signature.xml")
        //{
        //    var xmlDoc = new XmlDocument();

        //    // Create a SignedXml object.
        //    SignedXml signedXml = new SignedXml(xmlDoc);

        //    // Add the key to the SignedXml document.
        //    signedXml.SigningKey = key;
        //    signedXml.Signature.Id = "DistributorASignature";

        //    #region Sign files

        //    RecursiveAddRef(relativePath, Path, signedXml);
        //    //Reference reference = new Reference();
        //    //reference.Uri = relativePath + "\\config.xml";
        //    //XmlDsigExcC14NTransform env = new XmlDsigExcC14NTransform();
        //    //reference.AddTransform(env);
        //    //signedXml.AddReference(reference);

        //    //var htmlReference = new Reference(relativePath + "\\index.html");
        //    //htmlReference.AddTransform(new XmlDsigExcC14NTransform());
        //    ////htmlReference.Id = "rootIndex";
        //    //signedXml.AddReference(htmlReference);

        //    //var lochtmlReference = new Reference(relativePath + "\\locales\\en-au\\index.html");
        //    ////lochtmlReference.Id = "locindex";
        //    //lochtmlReference.AddTransform(new XmlDsigExcC14NTransform());
        //    //signedXml.AddReference(lochtmlReference);

        //    //var lochtml1Reference = new Reference(relativePath + "\\locales\\zh-cn\\index.html");
        //    ////lochtmlReference.Id = "locindex";
        //    //lochtml1Reference.AddTransform(new XmlDsigExcC14NTransform());
        //    //signedXml.AddReference(lochtml1Reference);

        //    //var scrtipReference = new Reference(relativePath + "\\locales\\zh-cn\\scripts\\runtime.js");
        //    ////lochtmlReference.Id = "locindex";

        //    ////scrtipReference.AddTransform(new XmlDsigExcC14NTransform());
        //    //signedXml.AddReference(scrtipReference);

        //    //var lochtml3Reference = new Reference(relativePath + "\\locales\\scripts\\runtime.js");
        //    ////lochtmlReference.Id = "locindex";
        //    //lochtml3Reference.AddTransform(new XmlDsigExcC14NTransform());
        //    //signedXml.AddReference(lochtml3Reference);

        //    #endregion

        //    #region distributor sign

        //    //var dataFragment = "<Object Id=\"prop\" xmlns:dsp=\"http://www.w3.org/ns/widgets-digsig\"><dsp:Profile URI=\"http://www.dotnetage.com/ray\" /><dsp:Role URI=\"http://www.dotnetage.com\"/></Object>";
        //    var dataDoc = new XmlDocument();
        //    var profileNode = dataDoc.CreateNode(XmlNodeType.Element, "dsp", "Profile", "http://www.w3.org/ns/widgets-digsig");
        //    var profileURIAttr = dataDoc.CreateAttribute("URI");
        //    profileURIAttr.Value = "http://www.w3.org/ns/widgets-digsig#profile";
        //    profileNode.Attributes.Append(profileURIAttr);
        //    dataDoc.AppendChild(profileNode);

        //    var dataObject = new DataObject();
        //    dataObject.Data = dataDoc.ChildNodes;
        //    dataObject.Id = "prop";

        //    signedXml.AddObject(dataObject);

        //    var dataReference = new Reference();
        //    dataReference.Uri = "#prop";
        //    //dataReference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        //    signedXml.AddReference(dataReference);

        //    #endregion

        //    #region Generate KeyInfo.

        //    KeyInfo keyInfo = new KeyInfo();
        //    keyInfo.AddClause(new RSAKeyValue(key));
        //    signedXml.KeyInfo = keyInfo;

        //    #endregion

        //    signedXml.ComputeSignature();
        //    XmlElement xmlDigitalSignature = signedXml.GetXml();
        //    File.WriteAllText(Path + (!Path.EndsWith("\\") ? "\\" : "") + signfile, xmlDigitalSignature.OuterXml, new UTF8Encoding(false));
        //}

        //public Boolean Verify(RSA Key, string signfile = "signature.xml")
        //{
        //    var signedXmlFile = Path + (!Path.EndsWith("\\") ? "\\" : "") + signfile;

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

        //public bool Verify() 
        //{
        //    if (HasSigned) 
        //    {
        //        var key = File.ReadAllText(ResolveBasePath + "\\signed.key");
        //        return Verify(key);
        //    }
        //    return false;
        //}

        //public bool HasSigned 
        //{
        //    get
        //    {
        //        return System.IO.File.Exists(ResolveBasePath + "\\signed.key");
        //    }
        //}
    }
}
