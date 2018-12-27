using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.Screens;
using Microsoft.Xna.Framework;

namespace BreezeDemo.Screens.Demo1
{
    public class Demo1Screen : DataboundScreen
    {
        public override async Task Initialise()
        {
            DataContext = new Demo1ViewModel();
            Demo1ViewModel vm = (Demo1ViewModel)DataContext;
            await base.Initialise();
            LoadXAML();
            this.IsFullScreen = true;
            UpdateAllAssets();
            Update(new GameTime());
            SetBindings();
            Debug.WriteLine(this.AllAssets);
        }
    }
}
