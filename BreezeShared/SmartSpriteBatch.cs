using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Breeze
{
    public enum PrimativesOrFakeTextures
    {
        Primatives,
        Textures
    }
    public class SmartSpriteBatch : SpriteBatch
    {
        private readonly List<UserPrimitiveContainer> userPrims = new List<UserPrimitiveContainer>();

        private BasicEffect basicEffect;

        public SpriteSortMode SortMode = SpriteSortMode.Deferred;
        public BlendState BlendState = BlendState.NonPremultiplied;
        public SamplerState SamplerState = null;
        public DepthStencilState DepthStencilState = null;
        //public RasterizerState RasterizerState = null;
        public Effect Effect = null;
        public Matrix? TransformMatrix = null;

        public bool DrawPrimativesInstantly = true;

        public GraphicsDevice GraphicsDevice;
        public bool InBatch;

        public SmartSpriteBatch(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
        }

        public Rectangle? Scissor { get; set; }

        public new void Begin(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null, Rectangle? scissor = null)
        {
            if (rasterizerState == null)
            {
                rasterizerState = GetRasterizerState(scissor);
            }

            base.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
            if (Scissor != null)
            {
                GraphicsDevice.ScissorRectangle = Scissor.Value;
            }
        }


        public new void End()
        {
            base.End();
        }

        public RasterizerState RasterizerState
        {
            get
            {
                if (Scissor.HasValue)
                {
                    return new RasterizerState() { MultiSampleAntiAlias = true, ScissorTestEnable = true, CullMode = CullMode.None };
                }

                return new RasterizerState() { MultiSampleAntiAlias = true, ScissorTestEnable = false, CullMode = CullMode.None };
            }
        }

        public RasterizerState GetRasterizerState(Rectangle? scissor)
        {
            if (scissor.HasValue)
            {
                return new RasterizerState() { MultiSampleAntiAlias = true, ScissorTestEnable = true, CullMode = CullMode.None };
            }

            return new RasterizerState() { MultiSampleAntiAlias = true, ScissorTestEnable = false, CullMode = CullMode.None };
        }

        public void DrawPrimsIfRequired()
        {
            if (userPrims.Count > 0)
            {
                DrawAllUserPrimitives();
            }

        }


        public class UserPrimitiveContainer
        {
            public PrimitiveType PrimitiveType;
            public VertexPositionColor[] VertexData;
            public int VertexOffset;
            public int PrimitiveCount;

            public UserPrimitiveContainer(PrimitiveType primitiveType, VertexPositionColor[] vertexData, int vertexOffset, int primitiveCount)
            {
                this.PrimitiveType = primitiveType;
                this.VertexData = vertexData;
                this.VertexOffset = vertexOffset;
                this.PrimitiveCount = primitiveCount;
            }
        }
        public void DrawUserPrimitives(PrimitiveType primitiveType, VertexPositionColor[] vertexData, int vertexOffset, int primitiveCount)
        {
 //           using (new BenchMark())
            {
                if (!DrawPrimativesInstantly)
                {
                    userPrims.Add(new UserPrimitiveContainer(primitiveType, vertexData, vertexOffset, primitiveCount));
                }
                else
                {
                    DrawUserPrimitives(new List<UserPrimitiveContainer>() {new UserPrimitiveContainer(primitiveType, vertexData, vertexOffset, primitiveCount)});
                }
            }
        }

        public void DrawUserPrimitives(List<UserPrimitiveContainer> uprims)
        {
//            using (new BenchMark("DrawUserPrimitives uprims"))
            {
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, 1);
                Matrix halfPixelOffset = Matrix.CreateTranslation(0.5f, -0.5f, 0);

                if (basicEffect == null)
                {
                    basicEffect = new BasicEffect(Solids.Instance.SpriteBatch.GraphicsDevice)
                    {
                        VertexColorEnabled = true,

                    };
                }

                basicEffect.World = Matrix.Identity;
                basicEffect.View = Matrix.Identity;
                basicEffect.Projection = halfPixelOffset * projection;

                foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    foreach (UserPrimitiveContainer up in uprims)
                    {
                        if (up.VertexData.Length > 0)
                        {
                            this.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(up.PrimitiveType, up.VertexData, up.VertexOffset, up.PrimitiveCount);
                        }
                    }
                }
            }
        }




        public void DrawUserPrimitives(List<UserPrimitiveContainer> uprims, Vector2 offset)
        {
 //           using (new BenchMark("DrawUserPrimitives uprims offset"))
            {
                Matrix projection = Matrix.CreateOrthographicOffCenter(offset.X, GraphicsDevice.Viewport.Width+ offset.X, GraphicsDevice.Viewport.Height+offset.Y, offset.Y, 0, 1);
                Matrix halfPixelOffset = Matrix.CreateTranslation(0.5f, -0.5f, 0);

                if (basicEffect == null)
                {
                    basicEffect = new BasicEffect(Solids.Instance.SpriteBatch.GraphicsDevice)
                    {
                        VertexColorEnabled = true,

                    };
                }

                basicEffect.World = Matrix.Identity;
                basicEffect.View = Matrix.Identity;
                basicEffect.Projection = halfPixelOffset * projection;

                foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    foreach (UserPrimitiveContainer up in uprims)
                    {
                        if (up.VertexData.Length > 0)
                        {
                            this.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(up.PrimitiveType, up.VertexData, up.VertexOffset, up.PrimitiveCount);
                        }
                    }
                }
            }
        }




        public void DrawAllUserPrimitives()
        {
            if (userPrims.Count > 0)
            {
                var xxx = userPrims.ToList();
                userPrims.Clear();


                Matrix projection = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, 1);
                Matrix halfPixelOffset = Matrix.CreateTranslation(0.5f, -0.5f, 0);

                if (basicEffect == null)
                {
                    basicEffect = new BasicEffect(Solids.Instance.SpriteBatch.GraphicsDevice)
                    {
                        VertexColorEnabled = true,

                    };
                }

                basicEffect.World = Matrix.Identity;
                basicEffect.View = Matrix.Identity;
                basicEffect.Projection = halfPixelOffset * projection;

                foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    foreach (UserPrimitiveContainer up in xxx.ToList())
                    {
                        if (up.VertexData.Length > 0)
                        {
                            this.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(up.PrimitiveType, up.VertexData, up.VertexOffset, up.PrimitiveCount);
                        }
                    }
                }
            }
        }

        public void DrawRectangle(Rectangle rectangle, Color color, Color? bgColor = null, FloatRectangle? clip = null)
        {
            DrawRectangle(new FloatRectangle(rectangle), color, bgColor, clip);
        }

        public void DrawRectangle(FloatRectangle rectangle, Color color, Color? bgColor = null, FloatRectangle? clip = null)
        {
            if (bgColor != null)
            {
                DrawSolidRectangle(rectangle, bgColor.Value);
            }

            var rect = rectangle.Clamp(clip);
           // rect = rectangle;
            List<Line> lines = new List<Line>
            {
                BuildLine(new Vector2(rect.X, rect.Y), new Vector2(rect.Right, rect.Y), color, clip),
                BuildLine(new Vector2(rect.Right, rect.Y), new Vector2(rect.Right, rect.Bottom), color, clip),
                BuildLine(new Vector2(rect.Right, rect.Bottom), new Vector2(rect.X, rect.Bottom), color, clip),
                BuildLine(new Vector2(rect.X, rect.Bottom), new Vector2(rect.X, rect.Y), color, clip)
            };

            DrawLines(lines, null);
        }
        public void DrawSolidRectangle(FloatRectangle rect, Color color, FloatRectangle? clip = null)
        {
            var temp = rect.Clamp(clip);
            //temp = rect;
            Vector3 topLeft = new Vector3(temp.X, temp.Y, 0);
            Vector3 bottomLeft = new Vector3(temp.X, temp.Y + temp.Height, 0);
            Vector3 bottomRight = new Vector3(temp.X + temp.Width, temp.Y + temp.Height, 0);
            Vector3 topRight = new Vector3(temp.X + temp.Width, temp.Y, 0);

            VertexPositionColor[] vertices = new VertexPositionColor[6];
            vertices[0] = new VertexPositionColor(topLeft, color);
            vertices[1] = new VertexPositionColor(topRight, color);
            vertices[2] = new VertexPositionColor(bottomRight, color);
            vertices[3] = new VertexPositionColor(bottomRight, color);
            vertices[4] = new VertexPositionColor(bottomLeft, color);
            vertices[5] = new VertexPositionColor(topLeft, color);

            this.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 2);
        }

        public Line BuildLine(Vector2 start, Vector2 end, Color color, FloatRectangle? clip)
        {
            var st = start;//.Clip(clip);
            var nd = end;//.Clip(clip);

            return new Line(st, nd, color, 1);
        }

        public struct Line
        {
            public Vector2 Start;
            public Vector2 End;
            public Color Color;
            public int BrushSize;
            public Line(Vector2 start, Vector2 end, Color color, int brushSize = 1)
            {
                Start = start;
                End = end;
                Color = color;
                BrushSize = brushSize;
            }
        }

        public void DrawLines(List<Line> lines, FloatRectangle? clip = null)
        {
            List<VertexPositionColor> vertices = new List<VertexPositionColor>();

            foreach (Line line in lines)
            {
                Vector2 start = line.Start.Clip(clip);
                Vector2 end = line.End.Clip(clip);

                if (line.BrushSize == 1)
                {
                    vertices.Add(new VertexPositionColor(new Vector3(start.X, start.Y, 0), line.Color));
                    vertices.Add(new VertexPositionColor(new Vector3(end.X, end.Y, 0), line.Color));
                }
                else
                {
                    for (float y = -(line.BrushSize / 2f); y < (line.BrushSize / 2f); y++)
                    {
                        for (float x = -(line.BrushSize / 2f); x < (line.BrushSize / 2f); x++)
                        {
                            vertices.Add(new VertexPositionColor(new Vector3(start.X + x, start.Y + y, 0), line.Color));
                            vertices.Add(new VertexPositionColor(new Vector3(end.X + x, end.Y + y, 0), line.Color));
                        }
                    }
                }
            }

            this.DrawUserPrimitives(PrimitiveType.LineList, vertices.ToArray(), 0, vertices.Count / 2);
        }

        public void DrawLine(Vector2 tstart, Vector2 tend, Color color, FloatRectangle? clip = null, float brushSize = 1)
        {
            Vector2 start = tstart.Clip(clip);
            Vector2 end = tend.Clip(clip);

            List<VertexPositionColor> vertices = new List<VertexPositionColor>();
            if (brushSize == 1)
            {
                vertices.Add(new VertexPositionColor(new Vector3(start.X, start.Y, 0), color));
                vertices.Add(new VertexPositionColor(new Vector3(end.X, end.Y, 0), color));
            }
            else
            {
                for (float y = -(brushSize / 2); y < (brushSize / 2); y++)
                {
                    for (float x = -(brushSize / 2); x < (brushSize / 2); x++)
                    {
                        vertices.Add(new VertexPositionColor(new Vector3(start.X + x, start.Y + y, 0), color));
                        vertices.Add(new VertexPositionColor(new Vector3(end.X + x, end.Y + y, 0), color));
                    }
                }
            }

            this.DrawUserPrimitives(PrimitiveType.LineList, vertices.ToArray(), 0, vertices.Count / 2);
        }

        public void DrawLine(Vector2[] points, Color color, FloatRectangle? clip = null, float brushSize = 1)
        {
            //using (new BenchMark())
            {
                Vector2[] sanityPoints = new Vector2[points.Length];

                for (int i = 0; i < points.Length; i++)
                {
                    sanityPoints[i] = points[i].Clip(clip);
                }


                List<VertexPositionColor> vertices = new List<VertexPositionColor>();

                for (int i = 0; i < points.Length - 1; i = i + 1)
                {
                    Vector2 start = sanityPoints[i];
                    Vector2 end = sanityPoints[i + 1];
                    {
                        if (brushSize == 1)
                        {
                            vertices.Add(new VertexPositionColor(new Vector3(start.X, start.Y, 0), color));
                            vertices.Add(new VertexPositionColor(new Vector3(end.X, end.Y, 0), color));
                        }
                        else
                        {
                            for (float y = -(brushSize / 2); y < (brushSize / 2); y++)
                            {
                                for (float x = -(brushSize / 2); x < (brushSize / 2); x++)
                                {
                                    vertices.Add(new VertexPositionColor(new Vector3(start.X + x, start.Y + y, 0), color));
                                    vertices.Add(new VertexPositionColor(new Vector3(end.X + x, end.Y + y, 0), color));
                                }
                            }
                        }
                    }
                }


                this.DrawUserPrimitives(PrimitiveType.LineList, vertices.ToArray(), 0, vertices.Count / 2);
            }
        }

        public void DrawLine(Vector2[] points, Color fromColor, Color toColor, FloatRectangle? clip = null, float brushSize = 1)
        {
            //using (new BenchMark())
            {
                Vector2[] sanityPoints = new Vector2[points.Length];

                for (int i = 0; i < points.Length; i++)
                {
                    sanityPoints[i] = points[i].Clip(clip);
                }

                Color color = fromColor;

                List<VertexPositionColor> vertices = new List<VertexPositionColor>();

                for (int i = 0; i < points.Length - 1; i = i + 1)
                {
                    color = Color.Lerp(fromColor, toColor, i / (float) points.Length);

                    Vector2 start = sanityPoints[i];
                    Vector2 end = sanityPoints[i + 1];
                    {
                        if (brushSize == 1)
                        {
                            vertices.Add(new VertexPositionColor(new Vector3(start.X, start.Y, 0), color));
                            vertices.Add(new VertexPositionColor(new Vector3(end.X, end.Y, 0), color));
                        }
                        else
                        {
                            for (float y = -(brushSize / 2); y < (brushSize / 2); y++)
                            {
                                for (float x = -(brushSize / 2); x < (brushSize / 2); x++)
                                {
                                    vertices.Add(new VertexPositionColor(new Vector3(start.X + x, start.Y + y, 0), color));
                                    vertices.Add(new VertexPositionColor(new Vector3(end.X + x, end.Y + y, 0), color));
                                }
                            }
                        }
                    }
                }


                this.DrawUserPrimitives(PrimitiveType.LineList, vertices.ToArray(), 0, vertices.Count / 2);
            }
        }

        public void DoBlur(int range)
        {
            //Texture2D ss = TextureHelpers.GetScreenShot();

            //Texture2D ss = Solids.DefaultScreenTarget;

            // var ss = Solids.AssetLibrary.GetTexture("Images\\loadingLogo.png");

            //DoEnd();
            //var effect = Solids.GaussianBlur.Effect;

            //effect.Parameters["range"].SetValue(range);
            //effect.Parameters["wifth"].SetValue((float)ss.Width);
            //effect.Parameters["hight"].SetValue((float)ss.Height);

            //// TODO: Add your drawing code here
            //base.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, effect);

            //base.Draw(ss, Solids.Instance.SpriteBatch.GraphicsDevice.Viewport.Bounds,null, Color.White);
            //base.End();







            //effect.CurrentTechnique = effect.Techniques["Technique1"];
            //effect.Parameters["wifth"].SetValue((float)ss.Width);
            //effect.Parameters["hight"].SetValue((float)ss.Height);

            //base.Begin(0,BlendState.Opaque, null, null, null, effect);
            //Draw(ss,GraphicsDevice.Viewport.Bounds,Color.Pink);
            //base.End();

        }

        //
        // Summary:
        //     Submit a sprite for drawing in the current batch.
        //
        // Parameters:
        //   texture:
        //     A texture.
        //
        //   destinationRectangle:
        //     The drawing bounds on screen.
        //
        //   color:
        //     A color mask.
        public new void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
        {
            //DoBegin();
            base.Draw(texture, destinationRectangle, color);
        }

        public new void Draw(Rectangle destinationRectangle, Color color)
        {
            //   DoBegin();
            base.Draw(Breeze.Solids.Pixel, destinationRectangle, color);
        }

        //
        // Summary:
        //     Submit a sprite for drawing in the current batch.
        //
        // Parameters:
        //   texture:
        //     A texture.
        //
        //   position:
        //     The drawing location on screen.
        //
        //   color:
        //     A color mask.
        public new void Draw(Texture2D texture, Vector2 position, Color color)
        {
            //  DoBegin();
            base.Draw(texture, position, color);
        }

        //
        // Summary:
        //     Submit a sprite for drawing in the current batch.
        //
        // Parameters:
        //   texture:
        //     A texture.
        //
        //   position:
        //     The drawing location on screen.
        //
        //   sourceRectangle:
        //     An optional region on the texture which will be rendered. If null - draws full
        //     texture.
        //
        //   color:
        //     A color mask.
        public new void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
        {
            //  DoBegin();
            base.Draw(texture, position, sourceRectangle, color);
        }
        //
        // Summary:
        //     Submit a sprite for drawing in the current batch.
        //
        // Parameters:
        //   texture:
        //     A texture.
        //
        //   destinationRectangle:
        //     The drawing bounds on screen.
        //
        //   sourceRectangle:
        //     An optional region on the texture which will be rendered. If null - draws full
        //     texture.
        //
        //   color:
        //     A color mask.
        //
        //   rotation:
        //     A rotation of this sprite.
        //
        //   origin:
        //     Center of the rotation. 0,0 by default.
        //
        //   effects:
        //     Modificators for drawing. Can be combined.
        //
        //   layerDepth:
        //     A depth of the layer of this sprite.
        public new void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
        {
            //   DoBegin();
            base.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth);
        }

        //
        // Summary:
        //     Submit a sprite for drawing in the current batch.
        //
        // Parameters:
        //   texture:
        //     A texture.
        //
        //   destinationRectangle:
        //     The drawing bounds on screen.
        //
        //   sourceRectangle:
        //     An optional region on the texture which will be rendered. If null - draws full
        //     texture.
        //
        //   color:
        //     A color mask.
        public new void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        {
            //   DoBegin();
            base.Draw(texture, destinationRectangle, sourceRectangle, color);
        }

        //
        // Summary:
        //     Submit a sprite for drawing in the current batch.
        //
        // Parameters:
        //   texture:
        //     A texture.
        //
        //   position:
        //     The drawing location on screen.
        //
        //   sourceRectangle:
        //     An optional region on the texture which will be rendered. If null - draws full
        //     texture.
        //
        //   color:
        //     A color mask.
        //
        //   rotation:
        //     A rotation of this sprite.
        //
        //   origin:
        //     Center of the rotation. 0,0 by default.
        //
        //   scale:
        //     A scaling of this sprite.
        //
        //   effects:
        //     Modificators for drawing. Can be combined.
        //
        //   layerDepth:
        //     A depth of the layer of this sprite.
        public new void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            //   DoBegin();
            base.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }

        //
        // Summary:
        //     Submit a sprite for drawing in the current batch.
        //
        // Parameters:
        //   texture:
        //     A texture.
        //
        //   position:
        //     The drawing location on screen or null if destinationRectangle
        //
        //   destinationRectangle:
        //     The drawing bounds on screen or null if position
        //
        //   sourceRectangle:
        //     An optional region on the texture which will be rendered. If null - draws full
        //     texture.
        //
        //   origin:
        //     An optional center of rotation. Uses Microsoft.Xna.Framework.Vector2.Zero if
        //     null.
        //
        //   rotation:
        //     An optional rotation of this sprite. 0 by default.
        //
        //   scale:
        //     An optional scale vector. Uses Microsoft.Xna.Framework.Vector2.One if null.
        //
        //   color:
        //     An optional color mask. Uses Microsoft.Xna.Framework.Color.White if null.
        //
        //   effects:
        //     The optional drawing modificators. Microsoft.Xna.Framework.Graphics.SpriteEffects.None
        //     by default.
        //
        //   layerDepth:
        //     An optional depth of the layer of this sprite. 0 by default.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     Throwns if both position and destinationRectangle been used.
        //
        // Remarks:
        //     This overload uses optional parameters. This overload requires only one of position
        //     and destinationRectangle been used.
        [Obsolete("In future versions this method can be removed.")]
        public new void Draw(Texture2D texture, Vector2? position = default(Vector2?), Rectangle? destinationRectangle = default(Rectangle?), Rectangle? sourceRectangle = default(Rectangle?), Vector2? origin = default(Vector2?), float rotation = 0, Vector2? scale = default(Vector2?), Color? color = default(Color?), SpriteEffects effects = SpriteEffects.None, float layerDepth = 0)
        {
            //   DoBegin();
            base.Draw(texture, position = default(Vector2?), destinationRectangle = default(Rectangle?), sourceRectangle = default(Rectangle?), origin = default(Vector2?), rotation = 0, scale = default(Vector2?), color = default(Color?), effects = SpriteEffects.None, layerDepth = 0);
        }

        //
        // Summary:
        //     Submit a sprite for drawing in the current batch.
        //
        // Parameters:
        //   texture:
        //     A texture.
        //
        //   position:
        //     The drawing location on screen.
        //
        //   sourceRectangle:
        //     An optional region on the texture which will be rendered. If null - draws full
        //     texture.
        //
        //   color:
        //     A color mask.
        //
        //   rotation:
        //     A rotation of this sprite.
        //
        //   origin:
        //     Center of the rotation. 0,0 by default.
        //
        //   scale:
        //     A scaling of this sprite.
        //
        //   effects:
        //     Modificators for drawing. Can be combined.
        //
        //   layerDepth:
        //     A depth of the layer of this sprite.
        public new void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            //   DoBegin();
            base.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }
    }

    public class SmartSpriteBatchManager : IDisposable
    {
        private readonly SmartSpriteBatch smartSpriteBatch;

        public SmartSpriteBatchManager(SmartSpriteBatch smartSpriteBatch, SpriteSortMode sortMode = SpriteSortMode.Immediate, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null, FloatRectangle? scissorRect = null)
        {
            this.smartSpriteBatch = smartSpriteBatch;
            if (blendState == null)
            {
                blendState = BlendState.NonPremultiplied;
            }

            if (scissorRect.HasValue)
            {
                this.smartSpriteBatch.GraphicsDevice.ScissorRectangle = scissorRect.ToRectangle();
            }
            
            this.smartSpriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix,scissorRect.ToRectangle());
        }

        public void Dispose()
        {
            this.smartSpriteBatch.End();
            this.smartSpriteBatch.GraphicsDevice.ScissorRectangle = Solids.Instance.Bounds;
        }
    }

    public class SmartScissorRect : IDisposable
    {
        private readonly Rectangle defaultScissorRectangle;
        public SmartScissorRect(Rectangle newScissorRector)
        {
            Solids.Instance.SpriteBatch.End();
            defaultScissorRectangle = Solids.Instance.SpriteBatch.GraphicsDevice.ScissorRectangle;
            Solids.Instance.SpriteBatch.GraphicsDevice.ScissorRectangle = newScissorRector;

        }
        public void Dispose()
        {
            Solids.Instance.SpriteBatch.End();
            Solids.Instance.SpriteBatch.GraphicsDevice.ScissorRectangle = defaultScissorRectangle;
        }
    }
}
