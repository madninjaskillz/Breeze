using System;
using System.Diagnostics;
using System.Reflection;
using Breeze.AssetTypes.XMLClass;

namespace Breeze
{
    public class DataboundAttribute : Attribute
    {

    }
}

namespace Breeze.AssetTypes.DataBoundTypes
{
    public partial class DataboundAsset
    {
        public void FixBinds()
        {
            //PropertyInfo[] props = this.GetType().GetAllProperties();
            PropertyInfo[] props = this.GetType().GetProperties();

            foreach (PropertyInfo propertyInfo in props)
            {
                if (propertyInfo.PropertyType.Name.StartsWith("DataboundValue"))
                {
                    DataboundValue tmp = (DataboundValue)propertyInfo.GetValue(this);
                    tmp.ParentAsset = this;
                }
            }

            if (this is DataboundContainterAsset containerAsset)
            {
                if (containerAsset.Children != null && containerAsset.Children.Value != null)
                {
                    foreach (DataboundAsset databoundAsset in containerAsset.Children.Value)
                    {
                        databoundAsset.FixBinds();
                    }
                }
            }
        }

        private VirtualizedDataContext virtualizedDataContext;
        public VirtualizedDataContext VirtualizedDataContext
        {
            get => virtualizedDataContext ?? ParentAsset?.VirtualizedDataContext;
            set => virtualizedDataContext = value;
        }

        public DataboundAsset ParentAsset { get; set; }

        public T GetVirtualizedValue<T>(DataboundValue<T> dbValue)
        {
            VirtualizedDataContext context = this.VirtualizedDataContext;
            return context.GetValue<T>(dbValue);
        }
        public class DataboundValue<T> : DataboundValue
        {
            private string binding;
            private T value;

            public T Value
            {
                get
                {
                    if (ObjectValue != null)
                    {
                        value = (T)ObjectValue;
                        ObjectValue = null;
                    }
                    return value;
                }

                set
                {
                    if (!string.IsNullOrWhiteSpace(BoundTo))
                    {
                        Debug.WriteLine(BoundTo);
                    }

                    if (this.value == null && value == null)
                    {
                        return;
                    }

                    if ((value == null && this.value != null) || (value != null && this.value == null))
                    {


                        this.value = value;
                        onChange?.Invoke();
                        onReverseBind?.Invoke(value);
                        if (base.OnReverseBindGeneric != null)
                        {
                            OnReverseBindGeneric(value);
                        }
                    }
                    else
                    {
                        if (value != null && !value.Equals(this.value))
                        {

                            this.value = value;
                            onChange?.Invoke();
                            onReverseBind?.Invoke(value);
                            if (base.OnReverseBindGeneric != null)
                            {
                                OnReverseBindGeneric(value);
                            }
                        }
                    }
                }
            }

            private Action onChange;
            private Action<T> onReverseBind;

            public DataboundValue()
            {

            }

            public DataboundValue(Action onChange = null)
            {
                if (onChange != null)
                {
                    this.onChange = onChange;
                }
            }


            public DataboundValue(T defaultValue, Action onChange = null)
            {
                if (onChange != null)
                {
                    this.onChange = onChange;
                }

                value = defaultValue;
            }

            public void SetChangeAction(Action action)
            {
                this.onChange = action;
            }

            public void SetReverseBindAction(Action<T> action)
            {
                this.onReverseBind = action;
            }

            public override string ToString()
            {
                if (Value == null)
                {
                    return "Is Null";
                }

                return Value.ToString();
            }

        }

    }
}
