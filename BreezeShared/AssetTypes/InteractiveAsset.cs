using System;
using System.Reflection;
using Breeze.AssetTypes.DataBoundTypes;
using Microsoft.Xna.Framework;

namespace Breeze.AssetTypes
{
    public class InteractiveAsset : DataboundContainterAsset
    {
        internal Action<ButtonClickEventArgs> InternalClickEvent = null;
        public DataboundValue<ButtonState> State { get; set; } = new DataboundValue<ButtonState>();
        
        public DataboundValue<string> OnClickEvent { get; set; } = new DataboundValue<string>();
        public DataboundValue<string> OnPressEvent { get; set; } = new DataboundValue<string>();
        public DataboundValue<string> OnReleaseEvent { get; set; } = new DataboundValue<string>();
        public Action<ButtonClickEventArgs> InternalPressEvent = null;
        public Action<ButtonClickEventArgs> InternalStickUpEvent = null;
        public Action<ButtonClickEventArgs> InternalStickDownEvent = null;
        public Action<ButtonClickEventArgs> InternalStickLeftEvent = null;
        public Action<ButtonClickEventArgs> InternalStickRightEvent = null;

        public void FireEvent(DataboundValue<string> eventBinding,object[] paramsToSend)
        {
            string eventName = eventBinding.Value();
            if (!string.IsNullOrWhiteSpace(eventName))
            {
                VirtualizedDataContext context = this.VirtualizedDataContext;

                MethodInfo methodInDataContext = context.GetType().GetMethod(eventName);
                if (methodInDataContext != null)
                {
                    methodInDataContext.Invoke(context, paramsToSend);
                }
                else
                {
                    MethodInfo methodInScreen = context.Screen.GetType().GetMethod(eventName);
                    methodInScreen?.Invoke(context.Screen, paramsToSend);
                }
            }
        }
    }
}