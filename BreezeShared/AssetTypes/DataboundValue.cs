using System;
using System.Diagnostics;

namespace Breeze.AssetTypes.DataBoundTypes
{

    public partial class DataboundAsset
    {
        public class DataboundValue<T> : DataboundValue
        {
            
            private string binding;
            private T value;

            public T Value
            {
                get => value;

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

            public DataboundValue() { }

            public DataboundValue(Action onChange = null)
            {
                this.onChange = onChange;
            }


            public DataboundValue(T defaultValue, Action onChange = null)
            {
                this.onChange = onChange;
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


         
        }

    }

    //public static class DataboundAssetExtensions
    //{
    //    public static void Bind<T>(this DataboundAsset asset, DataboundAsset.DataboundValue<T> databoundValue, string bindingName)
    //    {

    //    }
    //}
}
