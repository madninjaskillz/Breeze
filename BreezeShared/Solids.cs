using System;
using System.Collections.Generic;
using System.Text;
using Breeze.AssetTypes.DataBoundTypes;
using Breeze.FontSystem;
using Breeze.Helpers;
using Breeze.Services.InputService;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Breeze
{
    internal static class Solids
    {
        //todo - most of this should be in the instance with an easy way to grab it
        internal static Random Random = new Random();

        internal static GaussianBlur GaussianBlur
        {
            get => Instance.GaussianBlur;
            set => Instance.GaussianBlur = value;
        }
        public static Rectangle Bounds { get; set; }
        internal static Dictionary<string, DataboundAsset> GlobalTemplates = new Dictionary<string, DataboundAsset>();
        
        public static string AssetBase { get; set; } = "";
        //internal static SmartSpriteBatch SpriteBatch { get; set; }
        public static Texture2D Pixel;
        public static float MaxBlur { get; set; } = 12f;
        public static SamplerState SamplerState = SamplerState.AnisotropicClamp;
        public static Settings Settings = new Settings();
        public static bool TextboxVisible { get; set; }
        //TODO this feels dirty
        internal static BreezeInstance Instance { get; set; }
    }


    public class Settings
    {
        public string KBLanguage { get; set; } = "USA";
#if WINDOWS_UAP
        public bool EnableBlur { get; set; } = true;
#else
    public bool EnableBlur { get; set; } = false;
#endif
        public bool UseOSK { get; set; } = false;
    }
}
