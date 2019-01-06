using System.Reflection;
using Breeze.AssetTypes.DataBoundTypes;

namespace Breeze.AssetTypes
{
    public class InteractiveAsset : DataboundContainterAsset
    {
        public DataboundValue<ButtonState> State { get; set; } = new DataboundValue<ButtonState>();
        
        public DataboundValue<string> OnClickEvent { get; set; } = new DataboundValue<string>();

        public void FireEvent(DataboundValue<string> eventBinding,object[] paramsToSend)
        {
            string eventName = eventBinding.Value();

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