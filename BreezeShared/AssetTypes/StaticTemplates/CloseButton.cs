using System;
using System.Collections.Generic;
using System.Text;
using Breeze.AssetTypes;
using Breeze.Screens;
using Microsoft.Xna.Framework;

namespace Breeze.AssetTypes.StaticTemplates
{
    public class CloseButton : StaticTemplate
    {
       public new ButtonVisualDescriptor Normal => new ButtonVisualDescriptor()
       {
           FontColor = Color.White,
           BackgroundColor = Color.Red,
           BorderColor = Color.Red,
           FontFamily = Solids.Instance.Fonts.MDL2,
           BorderBrushSize = 0,
           TextJustification = FontAsset.FontJustification.Left
       };


        public new ButtonVisualDescriptor Hover => new ButtonVisualDescriptor()
        {
            FontColor = Color.White,
            BackgroundColor = Color.Red * 0.7f,
            BorderColor = Color.Red,
            FontFamily = Solids.Instance.Fonts.MDL2,
            BorderBrushSize = 0,
            TextJustification = FontAsset.FontJustification.Left
        };

        public new ButtonVisualDescriptor Pressing => new ButtonVisualDescriptor()
        {
            FontColor = Color.White,
            BackgroundColor = Color.Green,
            BorderColor = Color.Red,
            FontFamily = Solids.Instance.Fonts.MDL2,
            BorderBrushSize = 0,
            TextJustification = FontAsset.FontJustification.Left
        };

        public static StaticTemplate Template => new StaticTemplate()
        {
            Disabled = new CloseButton().Disabled,
            Hover = new CloseButton().Hover,
            Normal = new CloseButton().Normal,
            Pressing = new CloseButton().Pressing
        };
    }
}
