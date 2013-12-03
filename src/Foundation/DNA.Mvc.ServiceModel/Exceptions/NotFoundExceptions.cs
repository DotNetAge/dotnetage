//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    ///   Represents widget package not found error.
    /// </summary>
    public class WidgetPackageNotFoundException : Exception
    {
        public WidgetPackageNotFoundException() : base() { }
        public WidgetPackageNotFoundException(string message) : base(message) { }
    }

    /// <summary>
    ///  Represents PageTemplate not found error.
    /// </summary>
    public class PageTemplateNotFoundException : Exception
    {
        public PageTemplateNotFoundException() : base() { }
        public PageTemplateNotFoundException(string message) : base(message) { }
    }

    /// <summary>
    ///  Represents widget not found error.
    /// </summary>
    public class WebNotFoundException : Exception
    {
        public WebNotFoundException() : base() { }
        public WebNotFoundException(string message) : base(message) { }
    }

    /// <summary>
    ///  Represents web page not found error.
    /// </summary>
    public class WebPageNotFoundException : Exception
    {
        public WebPageNotFoundException() : base() { }
        public WebPageNotFoundException(string message) : base(message) { }
    }

    /// <summary>
    ///  Represents widget instance not found error.
    /// </summary>
    public class WidgetInstanceNotFoundException : Exception
    {
        public WidgetInstanceNotFoundException() : base() { }
        public WidgetInstanceNotFoundException(string message) : base(message) { }
    }

    /// <summary>
    ///  Represents WidgetDescriptor not found error.
    /// </summary>
    public class WidgetDescriptorNotFoundException : Exception
    {
        public WidgetDescriptorNotFoundException() : base() { }
        public WidgetDescriptorNotFoundException(string message) : base(message) { }
    }

    /// <summary>
    ///  Represents user not found error.
    /// </summary>
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException() : base() { }
        public UserNotFoundException(string message) : base(message) { }
    }

    /// <summary>
    ///  Represents ContentList not found error.
    /// </summary>
    public class ContentListNotFoundException : Exception
    {
        public ContentListNotFoundException() : base() { }
        public ContentListNotFoundException(string message) : base(message) { }
    }

    /// <summary>
    ///  Represents ContentView not found error.
    /// </summary>
    public class ContentViewNotFoundException : Exception
    {
        public ContentViewNotFoundException() : base() { }
        public ContentViewNotFoundException(string message) : base(message) { }
    }

    /// <summary>
    ///  Represents ContentDataItem not found error.
    /// </summary>
    public class ContentDataItemNotFoundException : Exception
    {
        public ContentDataItemNotFoundException() : base() { }
        public ContentDataItemNotFoundException(string message) : base(message) { }
    }

}

