using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Breeze.Screens.Splash
{
    public class SplashScreens : BaseScreen
    {
        private SplashViewModel vm;
        public override async Task Initialise()
        {
            await base.Initialise();
            LoadXAML();


            this.RootAsset.FixBinds();
            vm = new SplashViewModel();
            this.RootContext = vm;
            this.RootContext.Screen = this;
            this.IsFullScreen = true;
            UpdateAllAssets();
            Update(new GameTime());
            //  SetBindings();

        }
    }
}
