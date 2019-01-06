using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Breeze.AssetTypes.DataBoundTypes;
using Breeze.Screens;
using Breeze.Helpers;
using Microsoft.Xna.Framework;

namespace Breeze.AssetTypes.XMLClass
{
    public class UIElement
    {
        //[System.Xml.Serialization.XmlAttribute("UIElement.Children")]
        [System.Xml.Serialization.XmlArrayItem("Children", IsNullable = false)]
        public UIElement[] UIElementChildren { get; set; }

        [System.Xml.Serialization.XmlAttribute()]
        public string Position { get; set; }

        [System.Xml.Serialization.XmlAttribute()]
        public string Margin { get; set; }

        public virtual DataboundContainterAsset ToSystem()
        {
            throw new NotImplementedException();
        }

        
    }
}
