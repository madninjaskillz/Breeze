using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Breeze.AssetTypes.DataBoundTypes;
using Breeze.Helpers;


namespace Breeze.Screens
{
    public class MGUIViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        private readonly List<DataboundAsset> boundAssets = new List<DataboundAsset>();
        public void Bind(DataboundAsset asset, DataboundAsset.DataboundValue dbValue, string propName)
        {
            if (!boundAssets.Contains(asset))
            {
                boundAssets.Add(asset);
            }

            asset.Bind(dbValue, propName);
        }

        public void Bind<T>(DataboundAsset asset, DataboundAsset.DataboundValue<T> dbValue, string propName)
        {
            if (!boundAssets.Contains(asset))
            {
                boundAssets.Add(asset);
            }

            asset.Bind(dbValue, propName);
        }

        public void TwoWayBind(DataboundAsset asset, DataboundAsset.DataboundValue dbValue, string propName)
        {
            if (!boundAssets.Contains(asset))
            {
                boundAssets.Add(asset);
            }

            asset.Bind(dbValue, propName);

            dbValue.SetReverseBindActionAsObject((x) =>
            {
                this.SetPropValue(propName, x);
            });

            //      asset.Bind(dbValue, propName);
        }

        public void TwoWayBind<T>(DataboundAsset asset, DataboundAsset.DataboundValue<T> dbValue, string propName)
        {
            if (!boundAssets.Contains(asset))
            {
                boundAssets.Add(asset);
            }

            asset.Bind(dbValue, propName);
            dbValue.SetReverseBindAction((x) =>
            {
                this.SetPropValue(propName, x);
            });

            //      asset.Bind(dbValue, propName);
        }

        public void BindEvent<T>(DataboundAsset asset, DataboundAsset.DataboundEvent<T> dbValue, Action<T> action)
        {
            dbValue.Action = action;
        }

        public bool Set<T>(ref T input, T newValue, [CallerMemberName] string callerMemberName = "")
        {
            if ((input == null && newValue != null) || (input != null && !input.Equals(newValue)))
            {
                foreach (DataboundAsset databoundAsset in boundAssets)
                {
                    KeyValuePair<string, object>? db = databoundAsset.DataBindings.FirstOrDefault(t => t != null && t.Value.Key == callerMemberName);
                    if (db == null) continue;
                    DataboundAsset.DataboundValue<T> dbv = (DataboundAsset.DataboundValue<T>)db.Value.Value;
                    dbv.Value = newValue;
                }
            }

            input = newValue;
            return true;

        }

        public enum BindType
        {
            OneWay,
            TwoWay
        }


    }

}
