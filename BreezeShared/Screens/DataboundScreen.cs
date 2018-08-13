using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;
using Breeze.AssetTypes;
using Breeze.Helpers;
using Breeze.AssetTypes.DataBoundTypes;

namespace Breeze.Screens
{
    public class DataboundScreen : BaseScreen
    {
        public void LoadXAML()
        {
            Type type = this.GetType();

            Debug.WriteLine(type);
            string path = "Screens\\" + type.Name.Substring(0, type.Name.Length - ("Screen").Length) + "\\" + type.Name + ".xml";
            string xmlTest = Solids.Instance.Storage.FileSystemStorage.ReadText(path);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlTest);
            LoadScreen(xmlDoc);
        }
        public MGUIViewModel DataContext { get; set; }

        public void SetBindings()
        {
            var assets = GetAssetsAndTheirChildren(FixedAssets);

            foreach (var asset in assets.Where(x => x is DataboundAsset))
            {
                System.Diagnostics.Debug.WriteLine(asset);

                Type type = asset.GetType();

                var props = type.GetProperties();
                

                foreach (PropertyInfo propertyInfo in props.Where(x=>x.PropertyType.DeclaringType == typeof(DataboundAsset)))
                {
                    object t = asset.GetType().GetProperty(propertyInfo.Name).GetValue(asset, null);

                    if (t is DataboundAsset.DataboundValue propAsset)
                    {
                        if (!string.IsNullOrWhiteSpace(propAsset.BoundTo))
                        {
                            Debug.WriteLine(propAsset.BoundTo);

                            if (propAsset.BindingDirection == MGUIViewModel.BindType.OneWay)
                            {
                                this.DataContext.Bind((DataboundAsset)asset, propAsset, propAsset.BoundTo);
                            }


                            if (propAsset.BindingDirection == MGUIViewModel.BindType.TwoWay)
                            {
                                this.DataContext.TwoWayBind((DataboundAsset)asset, propAsset, propAsset.BoundTo);
                            }
                        }
                    }

                }
            }
        }

        //public abstract class Binding
        //{ }

        //public class Binding<T> 
        //{
        //    public DataboundAsset.DataboundValue<T> Property { get; set; }  
        //    public string Target { get; set; }
        //    public BindType BindType { get; set; } = BindType.OneWay;

        //    public Binding(DataboundAsset.DataboundValue<T> property, string target, BindType bindType = BindType.OneWay)
        //    {
        //        this.Property = property;
        //        this.Target = target;
        //        this.BindType = bindType;
        //    }

        //    public Binding(DataboundAsset.DataboundValue<T> property, string target)
        //    {
        //        this.Property = property;
        //        Target = target;
        //    }
        //}

        //public void AddAsset<T>(DataboundAsset asset, List<Binding<T>> bindings) 
        //{
        //    base.FixedAssets.Add(asset);

        //    foreach(Binding<T> binding in bindings)
        //    {
        //        switch (binding.BindType)
        //        {
        //            case BindType.OneWay:
        //                {
        //                    Bind(asset, binding.Property, binding.Target);
        //                    break;
        //                }

        //            case BindType.TwoWay:
        //                {
        //                    TwoWayBind(asset, binding.Property, binding.Target);
        //                    break;
        //                }
        //        }
        //    }

        //}
    }
}