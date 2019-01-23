using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using Breeze.AssetTypes.DataBoundTypes;
using Breeze.Helpers;
using Breeze.Screens;

namespace Breeze
{
    public static class ScreenXMLHelpers
    {

        public static List<DataboundAsset> HandleChildren(BaseScreen.Resources screenResources, XmlNode node, DataboundAsset parentAsset = null, BaseScreen screen = null)
        {
            List<DataboundAsset> results = new List<DataboundAsset>();
            List<Type> types = ReflectionHelpers.TypesImplementingInterface(typeof(DataboundAsset)).ToList();

            foreach (XmlNode childNode in node.ChildNodes)
            {
                Type type = types.FirstOrDefault(t => t.Name == childNode.Name);
                if (type != null)
                {
                    DataboundAsset asset = DataboundAsset.CreateFromXml(childNode.Attributes, type, parentAsset, screen);
                    asset.ParentAsset = parentAsset;

                    if (asset is DataboundAssetWhereChildIsContentAsset masterAsset && !string.IsNullOrEmpty(childNode.InnerText))
                    {
                        masterAsset.LoadContent(childNode.InnerText);
                    }

                    if (asset is DataboundContainterAsset containerAsset)
                    {
                        containerAsset.Children.Value.AddRange(HandleChildren(screenResources, childNode, asset));
                    }

                    results.Add(asset);
                }
                else
                {
                    if (childNode.Name.ToLower() == "resources")
                    {
                        foreach (XmlNode xmlNode in childNode.ChildNodes)
                        {
                            if (xmlNode.Name.ToLower() == "template")
                            {
                                string nm = (string)xmlNode.Attributes.GetNamedItem("Name").Value;

                                string templateAsString = xmlNode.InnerXml;
                                screenResources.TemplateXMLs.Add(nm, templateAsString);
                            }
                        }
                    }
                }
            }

            return results;
        }

    }
}
