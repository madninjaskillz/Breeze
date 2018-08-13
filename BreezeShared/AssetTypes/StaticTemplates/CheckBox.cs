using System;
using System.Collections.Generic;
using System.Text;
using Breeze.FontSystem;
using Breeze.Screens;
using Microsoft.Xna.Framework;

namespace Breeze.AssetTypes.StaticTemplates
{
    public  class CheckBox : StaticTemplate
    {
        public new ButtonVisualDescriptor Normal => new ButtonVisualDescriptor() { BorderBrushSize = 0, BorderColor = Color.Transparent, FontFamily = Solids.Instance.Fonts.Segoe, FontColor = Color.Black * 0.8f, ShadowDepth = 0 };
        public new ButtonVisualDescriptor Hover => new ButtonVisualDescriptor() { BorderBrushSize = 0, BorderColor = Color.Transparent, FontFamily = Solids.Instance.Fonts.Segoe, FontColor = Color.Green * 0.9f, ShadowDepth = 0, BackgroundColor = Color.Black * 0.1f };
        public new ButtonVisualDescriptor Pressing => new ButtonVisualDescriptor() { BorderBrushSize = 0, BorderColor = Color.Transparent, FontFamily = Solids.Instance.Fonts.Segoe, FontColor = Color.GreenYellow, ShadowDepth = 0 };
        public new ButtonVisualDescriptor Disabled => new ButtonVisualDescriptor() { BorderBrushSize = 0, BorderColor = Color.Transparent, FontFamily = Solids.Instance.Fonts.Segoe, FontColor = Color.Gray * 0.3f, ShadowDepth = 0 };


        public static StaticTemplate Template => new StaticTemplate()
        {
            Disabled = new CheckBox().Disabled,
            Hover = new CheckBox().Hover,
            Normal = new CheckBox().Normal,
            Pressing = new CheckBox().Pressing
        };
    }

    public static class CheckBoxExtensions
    {
        public static MDL2Symbols Converter(this bool input)
        {
            return input ? MDL2Symbols.TickBox : MDL2Symbols.Box;
        }
    }
}
