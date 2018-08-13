//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Xml.Serialization;
//using Breeze.Helpers;
//using Breeze.Screens;
//using Breeze.Services.InputService;
//using Breeze.AssetTypes.DataBoundTypes;
//using BreezeModels;
//using MachineInterface;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;

//#if WINDOWS_UAP
//using Windows.Foundation;
//#endif

//namespace Breeze.AssetTypes
//{
//    public class PatternAsset : DataboundAsset
//    {
//        private int delayedRegen = -1;

//        public PatternAsset()
//        {
//        }


//        public PatternAsset(Color color, int brushSize, FloatRectangle position, ChannelContainer channel, PatternInstance patternInstance, Pattern pattern, int sampleWidth, int renderDelay, float zoom, Color? backgroundColor = null)
//        {
//            Color.Value = color;
//            BrushSize.Value = brushSize;
//            Position.Value = position;
//            BackgroundColor.Value = backgroundColor;
//            Channel.Value = channel;
//            Pattern.Value = pattern;
//            PatternInstance.Value = patternInstance;
//            SampleWidth.Value = sampleWidth;
//            RenderDelay.Value = renderDelay;
//            Zoom.Value = zoom;

//            Position.SetChangeAction(Update);
//            Zoom.SetChangeAction(Update);
//            BrushSize.SetChangeAction(Update);
//            Color.SetChangeAction(Update);
//            BackgroundColor.SetChangeAction(Update);
//            Pattern.SetChangeAction(Update);
//            PatternInstance.SetChangeAction(Update);
//        }

//        private void Update()
//        {
//            throw new NotImplementedException();
//        }

//        private SmartSpriteBatch _spriteBatch;
//        private int generatedSampleWidth;
//        public DataboundValue<FloatRectangle> Position { get; set; } = new DataboundValue<FloatRectangle>();

//        [XmlIgnore]
//        public DataboundValue<Pattern> Pattern { get; set; } = new DataboundValue<Pattern>();
//        public DataboundValue<PatternInstance> PatternInstance { get; set; } = new DataboundValue<PatternInstance>();
//        [XmlIgnore]
//        public DataboundValue<ChannelContainer> Channel { get; set; } = new DataboundValue<ChannelContainer>();
//        public DataboundValue<Color?> BackgroundColor { get; set; }=new DataboundValue<Color?>();
//        public DataboundValue<Color> Color { get; set; }=new DataboundValue<Color>();
//        public DataboundValue<int> BrushSize { get; set; }=new DataboundValue<int>();
//        public DataboundValue<float> Scale { get; set; } = new DataboundValue<float>(1);
//        [XmlIgnore]
//        public DataboundValue<RenderTarget2D[]> RenderTargets = new DataboundValue<RenderTarget2D[]>();
//        public DataboundValue<int> RenderDelay = new DataboundValue<int>();
//        public DataboundValue<float> Zoom = new DataboundValue<float>();
//        public DataboundValue<int> SampleWidth { get; } = new DataboundValue<int>();

//        private int[] tops;
//        private int[] bottoms;

//        private int generatedWidth = 0;
//        private float generatedZoom = 0;


//        //internal void Update(PatternAsset patternAsset)
//        //{
//        //    Position = patternAsset.Position;
//        //    Zoom = patternAsset.Zoom;
//        //}

//        //internal void Update(FloatRectangle position, float zoom)
//        //{
//        //    Position = position;
//        //    Zoom = zoom;
//        //}

//        private void Generate(ScreenAbstractor screen, SmartSpriteBatch spriteBatch)
//        {
//            try
//            {
//                var ps = screen.Translate(Position.Value);

//                int pixelWidth = (int)ps.Value.Width;

//                float instanceTickWidth = PatternInstance.Value.TickEnd - PatternInstance.Value.TickStart;

//                var tttt = pixelWidth / instanceTickWidth;

//                int pw = (int)(tttt * Pattern.Value.TickLength);

//                var xtops = new int[pw / SampleWidth.Value];
//                var xbottoms = new int[pw / SampleWidth.Value];

//                int sampleLength = (int)(16 * Solids.Song.TicksToSample(instanceTickWidth));
//                float actualSamplesPerPixel = (float)sampleLength / (float)pixelWidth;
//                float asppdoubled = SampleWidth.Value * actualSamplesPerPixel;
      
//                int mid = (int)(ps.Value.Height / 2);
//                int ht = (int)(ps.Value.Height / 2);

//                Int16[] bounce = Pattern.Value.GetBounce((IMachine)Channel.Value.Machine).BounceDownLeft.From16BitByteArrayToInt16Array();
//                int aspddd = (int)Math.Max(asppdoubled, 1);
//                for (int x = 0; x < pw / SampleWidth.Value; x++)
//                {
//                    int tickStart = (int)((x * asppdoubled) + PatternInstance.Value.GetSampleOffset()) % bounce.Length;

