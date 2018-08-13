using System;
using System.Collections.Generic;
using System.Text;
using Breeze.AssetTypes;
using Breeze.AssetTypes.StaticTemplates;
using Breeze.Screens;


namespace Breeze.Helpers
{
    public static class StateHelper
    {
        public static ButtonVisualDescriptor GetState(this StaticTemplate template, ButtonState state, bool enabled)
        {
            if (!enabled) return template.Disabled;

            switch (state)
            {
                case ButtonState.Normal:
                {
                    return template.Normal;
                }
                case ButtonState.Hover:
                {
                    return template.Hover;
                }
                case ButtonState.Pressing:
                {
                    return template.Pressing;
                }
            }

            throw new Exception("Who added a new state?");

        }
    }
}
