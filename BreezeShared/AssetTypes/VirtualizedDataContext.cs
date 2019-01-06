using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Breeze.AssetTypes.DataBoundTypes;
using Breeze.Helpers;
using Breeze.Screens;

namespace Breeze
{
    public class VirtualizedDataContext
    {
        public VirtualizedDataContext()
        {
            PropertyInfo[] props = this.GetType().GetAllProperties();

            foreach (PropertyInfo propertyInfo in props)
            {
                DataboundAttribute attr = propertyInfo.GetCustomAttribute<DataboundAttribute>();
                if (attr != null)
                {
                    Store.Add(propertyInfo.Name, propertyInfo.GetValue(this));
                }
            }
        }

        public Dictionary<string, object> Store { get; set; } = new Dictionary<string, object>();


        public BaseScreen Screen { get; set; }

        public T GetValue<T>(DataboundAsset.DataboundValue<T> dbValue)
        {
            if (dbValue == null)
            {
                return default(T);
            }

            if (!string.IsNullOrWhiteSpace(dbValue.BoundTo))
            {

                if (!Store.ContainsKey(dbValue.BoundTo))
                {
                    return default(T);
                }

                object val = Store[dbValue.BoundTo];

                return (T)val;
            }
            else
            {
                return dbValue.Value;
            }
        }


        public void Set<T>(ref T input, T value, [CallerMemberName] string callerMemberName = "")
        {
            if (Store.ContainsKey(callerMemberName))
            {
                Store[callerMemberName] = value;
            }
            else
            {
                Store.Add(callerMemberName, value);
            }

            input = value;
        }
    }
}