//                    Int16[] samples = bounce.WrappedCopy(tickStart, aspddd);

//                    float mx = samples.Max() / (float)Int16.MaxValue;
//                    float mn = samples.Min() / (float)Int16.MaxValue;

//                    int tp = (int)(mid - (mx * ht));
//                    int bt = (int)(mid - (mn * ht));

//                    xtops[x] = tp;
//                    xbottoms[x] = bt;
//                }
//                tops = xtops;
//                bottoms = xbottoms;
//                generatedWidth = pixelWidth;
//                generatedZoom = Zoom.Value;
//                delayedRegen = 0;
//                justRendered = true;
//                generatedSampleWidth = SampleWidth.Value;
//            }
//            catch { }

//            Solids.IsCurrentlyGeneratingPatternImage = false;
//        }

//        //TODO - this is no longer called :(
//        public void Update(SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null)
//        {
//            if (Solids.IsCurrentlyGeneratingPatternImage) return;

//            FloatRectangle? ps = screen.Translate(Position.Value);
//            if (ps != null && (int)ps.Value.Width == generatedWidth && SampleWidth.Value == generatedSampleWidth)
//            {
//                delayedRegen = 0;
//                return;
//            }
//            if (Solids.IsCurrentlyGeneratingPatternImage == false)
//            {
//                delayedRegen++;
//            }

//            if (delayedRegen > RenderDelay.Value && !Solids.IsCurrentlyGeneratingPatternImage)
//            {
//                Solids.IsCurrentlyGeneratingPatternImage = true;

//#if WINDOWS_UAP
//                IAsyncAction asyncAction = Windows.System.Threading.ThreadPool.RunAsync(
//                    (workItem) =>
//                    {
//                        Generate(screen, _spriteBatch);
//                    });
//#else
//                Generate(screen, _spriteBatch);
//#endif
//            }
//        }


//        public override void Draw(SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
//        {
//            FloatRectangle? ps = screen.Translate(Position.Value.Move(scrollOffset));

//            clip = screen.Translate(clip);

//            if (clip.HasValue)
//            {
//                if (ps.Value.Right < clip.Value.X || ps.Value.X > clip.Value.Right)
//                {
//                    return;
//                }
//            }

//            Rectangle tmp = screen.Translate(Position.Value.Move(scrollOffset)).ToRectangle();

//            if (clip.HasValue)
//            {
//                if (tmp.Bottom < clip.Value.Y || tmp.Y > clip.Value.Bottom)
//                {
//                    return;
//                }
//            }

//            Rectangle clipped = tmp.Clip(clip);

//            if (clipped.X == clipped.Right)
//            {
//                return;
//            }

//            if (generatedWidth > 0)
//            {
//                if ((ps.Value.Width != rendered.Width&& ps.Value.Height != rendered.Height) || justRendered)
//                {
//                    SetVerticesAtZero(screen, ps.Value, clip.Value);
//                    justRendered = false;
//                }

//                if (vertices.Count > 2)
//                {
//                    //spriteBatch.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

//                    spriteBatch.DrawUserPrimitives(new List<SmartSpriteBatch.UserPrimitiveContainer>() { new SmartSpriteBatch.UserPrimitiveContainer(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2) }, this.Position.Value.ToVector2);
//                }
//            }



//            spriteBatch.DrawSolidRectangle(new FloatRectangle(
//                (int)(ps.Value.Clamp(clip).X),
//                (int)(ps.Value.Clamp(clip).Y + (ps.Value.Clamp(clip).Height) / 2f) - 2,
//                (int)(ps.Value.Clamp(clip).Width), 2),
//            Color.Value * 0.5f);

//            //spriteBatch.DrawLine(
//            //    new Vector2(ps.Value.Clamp(clip.Value).X, ps.Value.Clamp(clip.Value).Y+(ps.Value.Clamp(clip.Value).Height) / 2f),
//            //    new Vector2(ps.Value.Clamp(clip.Value).X + (ps.Value.Clamp(clip.Value).Width), ps.Value.Clamp(clip.Value).Y + (ps.Value.Clamp(clip.Value).Height) / 2f),
//            //    Color.Black * 0.5f,
//            //    null);
//        }

//        private bool justRendered;
//        List<VertexPositionColor> vertices = new List<VertexPositionColor>();
//        private void SetVertices(ScreenAbstractor screen, FloatRectangle ps, FloatRectangle clip)
//        {
//            int pixelWidth = (int)ps.Width;
//            bool first = true;
//            int minX = 0;
//            int maxX = (int)screen.bounds.BottomRight.X;

