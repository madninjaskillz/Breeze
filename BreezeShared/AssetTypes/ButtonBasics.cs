using System;
using Breeze.AssetTypes;
using Microsoft.Xna.Framework;

namespace Breeze.Screens
{
    public class ButtonBasics
    {
        public Color? OverrideBgColor;
        public string Text { get; set; }
        public Action<ButtonClickEventArgs> ClickAction { get; set; }
        public FontSystem.MDL2Symbols? Symbol { get; set; }
        public bool Enabled { get; set; } = true;
        public ButtonBasics(string text, Action<ButtonClickEventArgs> action = null, bool enabled = true)
        {
            Text = text;
            ClickAction = action;
            Enabled = enabled;
        }

        public ButtonBasics(FontSystem.MDL2Symbols symbol, string text, Action<ButtonClickEventArgs> action = null, bool enabled = true)
        {
            Symbol = symbol;
            Text = text;
            ClickAction = action;
            Enabled = enabled;
        }
    }
}
