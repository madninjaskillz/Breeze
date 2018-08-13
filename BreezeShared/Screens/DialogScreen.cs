using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Breeze.Helpers;
using Breeze.AssetTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace Breeze.Screens
{
    public class DialogScreen : BaseScreen
    {
        private List<Task> loadingTasks;

        private int itemsLoaded = 0;
        private float fontScale = 1f;
        private FontAsset titleFont;
        private RectangleAsset titleContainer;
        public List<StaticButtonAsset> Buttons = new List<StaticButtonAsset>();
        public string Title;
        public string Body;

        public override async Task Initialise()
        {
            IsFullScreen = true;
            DimBackground = false;
            BlurBackground = false;


            await base.Initialise();

            
            titleContainer = new RectangleAsset(Color.White, 0, new FloatRectangle(0.15f, 0.15f, 0.75f, 0.05f), Color.Purple * 0.65f, 8);
            titleFont = new FontAsset(Title, Color.White, new FloatRectangle(0.155f, 0.151f, 0.75f, 0.05f), Solids.Instance.Fonts.EuroStile, FontAsset.FontJustification.Left);
            
            base.FixedAssets.Add(titleContainer);
            base.FixedAssets.Add(titleFont);
            
            base.FixedAssets.Add(new RectangleAsset(0xFF99DBFF.ToColor(), 0, new FloatRectangle(0.15f, 0.2f, 0.75f, 0.48f), 0xFF99DBFF.ToColor() * 0.15f, 18));
            base.FixedAssets.Add(new RectangleAsset(0xFF99DBFF.ToColor(), 0, new FloatRectangle(0.15f, 0.68f, 0.75f, 0.22f), 0xFF99DBFF.ToColor() * 0.25f, 12));

            var bodyLines = Body.ToList();
            float ypos = 0;

            //TODO FIX ME
            //foreach (string bodyLine in bodyLines)
            //{
            //    base.FixedAssets.Add(new FontAsset(bodyLine, Color.White, new FloatRectangle(0.25f, 0.325f+ypos, 0.5f, 0.06f), Solids.FontFamilies.EuroStile, FontAsset.FontJustification.Center) { PseudoAntiAlias = { Value = false } });
            //    ypos = ypos + 0.06f;

            //}
            
            base.FixedAssets.Add(new BoxShadowAsset(Color.White, 0.05f, new FloatRectangle(0.15f, 0.15f, 0.75f, 0.75f)));
            base.FixedAssets.AddRange(Buttons);

            
            FixedAspectRatio = 2.5f;

        }



        public override void Update(GameTime gameTime)
        {
            

            base.Update(gameTime);
        }

    }
}
