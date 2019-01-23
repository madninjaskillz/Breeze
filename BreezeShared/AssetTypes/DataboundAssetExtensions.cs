using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Breeze.Helpers;
using Breeze.Screens;
using Microsoft.Xna.Framework;

namespace Breeze.AssetTypes.DataBoundTypes
{
    public static class DataboundAssetExtensions
    {
        public static DataboundAsset.DataboundValue<T> GetDBValue<T>(string input)
        {
            if (input.ToLower().Trim().Contains("boundto"))
            {
                var result = new DataboundAsset.DataboundValue<T>(default(T));

                List<KeyValuePair<string, string>> args = SplitArgs(input);

                if (args.Any(x => x.Key == "boundto"))
                {
                    string v = args.First(x => x.Key == "boundto").Value;
                 
                    result.BoundTo = v;
                }


                if (args.Any(x => x.Key.ToLower() == "invert"))
                {
                    result.Invert = bool.Parse(args.First(x => x.Key.ToLower() == "invert").Value);
                }

                if (args.Any(x => x.Key == "bindingdirection"))
                {
                    result.BindingDirection = (BindType)Enum.Parse(typeof(BindType), args.First(x => x.Key == "bindingdirection").Value);
                }


                return result;
            }
            else
            {
                string typeName = typeof(T).ToString();
                var x = typeof(T);
                if (x.Name == "Nullable`1")
                {
                    Type derp = Nullable.GetUnderlyingType(x);

                    T def = (T)ConvertValue(input, derp);
                    return new DataboundAsset.DataboundValue<T>(def);

                }

                T defaultValue = (T)ConvertValue<T>(input);

                return new DataboundAsset.DataboundValue<T>(defaultValue);
            }
        }

        public static object GetDBerValue(string input, Type T)
        {
            object defaultVal = T.GetDefaultValue();
            if (input.ToLower().Trim().Contains("boundto"))
            {
                DataboundAsset.DataboundValue result = CreateDataboundValue(T);
                List<KeyValuePair<string, string>> args = SplitArgs(input);

                if (args.Any(x => x.Key.ToLower() == "boundto"))
                {
                    string v = args.First(x => x.Key == "boundto").Value;
                    
                    result.BoundTo = v;
                }

                if (args.Any(x => x.Key.ToLower() == "invert"))
                {
                    result.Invert = bool.Parse(args.First(x => x.Key.ToLower() == "invert").Value);
                }

                if (args.Any(x => x.Key.ToLower() == "bindingdirection"))
                {
                    result.BindingDirection = (BindType)Enum.Parse(typeof(BindType), args.First(x => x.Key.ToLower() == "bindingdirection").Value);
                }

                return result;
            }
            else
            {

                if (T.Name == "Nullable`1")
                {
                    Type derp = Nullable.GetUnderlyingType(T);

                    object def = ConvertValue(input, derp);
                    //todo def not set
                    return CreateDataboundValue(T, def);

                }

                object defaultValue = ConvertValue(input, T);

                //todo defaultValue not set
                return CreateDataboundValue(T, defaultValue);
            }
        }

        private static DataboundAsset.DataboundValue CreateDataboundValue(Type type)
        {
            //if (type == typeof(Color))
            //{
            //    return new DataboundAsset.DataboundValue<Color>();
            //}

            var dvType = typeof(DataboundAsset.DataboundValue<>);
            var constructedListType = dvType.MakeGenericType(type);

            object instance = Activator.CreateInstance(constructedListType);

            DataboundAsset.DataboundValue result = (DataboundAsset.DataboundValue)instance;

            return result;
        }

        public static DataboundAsset.DataboundValue CreateDataboundValue(Type type, object def)
        {
            var dvType = typeof(DataboundAsset.DataboundValue<>);
            var constructedListType = dvType.MakeGenericType(type);

            object instance = Activator.CreateInstance(constructedListType);

            DataboundAsset.DataboundValue result = (DataboundAsset.DataboundValue)instance;

            result.ObjectValue = def;
            return result;
        }

        public static DataboundAsset.DataboundValue GetDBValue(string input, Type type)
        {
            //todo GOD DAMN i hate this.
            switch (type)
            {

                case Type intType when intType == typeof(int): return GetDBValue<int>(input);
                case Type intNType when intNType == typeof(int?): return GetDBValue<int?>(input);

                case Type stringType when stringType == typeof(string): return GetDBValue<string>(input);

                case Type floatType when floatType == typeof(float): return GetDBValue<float>(input);
                case Type floatNType when floatNType == typeof(float?): return GetDBValue<float?>(input);

                case Type boolType when boolType == typeof(bool): return GetDBValue<bool>(input);
                case Type boolNType when boolNType == typeof(bool?): return GetDBValue<bool?>(input);

                case Type ThicknessType when ThicknessType == typeof(Thickness): return GetDBValue<Thickness>(input);

                case Type ColorType when ColorType == typeof(Color): return GetDBValue<Color>(input);
                case Type ColorNType when ColorNType == typeof(Color?): return GetDBValue<Color?>(input);

                case Type FloatRectangleType when FloatRectangleType == typeof(FloatRectangle): return GetDBValue<FloatRectangle>(input);
                case Type FloatRectangleNType when FloatRectangleNType == typeof(FloatRectangle?): return GetDBValue<FloatRectangle?>(input);

                case Type FontJustificationType when FontJustificationType == typeof(FontAsset.FontJustification): return GetDBValue<FontAsset.FontJustification>(input);
                case Type FontJustificationNType when FontJustificationNType == typeof(FontAsset.FontJustification?): return GetDBValue<FontAsset.FontJustification?>(input);

            }

            throw new Exception("Unhandled type :(");
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
                    var cparts = input.Replace(" ", "").Substring(parts[0].Length + 1).Split('*');

                    float mult = 1;
                    if (cparts.Length > 1)
                    {
                        float.TryParse(cparts[1], out mult);
                    }

                    return (Color)new Color((uint)Enum.Parse(typeof(ColorHelpers.XnaColor), cparts[0])) * mult;
                }

