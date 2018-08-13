using Breeze.AssetTypes.DataBoundTypes;

namespace Breeze.AssetTypes
{
    public class InteractiveAsset : DataboundContainterAsset
    {
        public DataboundValue<ButtonState> State { get; set; } = new DataboundValue<ButtonState>();
        
        
    }
}