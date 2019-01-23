using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze;
using Breeze.AssetTypes;
using Breeze.Screens;
using Microsoft.Xna.Framework;

namespace BreezeDemo.Screens.Demo1
{
    public class Demo1Screen : BaseScreen
    {
        private Demo1VirtualizedContext vm;
        public override async Task Initialise()
        {
            await base.Initialise();
            LoadXAML();


            this.RootAsset.FixBinds();
            vm = new Demo1VirtualizedContext();
            this.RootContext = vm;
            this.RootContext.Screen = this;
            this.IsFullScreen = true;
            UpdateAllAssets();
            Update(new GameTime());
            //  SetBindings();

        }

        public void TestButtonClick(ButtonClickEventArgs args)
        {
            ButtonAsset button = args.Sender as ButtonAsset;
            Demo1VirtualizedContext context = (Demo1VirtualizedContext)button.VirtualizedDataContext;

            Debug.WriteLine("OH HAI THERE!");

            context.TestText = context.TestText + "!";
        }
    }
}
