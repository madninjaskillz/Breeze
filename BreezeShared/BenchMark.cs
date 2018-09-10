using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Breeze
{
    public static class BenchMarkProvider
    {
        public static Dictionary<string, BenchMarkDetails> BenchMarks = new Dictionary<string, BenchMarkDetails>();

        public static int FramePointer = 0;
        public static int Steps = 32;

        public static void NextFrame()
        {

            FPSLog[FramePointer % 32] = Solids.Instance.FrameCounter.CurrentFramesPerSecond;
            FramePointer++;
            //foreach (var b in BenchMarks.Values)
            //{
            //    if (b.History[FramePointer % Steps] > 0)
            //    {
            //        b.History[(FramePointer) % Steps] = -1;
            //    }
            //}
        }

        public static float[] FPSLog = new float[256];
    }



    public class BenchMarkDetails
    {
        public TimeSpan FrameTime { get; set; } = TimeSpan.Zero;
        public string Key { get; set; }
        public int NumberOfTimesRun { get; set; } = 0;
        public TimeSpan ShortestTime { get; set; } = TimeSpan.MaxValue;
        public TimeSpan LongestTime { get; set; } = TimeSpan.Zero;
        public TimeSpan TotalTime { get; set; } = TimeSpan.Zero;
        public int[] History { get; set; } = new int[512];
        public double[] FrameHistory { get; set; } = new double[512];
        public TimeSpan[] FrameHistorySpan { get; set; } = new TimeSpan[512];
        public TimeSpan AverageTime => NumberOfTimesRun == 0 ? TimeSpan.Zero : TimeSpan.FromMilliseconds(TotalTime.TotalMilliseconds / NumberOfTimesRun);
        public Color Color { get; set; } = Color.White;
    }

    public class BenchMark : IDisposable
    {
        public string Key { get; set; }
        public DateTime StartTime { get; set; }
        //public BenchMark([CallerMemberName] string key = null)
        //{
        //    Key = key;
        //    StartTime = DateTime.Now;
        //}

        public BenchMark(string txt ="",[CallerMemberName] string key = null)
        {
            Key = key;
            if (!string.IsNullOrWhiteSpace(txt))
            {
                Key = txt + ": " + key;
            }

            StartTime = DateTime.Now;
        }

        public void Dispose()
        {
            TimeSpan timeRun = (DateTime.Now - StartTime);

            if (BenchMarkProvider.BenchMarks.ContainsKey(Key))
            {
                var cb = BenchMarkProvider.BenchMarks[Key];

                cb.NumberOfTimesRun++;

                if (timeRun < cb.ShortestTime) cb.ShortestTime = timeRun;
                if (timeRun > cb.LongestTime) cb.LongestTime = timeRun;

                cb.TotalTime = cb.TotalTime + timeRun;
            }
            else
            {
                try
                {
                    BenchMarkProvider.BenchMarks.Add(Key, new BenchMarkDetails()
                    {
                        Key = this.Key,
                        NumberOfTimesRun = 1,
                        TotalTime = timeRun,
                        ShortestTime = timeRun,
                        LongestTime = timeRun,
                        History = new int[BenchMarkProvider.Steps],
                        FrameHistory = new double[BenchMarkProvider.Steps],
                        FrameHistorySpan = new TimeSpan[BenchMarkProvider.Steps],

                        Color = new Color(new Vector3(Solids.Random.Next(256) / 256f, Solids.Random.Next(256) / 256f, Solids.Random.Next(256) / 256f))
                    });
                }
                catch
                {
                }
            }

            BenchMarkProvider.BenchMarks[Key].FrameTime = BenchMarkProvider.BenchMarks[Key].FrameTime + timeRun;
            BenchMarkProvider.BenchMarks[Key].History[BenchMarkProvider.FramePointer % BenchMarkProvider.Steps] = (int)timeRun.TotalMilliseconds;
        }
    }

    public class FrameCounter
    {
        public FrameCounter()
        {
        }

        public long TotalFrames { get; private set; }
        public float TotalSeconds { get; private set; }
        public float AverageFramesPerSecond { get; private set; }
        public float CurrentFramesPerSecond { get; private set; }

        public const int MAXIMUM_SAMPLES = 100;

        private Queue<float> _sampleBuffer = new Queue<float>();

        public bool Update(float deltaTime)
        {
            CurrentFramesPerSecond = 1.0f / deltaTime;

            _sampleBuffer.Enqueue(CurrentFramesPerSecond);

            if (_sampleBuffer.Count > MAXIMUM_SAMPLES)
            {
                _sampleBuffer.Dequeue();
                AverageFramesPerSecond = _sampleBuffer.Average(i => i);
            }
            else
            {
                AverageFramesPerSecond = CurrentFramesPerSecond;
            }

            TotalFrames++;
            TotalSeconds += deltaTime;
            return true;
        }
    }
}
