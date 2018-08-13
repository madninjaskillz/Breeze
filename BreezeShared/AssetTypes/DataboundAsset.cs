using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Breeze.AssetTypes;
using Breeze.Helpers;
using Breeze.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
    public class ScreenAsset : DataboundAsset
    {

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
                    child.ParentPosition = new FloatRectangle(this.ActualPosition);
                    //child.Position.Value = new FloatRectangle(this.Position.Value.X,this.Position.Value.Y,child.Position.Value.Width,child.Position.Value.Height);
                }
            }
        }


    }

    public class Binding
    {
        public DataboundAsset.DataboundValue Property { get; set; }
        public string BoundTo { get; set; }

        public MGUIViewModel.BindType Direction { get; set; }
    }

    public partial class DataboundAsset
    {
        public DataboundValue<bool> IsHidden { get; set; } = new DataboundValue<bool>();
        public string Key { get; set; }
        public int ZIndex { get; set; } = 0;

        public FloatRectangle? Clip { get; set; } = null;

        public Rectangle? ScissorRect { get; set; }
        //public FloatRectangle Position { get; set; }

        public virtual void Draw(SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
        {
            throw new NotImplementedException();
        }




        public string XName { get; set; }
        //public DataboundAsset Parent { get; set; } = null;
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
                    return Position.Value;
                }

                return Position.Value.ConstrainTo(ParentPosition.Value);
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
            public string BoundTo { get; set; } = null;
            public MGUIViewModel.BindType BindingDirection { get; set; } = MGUIViewModel.BindType.OneWay;

            [XmlIgnore]
            public Guid Id = Guid.NewGuid();

            [XmlIgnore]
            public Action<object> OnReverseBindGeneric;
            public void SetReverseBindActionAsObject(Action<object> action)
            {
                this.OnReverseBindGeneric = action;
            }
        }

        public virtual void LoadFromXml(XmlAttributeCollection childNodeAttributes)
        {
            throw new NotImplementedException();
        }
    }

    //public static class DataboundAssetExtensions
    //{
    //    public static void Bind<T>(this DataboundAsset asset, DataboundAsset.DataboundValue<T> databoundValue, string bindingName)
    //    {

    //    }
    //}
}