//            float widthFactor = ps.Width / generatedWidth;
//            widthFactor = Zoom.Value / generatedZoom;
//            vertices = new List<VertexPositionColor>();

//            float reqWidth = pixelWidth - 1 - SampleWidth.Value;
//            float startI = 0;
//            float startPos = (int)ps.X;
//            if (startPos < 0)
//            {
//                float dif = (0 - startPos);
//                startI = dif;
//                startPos = startPos + dif;
//                reqWidth = reqWidth - dif;
//            }

//            if (startPos < clip.X)
//            {
//                float dif = (clip.X - startPos);
//                startI = startI + dif;
//                startPos = startPos + dif;
//                reqWidth = reqWidth - dif;
//            }

//            if (startPos + reqWidth > screen.bounds.Width)
//            {
//                reqWidth = (int)screen.bounds.Width - startPos;
//            }


//            if (startPos + reqWidth > clip.BottomRight.X)
//            {
//                reqWidth = (int)clip.BottomRight.X - startPos;
//            }

//            if (startPos > 0 && reqWidth > 0)
//            {
//                float sourcePixel = ((float)(((startI) / widthFactor) / SampleWidth.Value));
//                int sp = (int)(sourcePixel - 0.499999f) % tops.Length;

//                vertices.Add(new VertexPositionColor(new Vector3(ps.X + startI, Math.Min(Math.Max(ps.Y + bottoms[(int)sp], clip.Y), clip.Bottom), 0), Color.Value));
//                vertices.Add(new VertexPositionColor(new Vector3(ps.X + startI, Math.Min(Math.Max(ps.Y + tops[(int)sp] - 1, clip.Y), clip.Bottom), 0), Color.Value));

//                float mxb = bottoms.Max();

//                for (int i = 1; i < reqWidth; i = i + SampleWidth.Value)
//                {
//                    sourcePixel = ((float)(((i + startI) / widthFactor) / SampleWidth.Value));
//                    sp = (int)(sourcePixel - 0.499999f) % tops.Length;
//                    float height = bottoms[(int)sp] / mxb;

//                    vertices.Add(new VertexPositionColor(new Vector3(ps.X + i + startI + SampleWidth.Value, Math.Min(Math.Max(ps.Y + bottoms[(int)sp], clip.Y), clip.Bottom), 0), Color.Value * height));
//                    vertices.Add(new VertexPositionColor(new Vector3(ps.X + i + startI + SampleWidth.Value, Math.Min(Math.Max(ps.Y + tops[(int)sp] - 1, clip.Y), clip.Bottom), 0), Color.Value * height));

//                }
//            }

//            rendered = ps;
//        }

//        private void SetVerticesAtZero(ScreenAbstractor screen, FloatRectangle ps, FloatRectangle clip)
//        {
//            int pixelWidth = (int)ps.Width;
//            bool first = true;
//            int minX = 0;
//            int maxX = (int)screen.bounds.BottomRight.X;

//            float widthFactor = ps.Width / generatedWidth;
//            widthFactor = Zoom.Value / generatedZoom;
//            vertices = new List<VertexPositionColor>();

//            float reqWidth = pixelWidth - 1 - SampleWidth.Value;
            
//            float startPos = 0;

//            if (startPos + reqWidth > clip.BottomRight.X)
//            {
//                reqWidth = (int)clip.BottomRight.X - startPos;
//            }

//            if (reqWidth > 0)
//            {
//                int sp = 0;// (int)(sourcePixel - 0.499999f) % tops.Length;

//                vertices.Add(new VertexPositionColor(new Vector3(0 , bottoms[(int)sp], 0), Color.Value));
//                vertices.Add(new VertexPositionColor(new Vector3(0 , tops[(int)sp] - 1, 0), Color.Value));

//                float mxb = bottoms.Max();

//                for (int i = 1; i < reqWidth; i = i + SampleWidth.Value)
//                {
//                    float sourcePixel = ((float)(((i ) / widthFactor) / SampleWidth.Value));// ((float)(((startI) / widthFactor) / SampleWidth));
//                    sp = (int)(sourcePixel - 0.499999f) % tops.Length;
//                    float height = bottoms[(int)sp] / mxb;

//                    vertices.Add(new VertexPositionColor(new Vector3(ps.X + i  + SampleWidth.Value, bottoms[(int)sp],0), Color.Value * height));
//                    vertices.Add(new VertexPositionColor(new Vector3(ps.X + i  + SampleWidth.Value, tops[(int)sp] - 1, 0), Color.Value * height));

//                }
//            }

//            rendered = ps;
//        }

//        private FloatRectangle rendered;
//    }
//}
