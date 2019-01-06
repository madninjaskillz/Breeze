using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze;
using Breeze.AssetTypes;
using Breeze.AssetTypes.DataBoundTypes;
using Breeze.FontSystem;
using Breeze.Screens;

namespace BreezeDemo.Screens.Demo1
{
    public class Demo1VirtualizedContext : VirtualizedDataContext
    {
        private string testText  = "ooh hello";

        [Databound]
        public string TestText
        {
            get => testText;
            set => Set(ref testText, value);
        }

        private IEnumerable<ButtonItem> buttons = new List<ButtonItem>();

        [Databound]
        public IEnumerable<ButtonItem> Buttons
        {
            get => buttons;
            set => Set(ref buttons, value);
        }

        private void PopulateButtons()
        {
            var buttons = new List<ButtonItem>();

            buttons.Add(new ButtonItem(MDL2Symbols.BlockContact, "Block"));
            buttons.Add(new ButtonItem(MDL2Symbols.ClosePane2, "Close pane 2"));
            buttons.Add(new ButtonItem(MDL2Symbols.Like, "Like"));

            Buttons = buttons;
        }

        public Demo1VirtualizedContext()
        {
            PopulateButtons();
        }

        public class ButtonItem : VirtualizedDataContext
        {
            private string text;

            [Databound]
            public string Text
            {
                get => text;
                set => Set(ref text, value);
            }

            private string symbol;

            [Databound]
            public string Symbol
            {
                get => symbol;
                set => Set(ref symbol, value);
            }

            public ButtonItem(MDL2Symbols symbol, string buttonText)
            {
                this.Symbol = symbol.ToString();
                this.Text = buttonText;
            }
        }
    }
}
