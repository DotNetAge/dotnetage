///  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
///  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
///  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Collections;

namespace DNA.Web
{
    public interface IModelWrapper
    {
        IEnumerable Model { get; }
        int Total { get; }
    }

    [Serializable]
    public class ModelWrapper : IModelWrapper
    {
        public ModelWrapper() { }
        public ModelWrapper(IEnumerable model)
        {
            Model = model;
        }
        public ModelWrapper(IEnumerable model, int total)
            : this(model)
        { Total = total; }

        public IEnumerable Model { get; set; }

        public int Total { get; set; }

        IEnumerable IModelWrapper.Model
        {
            get { return this.Model; }
        }

        int IModelWrapper.Total
        {
            get { return this.Total; }
        }
    }

    [Serializable]
    public class ModelWrapper<T> : IModelWrapper
    {
        public ModelWrapper() { }
        
        public ModelWrapper(IEnumerable<T> model)
        {
            Model = model;
        }

        public ModelWrapper(IEnumerable<T> model, int total)
            : this(model)
        { Total = total; }

        public IEnumerable<T> Model { get; set; }

        public int Total { get; set; }

        IEnumerable IModelWrapper.Model
        {
            get { return this.Model; }
        }

        int IModelWrapper.Total
        {
            get { return this.Total; }
        }
    }


}
