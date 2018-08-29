using System.Collections.Generic;
using Breeze.FontSystem;
using Breeze.Helpers;
using Breeze.Storage.Helpers;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Breeze
{
    public class AssetLibrary
    {
        private readonly SmartSpriteBatch spriteBatch;
        private readonly Dictionary<string, object> library = new Dictionary<string, object>();

        public AssetLibrary(SmartSpriteBatch sb)
        {
            spriteBatch = sb;
        }

        public Texture2D GetTexture(string filename, bool cache = true)
        {
            string key = filename.ToLower();

            if (library.ContainsKey(key) && cache)
            {
                return (Texture2D)library[key];
            }

            Texture2D tmp = spriteBatch.GraphicsDevice.LoadTexture(filename);
            

            if (cache)
            {
                library.Add(key, tmp);
            }

            return tmp;
        }

        public BMFont GetFont(string fontName)
        {
            string key = fontName.ToLower();

            if (library.ContainsKey(key))
            {
                return (BMFont)library[key];
            }

            BMFont tmp = BMFont.LoadFont(fontName);
            library.Add(key, tmp);

            return tmp;
        }

        public Audio UIAudio = new Audio();

        public class Audio
        {
            public SoundEffect ToastAlert;
        }
    }
}