                if (input.Replace(" ", "").ToLower().StartsWith("0x"))
                {
                    return (Color)new Color(Convert.ToUInt32(input, 16));
                }

            }


            return (T)Convert.ChangeType(input, typeof(T));

        }

        public static object ConvertValue(string input, Type type)
        {
            if (input.ToLower().Trim() == "null")
            {
                return null;
            }

            if (type == typeof(string))
            {
                return input;
            }

            if (type == typeof(FloatRectangle) || type == typeof(FloatRectangle?))
            {
                return new FloatRectangle(input);
            }

            if (type == typeof(Thickness))
            {
                return new Thickness(input);
            }

            if (type == typeof(Color) || type == typeof(Color?))
            {
                if (input.ToLower().StartsWith("color"))
                {
                    var parts = input.Replace(" ", "").Split('.');
                    var cparts = input.Replace(" ", "").Substring(parts[0].Length + 1).Split('*');

                    float mult = 1;
                    if (cparts.Length > 1)
                    {
                        float.TryParse(cparts[1], out mult);
                    }

                    return (Color)new Color((uint)Enum.Parse(typeof(ColorHelpers.XnaColor), cparts[0])) * mult;
                }

                if (input.Replace(" ", "").ToLower().StartsWith("0x"))
                {
                    return (Color)new Color(Convert.ToUInt32(input, 16));
                }

            }

            
            if (type.GetTypeInfo().IsEnum)
            {
                return Enum.Parse(type, input);
            }

            return Convert.ChangeType(input, type);

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
        public static void SetDVValue<T>(this DataboundAsset.DataboundValue<T> input, object value)
        {
            input.Value = (T) value;
        }
        public static bool HasValue<T>(this DataboundAsset.DataboundValue<T> dbValue)
        {
            if (dbValue == null)
            {
                return false;
            }

            return dbValue.Value() != null;
        }

        public static T GetVirtualizedValue<T>(this DataboundAsset.DataboundValue<T> dbValue)
        {
            if (dbValue.Value == null && dbValue.ParentAsset == null && dbValue.BoundTo == null)
            {
                return default(T);

            }

            if (dbValue.ParentAsset == null && dbValue.BoundTo == null)
            {
                return dbValue.Value;
            }

            if (dbValue.ParentAsset == null)
            {
                return default(T);
            }

            if (dbValue.Value != null && dbValue.BoundTo == null)
            {
                return dbValue.Value;
            }

            return dbValue.ParentAsset.GetVirtualizedValue(dbValue);
        }

        public static T Value<T>(this DataboundAsset.DataboundValue<T> dbValue) => GetVirtualizedValue(dbValue);

        public static void FixParentChildRelationship(this DataboundAsset asset)
        {
            if (asset is DataboundContainterAsset containerAsset)
            {
                if (containerAsset.Children?.Value != null)
                {
                    foreach (DataboundAsset child in containerAsset.Children.Value)
                    {
                        child.ParentAsset = asset;
                        FixParentChildRelationship(child);

                        if (child.VirtualizedDataContext.Screen == null)
                        {

                            if (asset?.ParentAsset?.VirtualizedDataContext?.Screen != null)
                            {
                                child.VirtualizedDataContext.Screen = asset?.ParentAsset?.VirtualizedDataContext?.Screen;
                            }
                        }
                    }
                }
            }
        }

        public static DataboundAsset FindChildOfType<T>(this DataboundContainterAsset containerAsset) where T : DataboundAsset
        {
            if (containerAsset.Children?.Value == null)
            {
                return null;
            }

            DataboundAsset correctChild = containerAsset.Children.Value.FirstOrDefault(t => t is T);
            if (correctChild != null)
            {
                return correctChild;
            }

            foreach (DataboundContainterAsset child in containerAsset.Children.Value.OfType<DataboundContainterAsset>())
            {
                DataboundAsset procChild = child.FindChildOfType<T>();
                if (procChild is T sorted)
                {
                    return sorted;
                }
            }

            return null;
        }

        public static List<DataboundAsset> FindChildrenOfType<T>(this DataboundContainterAsset containerAsset) where T : DataboundAsset
        {
            List<DataboundAsset> result = new List<DataboundAsset>();

            if (containerAsset.Children?.Value == null)
            {
                return result;
            }

            result.AddRange(containerAsset.Children.Value.Where(t => t is T));
            
            foreach (DataboundContainterAsset child in containerAsset.Children.Value.OfType<DataboundContainterAsset>())
            {
                DataboundAsset procChild = child.FindChildOfType<T>();
                if (procChild is T sorted)
                {
                    result.Add(sorted);
                }
            }

            return result;
        }
    }
}