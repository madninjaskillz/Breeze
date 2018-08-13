using System;
using System.Collections.Generic;
using System.Text;
using Breeze.Screens;
using Microsoft.Xna.Framework;

namespace Breeze.AssetTypes.StaticTemplates
{
    public class TextBox : StaticTemplate
    {
        public new ButtonVisualDescriptor Normal => new ButtonVisualDescriptor() { BorderBrushSize = 2, BorderColor = Color.Black * 0.7f, FontFamily = Solids.Instance.Fonts.SegoeLight, FontColor = Color.Black * 0.7f };
        public new ButtonVisualDescriptor Hover => new ButtonVisualDescriptor() { BorderBrushSize = 1, BorderColor = Color.Green * 0.9f, FontFamily = Solids.Instance.Fonts.SegoeLight, FontColor = Color.Green * 0.9f, BackgroundColor = Color.Black * 0.1f};
        public new ButtonVisualDescriptor Pressing => new ButtonVisualDescriptor() { BorderBrushSize = 2, BorderColor = Color.GreenYellow * 0.5f, FontFamily = Solids.Instance.Fonts.SegoeLight, FontColor = Color.GreenYellow * 0.5f };
        public new ButtonVisualDescriptor Disabled => new ButtonVisualDescriptor() { BorderBrushSize = 2, BorderColor = Color.Gray * 0.35f, FontFamily = Solids.Instance.Fonts.SegoeLight, FontColor = Color.Gray * 0.35f };


        public static StaticTemplate Template => new StaticTemplate()
        {
            Disabled = new TextBox().Disabled,
            Hover = new TextBox().Hover,
            Normal = new TextBox().Normal,
            Pressing = new TextBox().Pressing
        };
    }
}
