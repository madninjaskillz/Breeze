using System;
using System.Collections.Generic;
using System.Linq;
using Breeze.AssetTypes.DataBoundTypes;
using Breeze.Screens;
using Breeze.Helpers;
using Microsoft.Xna.Framework;

namespace Breeze.AssetTypes.XMLClass
{
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class UIScreen : UIElement
    {

    }
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

        public static DataboundAsset.DataboundValue<T> GetDBValue<T>(string input)
        {
            if (input.ToLower().Trim().Contains("boundto"))
            {
                var result = new DataboundAsset.DataboundValue<T>(default(T));

                List<KeyValuePair<string, string>> args = SplitArgs(input);

                if (args.Any(x => x.Key == "boundto"))
                {
                    result.BoundTo = args.First(x => x.Key == "boundto").Value;
                }

                if (args.Any(x => x.Key == "bindingdirection"))
                {
                    result.BindingDirection = (MGUIViewModel.BindType)Enum.Parse(typeof(MGUIViewModel.BindType), args.First(x => x.Key == "bindingdirection").Value);
                }

                
                return result;
            }
            else
            {

                T defaultValue = (T)ConvertValue<T>(input);
                return new DataboundAsset.DataboundValue<T>(defaultValue);
            }
        }

        public static object ConvertValue<T>(string input)
        {
            if (input.ToLower().Trim() == "null")
            {
                return null;
            }

            if (typeof(T) == typeof(string))
            {
                return input;
            }

            if (typeof(T) == typeof(FloatRectangle) || typeof(T) == typeof(FloatRectangle?))
            {
                return new FloatRectangle(input);
            }

            if (typeof(T) == typeof(Thickness))
            {
                return new Thickness(input);
            }

            if (typeof(T) == typeof(Color) || typeof(T) == typeof(Color?))
            {
                if (input.ToLower().StartsWith("color"))
                {
                    var parts = input.Replace(" ", "").Split('.');
                    var cparts = input.Replace(" ", "").Substring(parts[0].Length+1).Split('*');

                    float mult = 1;
                    if (cparts.Length > 1)
                    {
                        float.TryParse(cparts[1], out mult);
                    }

                    return (Color)new Color((uint)Enum.Parse(typeof(ColorHelpers.XnaColor), cparts[0])) * mult;
                }

                if (input.Replace(" ","").ToLower().StartsWith("0x"))
                {
                    return (Color) new Color(Convert.ToUInt32(input, 16) );
                }

            }


            return (T)Convert.ChangeType(input, typeof(T));

        }

        public static List<KeyValuePair<string, string>> SplitArgs(string input)
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            string[] parts = input.Split(',');
            foreach (string part in parts)
            {
                if (part.Contains("="))
                {
                    int index = part.IndexOf("=");
                    string leftPart = part.Substring(0, index).Trim().ToLower();
                    string rightPart = part.Substring(index + 1).Trim();

                    result.Add(new KeyValuePair<string, string>(leftPart, rightPart));
                }
            }

            return result;
        }
    }
}
