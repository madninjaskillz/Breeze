using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Breeze.FontSystem
{
    public class FontRenderer
    {
        public FontRenderer(BMFont font)
        {
            _fontFile = font.FontFile;
            _texture = font.FontTexture;
            _characterMap = new Dictionary<char, FontChar>();

            foreach (var fontCharacter in _fontFile.Chars)
            {
                char c = (char)fontCharacter.ID;
                if (!_characterMap.ContainsKey(c))
                {
                    _characterMap.Add(c, fontCharacter);
                }
            }
        }

        private Dictionary<char, FontChar> _characterMap;
        private FontFile _fontFile;
        private Texture2D _texture;
        public void DrawText(SmartSpriteBatch spriteBatch, float x, float y, string text, Color? color = null, float scale = 1, float rotation = 1, float depth = 1, SpriteEffects effect = SpriteEffects.None)
        {
            if (_texture == null || text == null)
            {
                return;
            }
            Color col = Color.White;
            if (color != null)
                col = (Color)color;
            float currentX = x;
            float currentY = y;
            foreach (char c in text)
            {
                FontChar fontChar;
                if (_characterMap.TryGetValue(c, out fontChar))
                {
                    var sourceRectangle = new Rectangle(fontChar.X, fontChar.Y, fontChar.Width, fontChar.Height);
                    var position = new Vector2(currentX + (fontChar.XOffset * scale), currentY + (fontChar.YOffset * scale));

                    spriteBatch.Draw(_texture, position, sourceRectangle, col, rotation, Vector2.Zero, scale, effect, depth);
                    currentX += (fontChar.XAdvance * scale);
                }
            }
        }

        public void  DrawText(SmartSpriteBatch spriteBatch, float x, float y, string text, Color? color = null, Vector2? scale = null, float rotation = 1, float depth = 1, SpriteEffects effect = SpriteEffects.None, FloatRectangle? clip = null)
        {
            if (_texture == null||text==null)
            {
                return;
            }
            Vector2 scaling = Vector2.One;
            float xscale = 1;
            float yscale = 1;
            if (scale != null)
            {
                xscale = ((Vector2)scale).X;
                yscale = ((Vector2)scale).Y;
                scaling = (Vector2)scale;
            }
            Color col = Color.White;
            if (color != null)
                col = (Color)color;
            float currentX = x;
            float currentY = y;
            foreach (char c in text)
            {
                FontChar fontChar;
                if (_characterMap.TryGetValue(c, out fontChar))
                {
                    Rectangle sourceRectangle = new Rectangle(fontChar.X, fontChar.Y, fontChar.Width, fontChar.Height);
                    Vector2 position = new Vector2(currentX + (fontChar.XOffset * xscale), currentY + (fontChar.YOffset * yscale));
                    Rectangle positionRect = new Rectangle((int)position.X, (int)position.Y, (int)(fontChar.Width * scaling.X), (int)(fontChar.Height * scaling.Y));
                   
                    spriteBatch.Draw(_texture, positionRect, sourceRectangle, col, rotation, Vector2.Zero, effect, depth);

                    currentX += (fontChar.XAdvance * xscale);
                }
            }
        }

        public Vector2 MeasureString(string text)
        {
            if (text == null)
            {
                return Vector2.Zero;
            }

            float currentX = 0;
            float maxY = 0;
            foreach (char c in text)
            {
                FontChar fontChar;
                if (_characterMap.TryGetValue(c, out fontChar))
                {
                    currentX += (fontChar.XAdvance);
                    if (fontChar.YOffset + fontChar.Height > maxY) maxY = fontChar.YOffset + fontChar.Height;
                }
            }

            return new Vector2(currentX, maxY);
        }
    }

}
