using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Breeze.AssetTypes;
using Breeze.AssetTypes.XMLClass;
using Breeze.Helpers;
using Breeze.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace Breeze.AssetTypes.DataBoundTypes
{
    public class KeyedAsset : DataboundAsset
    {
        public void Update(KeyedAsset da, KeyedAsset asset)
        {
            //throw new NotImplementedException();
        }

        public bool IsDirty { get; set; }
    }

    public class DataboundContainterAsset : DataboundAsset
    {
        public DataboundValue<List<DataboundAsset>> Children { get; set; } = new DataboundValue<List<DataboundAsset>>(new List<DataboundAsset>());

        public void SetChildrenOriginToMyOrigin()
        {
            if (Children.Value != null)
            {
                foreach (var child in Children.Value)
                {
                    child.ParentPosition = new FloatRectangle(this.ActualPosition.AdjustForMargin(Margin));
                    //child.Position.Value = new FloatRectangle(this.Position.Value.X,this.Position.Value.Y,child.Position.Value.Width,child.Position.Value.Height);
                }
            }
        }

        public void SetChildrenOriginToMyOrigin(FloatRectangle actualPosition)
        {
            if (Children.Value != null)
            {
                foreach (var child in Children.Value)
                {
                    child.ParentPosition = actualPosition;
                    //child.Position.Value = new FloatRectangle(this.Position.Value.X,this.Position.Value.Y,child.Position.Value.Width,child.Position.Value.Height);
                }
            }
        }
    }

    public class Binding
    {
        public DataboundAsset.DataboundValue Property { get; set; }
        public string BoundTo { get; set; }
        public BindType Direction { get; set; }
    }

    public class DataboundAssetWhereChildIsContentAsset : DataboundAsset
    {
        public virtual void LoadContent(string child)
        {
            throw new NotImplementedException();
        }
    }

    public partial class DataboundAsset
    {
        public override string ToString()
        {
            return XName + ", Type:" + this.GetType().ToString() + ", ActualPosition:" + this.ActualPosition+", Position:"+this.Position.ToString() + ", ParentName:" + this.ParentAsset?.XName;
        }
        public class AssetAttribute : Attribute
        {
            public string XMLName;
    
            public AssetAttribute(string xmlName)
            {
                XMLName = xmlName;
            }
        }


        public bool IsHiddenOrParentHidden()
        {
            var isHidden = IsHidden.Value();

            if (!isHidden && ParentAsset!=null)
            {
                isHidden = ParentAsset.IsHiddenOrParentHidden();
            }

            return isHidden;
        }
        public DataboundValue<bool> IsHidden { get; set; } = new DataboundValue<bool>();
        public string Key { get; set; }
        public int ZIndex { get; set; } = 0;

        public FloatRectangle? Clip { get; set; } = null;

        public Rectangle? ScissorRect { get; set; }
       
        public virtual void Draw(BaseScreen.Resources screenResources, SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
        {
            throw new NotImplementedException();
        }
        
        public DataboundValue<string> XName { get; set; } = new DataboundValue<string>();
        public DataboundValue<Thickness> Margin { get; set; } = new DataboundValue<Thickness>();

        [XmlIgnore]
        public FloatRectangle? ParentPosition { get; set; }
        public DataboundValue<FloatRectangle> Position { get; set; } = new DataboundValue<FloatRectangle>();

        [XmlIgnore]
        public FloatRectangle ActualPosition
        {
            get
            {
                if (ParentPosition == null)
                {
                    return Position.Value();
                }

                return Position.Value().ConstrainTo(ParentPosition.Value);
            }
        }

        [XmlIgnore]
        public Vector2 ActualSize { get; set; } = Vector2.Zero;
        public List<Binding> Bindings { get; set; } = new List<Binding>();

        public List<KeyValuePair<string, object>?> DataBindings = new List<KeyValuePair<string, object>?>();

        public void Bind<T>(DataboundAsset.DataboundValue<T> databoundValue, string bindingName)
        {
            if (DataBindings.Any(x => x != null && x.Value.Value == databoundValue))
            {
                DataBindings.RemoveAll(x => x != null && x.Value.Value == databoundValue);
            }

            DataBindings.Add(new KeyValuePair<string, object>(bindingName, databoundValue));
        }

        public void Bind(DataboundAsset.DataboundValue databoundValue, string bindingName)
        {
            if (DataBindings.Any(x => x != null && x.Value.Value == databoundValue))
            {
                DataBindings.RemoveAll(x => x != null && x.Value.Value == databoundValue);
            }

            DataBindings.Add(new KeyValuePair<string, object>(bindingName, databoundValue));
        }

        public bool Set<T>(ref T input, T newValue, [CallerMemberName] string callerMemberName = "")
        {
            if (!input.Equals(newValue))
            {
                var db = DataBindings.FirstOrDefault(t => t != null && t.Value.Key == callerMemberName);
                if (db == null)
                {
                    throw new Exception("Binding not found");
                }

                DataboundValue<T> dbv = (DataboundValue<T>)db.Value.Value;

                dbv.Value = newValue;
                return true;
            }
            else
            {
                return false;
            }
        }

        public class DataboundEvent<T>
        {
            [XmlIgnore]
            public Action<T> Action { get; set; }

            private string _event;

            public string Event
            {
                get { return _event; }
                set { _event = value; }
            }

            public void Fire(T args)
            {
                Action?.Invoke(args);
            }

          
        }

        public abstract class DataboundValue
        {
            public DataboundAsset ParentAsset;
            public string BoundTo { get; set; } = null;
            public bool Invert { get; set; } = false;
            public BindType BindingDirection { get; set; } = BindType.OneWay;

            [XmlIgnore]
            public Guid Id = Guid.NewGuid();

            [XmlIgnore]
            public Action<object> OnReverseBindGeneric;
            public void SetReverseBindActionAsObject(Action<object> action)
            {
                this.OnReverseBindGeneric = action;
            }

            public object ObjectValue { get; set; }
        }

        public void LoadFromXml(XmlAttributeCollection childNodeAttributes)
        {
            PropertyInfo[] props = this.GetType().GetAllProperties();

            foreach (PropertyInfo propertyInfo in props)
            {
                AssetAttribute attr = propertyInfo.GetCustomAttribute<AssetAttribute>();
                if (attr != null)
                {
                    if (childNodeAttributes.GetNamedItem(attr.XMLName)?.Value != null)
                    {
                        string temp = childNodeAttributes.GetNamedItem(attr.XMLName)?.Value;
                        if (temp != null)
                        {

                            DataboundValue temp2 = DataboundAssetExtensions.GetDBValue(temp, propertyInfo.PropertyType.GenericTypeArguments.First());
                            
                            propertyInfo.SetValue(this, temp2);
                        }

                    }
                }
                else
                {
                    string name = propertyInfo.Name;
                    string temp = childNodeAttributes.GetNamedItem(name)?.Value;
                    if (temp != null)
                    {
                        DataboundValue temp2 = DataboundAssetExtensions.GetDBValue(temp, propertyInfo.PropertyType.GenericTypeArguments.First());
                        propertyInfo.SetValue(this, temp2);
                    }
                }
            }
            }

        public static DataboundAsset CreateFromXml(XmlAttributeCollection childNodeAttributes, Type type, DataboundAsset parentAsset, BaseScreen baseScreen)
        {
            DataboundAsset asset = (DataboundAsset)Activator.CreateInstance(type);

            var assetType = asset.GetType();

            PropertyInfo[] tprops = assetType.GetProperties();
            
            PropertyInfo[] props = tprops.Where(propertyInfo => propertyInfo.GetMethod.ReturnType.AssemblyQualifiedName.Contains("DataboundValue")).ToArray();
            
            List<string> childNodeNames = new List<string>();
            foreach (object childNodeAttribute in childNodeAttributes)
            {
                childNodeNames.Add(((XmlAttribute)childNodeAttribute).Name);
            }

            Dictionary<string,PropertyInfo> prps = new Dictionary<string, PropertyInfo>();

            foreach (PropertyInfo propertyInfo in props)
            {
                AssetAttribute attr = propertyInfo.GetCustomAttribute<AssetAttribute>();

                string name = propertyInfo.Name;
                if (attr != null)
                {
                    name = attr.XMLName;
                }

                prps.Add(name, propertyInfo);
            }

            Breeze.VirtualizedDataContext embeddedDataContext = new VirtualizedDataContext();

            foreach (string s in childNodeNames)
            {
                if (prps.ContainsKey(s))
                {
                    PropertyInfo p = prps[s];

                    string temp = childNodeAttributes.GetNamedItem(s)?.Value;
                    if (temp != null)
                    {
                        DataboundValue temp3 = (DataboundValue)DataboundAssetExtensions.GetDBerValue(temp, p.PropertyType.GenericTypeArguments.First());
                        
                        p.SetValue(asset, temp3);
                    }
                }
                else
                {
                    
                    if (s.StartsWith("x_"))
                    {
                        string propName = s.Substring(2);

                        string temp = childNodeAttributes.GetNamedItem(s)?.Value;

                        string propType = temp.Split('|').First();
                        string value = temp.Split('|').Last();
                  
                        Type deftype = Type.GetType(propType) ?? Type.GetType("System." + propType);

                        if (temp != null)
                        {
                            DataboundValue temp3 = (DataboundValue)DataboundAssetExtensions.GetDBerValue(value, deftype);

                            embeddedDataContext.Store.Add(propName, value);
                        }
                    }
                    else
                    {
                        throw new Exception("Could not find prop: " + s);
                    }
                }
            }

            if (embeddedDataContext.Store.Count > 0)
            {
                asset.VirtualizedDataContext = embeddedDataContext;
                asset.VirtualizedDataContext.Screen = baseScreen;
            }

            return asset;
        }
    }

    //public static class DataboundAssetExtensions
    //{
    //    public static void Bind<T>(this DataboundAsset asset, DataboundAsset.DataboundValue<T> databoundValue, string bindingName)
    //    {

    //    }
    //}
}
