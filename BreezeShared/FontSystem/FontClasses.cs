using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
#if WINDOWS_UWP
using Windows.Storage;
#endif
using Breeze.Helpers;
using Breeze.Helpers;
using Breeze.Storage.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Breeze.FontSystem
{

    [XmlRoot("font")]
    public class FontFile
    {
        [XmlElement("info")]
        public FontInfo Info
        {
            get;
            set;
        }

        [XmlElement("common")]
        public FontCommon Common
        {
            get;
            set;
        }

        [XmlArray("pages")]
        [XmlArrayItem("page")]
        public List<FontPage> Pages
        {
            get;
            set;
        }

        [XmlArray("chars")]
        [XmlArrayItem("char")]
        public List<FontChar> Chars
        {
            get;
            set;
        }

        [XmlArray("kernings")]
        [XmlArrayItem("kerning")]
        public List<FontKerning> Kernings
        {
            get;
            set;
        }
    }


    public class FontInfo
    {
        [XmlAttribute("face")]
        public String Face
        {
            get;
            set;
        }

        [XmlAttribute("size")]
        public Int32 Size
        {
            get;
            set;
        }

        [XmlAttribute("bold")]
        public Int32 Bold
        {
            get;
            set;
        }

        [XmlAttribute("italic")]
        public Int32 Italic
        {
            get;
            set;
        }

        [XmlAttribute("charset")]
        public String CharSet
        {
            get;
            set;
        }

        [XmlAttribute("unicode")]
        public Int32 Unicode
        {
            get;
            set;
        }

        [XmlAttribute("stretchH")]
        public Int32 StretchHeight
        {
            get;
            set;
        }

        [XmlAttribute("smooth")]
        public Int32 Smooth
        {
            get;
            set;
        }

        [XmlAttribute("aa")]
        public Int32 SuperSampling
        {
            get;
            set;
        }

        private Rectangle _Padding;
        [XmlAttribute("padding")]
        public String Padding
        {
            get
            {
                return _Padding.X + "," + _Padding.Y + "," + _Padding.Width + "," + _Padding.Height;
            }
            set
            {
                String[] padding = value.Split(',');
                _Padding = new Rectangle(Convert.ToInt32(padding[0]), Convert.ToInt32(padding[1]), Convert.ToInt32(padding[2]), Convert.ToInt32(padding[3]));
            }
        }

        private Point _Spacing;
        [XmlAttribute("spacing")]
        public String Spacing
        {
            get
            {
                return _Spacing.X + "," + _Spacing.Y;
            }
            set
            {
                String[] spacing = value.Split(',');
                _Spacing = new Point(Convert.ToInt32(spacing[0]), Convert.ToInt32(spacing[1]));
            }
        }

        [XmlAttribute("outline")]
        public Int32 OutLine
        {
            get;
            set;
        }
    }


    public class FontCommon
    {
        [XmlAttribute("lineHeight")]
        public Int32 LineHeight
        {
            get;
            set;
        }

        [XmlAttribute("base")]
        public Int32 Base
        {
            get;
            set;
        }

        [XmlAttribute("scaleW")]
        public Int32 ScaleW
        {
            get;
            set;
        }

        [XmlAttribute("scaleH")]
        public Int32 ScaleH
        {
            get;
            set;
        }

        [XmlAttribute("pages")]
        public Int32 Pages
        {
            get;
            set;
        }

        [XmlAttribute("packed")]
        public Int32 Packed
        {
            get;
            set;
        }

        [XmlAttribute("alphaChnl")]
        public Int32 AlphaChannel
        {
            get;
            set;
        }

        [XmlAttribute("redChnl")]
        public Int32 RedChannel
        {
            get;
            set;
        }

        [XmlAttribute("greenChnl")]
        public Int32 GreenChannel
        {
            get;
            set;
        }

        [XmlAttribute("blueChnl")]
        public Int32 BlueChannel
        {
            get;
            set;
        }
    }


    public class FontPage
    {
        [XmlAttribute("id")]
        public Int32 ID
        {
            get;
            set;
        }

        [XmlAttribute("file")]
        public String File
        {
            get;
            set;
        }
    }


    public class FontChar
    {
        [XmlAttribute("id")]
        public Int32 ID
        {
            get;
            set;
        }

        [XmlAttribute("x")]
        public Int32 X
        {
            get;
            set;
        }

        [XmlAttribute("y")]
        public Int32 Y
        {
            get;
            set;
        }

        [XmlAttribute("width")]
        public Int32 Width
        {
            get;
            set;
        }

        [XmlAttribute("height")]
        public Int32 Height
        {
            get;
            set;
        }

        [XmlAttribute("xoffset")]
        public Int32 XOffset
        {
            get;
            set;
        }

        [XmlAttribute("yoffset")]
        public Int32 YOffset
        {
            get;
            set;
        }

        [XmlAttribute("xadvance")]
        public Int32 XAdvance
        {
            get;
            set;
        }

        [XmlAttribute("page")]
        public Int32 Page
        {
            get;
            set;
        }

        [XmlAttribute("chnl")]
        public Int32 Channel
        {
            get;
            set;
        }
    }


    public class FontKerning
    {
        [XmlAttribute("first")]
        public Int32 First
        {
            get;
            set;
        }

        [XmlAttribute("second")]
        public Int32 Second
        {
            get;
            set;
        }

        [XmlAttribute("amount")]
        public Int32 Amount
        {
            get;
            set;
        }
    }

    public class FontLoader
    {
        public static FontFile Load(String filename)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(FontFile));

            var st = Solids.Instance.Storage.DatfileStorage.GetStream(filename);
            using (TextReader textReader = new StreamReader(st))
            {
                FontFile file = (FontFile)deserializer.Deserialize(textReader);
                return file;
            }
            //      
#if WINDOWS_UWP
            StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            string fullpath = Solids.Instance.ContentPath.GetFilePath(filename);
            using (Stream fs = folder.OpenStreamForReadAsync(Solids.Instance.ContentPathRelative + filename).Result)
            {
                using (TextReader textReader = new StreamReader(fs))
                {
                    FontFile file = (FontFile)deserializer.Deserialize(textReader);
                    return file;
                }
            }

#else
            return filename.DeserializeXMLFromFile<FontFile>().Result; 
#endif

        }
    }

    public class FontFamily
    {
        private Dictionary<int, BMFont> fonts = new Dictionary<int, BMFont>();

        public string FontName { get; set; }
        public string BaseName { get; set; }

        public FontFamily(string fontName, string baseName)
        {
            FontName = fontName;
            BaseName = baseName;

            string path = "Fonts\\";

            var files = Solids.Instance.Storage.DatfileStorage.GetFiles(path, BaseName + "*.fnt");
            var filesFiltered = files.Where(t => "0,1,2,3,4,5,6,7,8,9".Split(',').Contains(t.Substring(BaseName.Length,1))).ToArray();
            foreach (string file in filesFiltered)
            {
//                EzmuzeDebug.WriteLine(file);

                string interestingPart = file;
                //.Substring(path.Length);
                interestingPart = interestingPart.Substring(0, interestingPart.Length - 4);
                if (interestingPart.Length > BaseName.Length)
                {
                    var fnt = BMFont.LoadFont(interestingPart);

                    var sz = fnt.MeasureString("Igj'#" + MDL2Symbols.Add.AsChar()).Y;
                    try
                    {
                        if (!fonts.ContainsKey((int)sz))
                        {
                            fonts.Add((int)sz, fnt);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Write(e.Message);
                    }
                }
            }

//            EzmuzeDebug.WriteLine(fonts.Count + " fonts loaded.");


        }

        public BMFont GetFont(int height)
        {
            //using (new BenchMark())
            {
                if (cachedFontsForHeights.ContainsKey(height))
                {
                    return cachedFontsForHeights[height];
                }

                int bestFound = int.MaxValue;
                int found = 0;

                foreach (int fontsKey in fonts.Keys)
                {
                    if (Math.Abs(fontsKey - height) < bestFound)
                    {
                        bestFound = Math.Abs(fontsKey - height);
                        found = fontsKey;
                    }
                }

                var tmp = fonts.First(t => t.Key == found).Value;

                cachedFontsForHeights.Add(height, tmp);

                return tmp;
            }
        }

        private Dictionary<int, BMFont> cachedFontsForHeights = new Dictionary<int, BMFont>();

        // public void Draw()
    }

    public class BMFont
    {
        private float defaultScale = 1f;
        public Guid Id = Guid.NewGuid();
        public FontFile FontFile;
        public Texture2D FontTexture;
        public float LineSpacing;
        public FontRenderer Renderer;
        public static BMFont LoadFont(string fontName)
        {
            string path = "Fonts\\" + fontName + ".fnt";
            string imagepath = "Fonts\\" + fontName + "_0.png";

            BMFont retval = new BMFont
            {
                FontTexture = Solids.Instance.AssetLibrary.GetTexture(imagepath),
                FontFile = FontLoader.Load(path)
            };


            retval.defaultScale = 32f / retval.FontFile.Info.Size;


            retval.Renderer = new FontRenderer(retval);
            retval.LineSpacing = retval.FontFile.Common.LineHeight;// * retval.defaultScale;
            return retval;
        }

        public void DrawText(SmartSpriteBatch spriteBatch, float x, float y, string text, Color? color = null, float scale = 1, float rotation = 0, float depth = 1, SpriteEffects effect = SpriteEffects.None)
        {
                Renderer.DrawText(spriteBatch, x, y, text, color, scale, rotation, depth, effect);
        }

        public void DrawText(SmartSpriteBatch spriteBatch, Vector2 pos, string text, Color? color = null, Vector2? scale = null, float rotation = 0, float depth = 1, SpriteEffects effect = SpriteEffects.None)
        {
                Renderer.DrawText(spriteBatch, pos.X, pos.Y, text, color, scale, rotation, depth, effect);
        }

        public Vector2 MeasureString(string text)
        {
            return Renderer.MeasureString(text);
        }
    }
}
