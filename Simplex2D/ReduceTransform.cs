namespace Simplex2D
{
    using System;
    using System.Drawing;
    using Library.Graphics;

    class ReduceTransform
    {
        public int Type { get; set; }

        private T[][] TransformInternal<T>(T[] k, T def, Func<T, T, T> combine)
        {
            switch (Type)
            {
                case 0:
                    return new[] {
                        new []{k[3], k[0], k[1]},
                        new []{k[3], k[1], k[2]},
                        new []{k[0], k[2], k[1]},
                        new []{k[3], k[2], k[0]}
                    };
                case 1:
                    return new[] {
                        new[] {k[0], k[2], k[3]},
                        new[] {def, def, def},
                        new[] {k[1], k[2], k[0]},
                        new[] {k[1], k[0], k[3]}
                    };
                case 2:
                    return new[]{
                        new []{k[0], k[1], k[2]},
                        new []{k[3], k[1], k[2]},
                        new []{k[2], k[3], k[0]},
                        new []{k[1], k[0], k[3]},
                    };
                case 3:
                    return new[]{
                        new[] {k[0], k[2], k[3]},
                        new[] {k[1], k[2], k[3]},
                        new[] {k[1], k[2], k[0]},
                        new[] {k[1], k[0], k[3]} 
                    };
            }
            return null;
        }

        public double[][] Transform(double[] h)
        {
            return TransformInternal(h, 0.0, (d1, d2) => d1 + d2);
        }

        public Color[][] Transform(Color[] c)
        {
            return TransformInternal(c, Color.Black, (c1, c2) => ColorMix.Mix(c1, c2));
        }
    }
}
