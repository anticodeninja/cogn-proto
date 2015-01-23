namespace Simplex2D
{
    using System;
    using System.Drawing;
    using Library.Graphics;

    class ReduceTransform
    {
        public int Type { get; set; }

        public double[][] Transform(double[] h)
        {
            switch (Type)
            {
                case 0:
                    return new[] {
                        new []{h[3], h[0], h[2]},
                        new []{h[3], h[2], h[1]},
                        new []{h[0], h[1], h[2]},
                        new []{h[0], h[1], h[3]}
                    };
                case 1:
                    return new[] {
                        new[] {h[0], h[1], h[2]},
                        new[] {0.0, 0.0, 0.0},
                        new[] {h[3], h[1], h[0]},
                        new[] {h[3], h[0], h[2]}
                    };
                case 2:
                    Func<double, double, double> combine = (d1, d2) => d1 + d2;
                    return new[]{
                        new []{combine(h[0], h[1]), h[2], h[3]},
                        new []{combine(h[0], h[1]), combine(h[0], h[3]), combine(h[0], h[2])},
                        new []{h[1], h[2], combine(h[0], h[3])},
                        new []{h[1], combine(h[0], h[2]), h[3]}
                    };
            }
            return null;
        }

        public Color[][] Transform(Color[] c)
        {
            switch (Type)
            {
                case 0:
                    return new[] {
                        new []{c[3], c[0], c[2]},
                        new []{c[3], c[2], c[1]},
                        new []{c[0], c[1], c[2]},
                        new []{c[0], c[1], c[3]}
                    };
                case 1:
                    return new[] {
                        new[] {c[0], c[1], c[2]},
                        new[] {Color.Black, Color.Black, Color.Black},
                        new[] {c[3], c[1], c[0]},
                        new[] {c[3], c[0], c[2]}
                    };
                case 2:
                    //Func<Color, Color, Color> combine = (c1, c2) => 
                    //    Color.FromArgb((c1.R + c2.R) / 2, (c1.G + c2.G) / 2, (c1.B + c2.B) / 2);
                    Func<Color, Color, Color> combine = (c1, c2) => ColorMix.Mix(c1, c2);
                    return new[]{
                        new []{combine(c[0], c[1]), c[2], c[3]},
                        new []{combine(c[0], c[1]), combine(c[0], c[3]), combine(c[0], c[2])},
                        new []{c[1], c[2], combine(c[0], c[3])},
                        new []{c[1], combine(c[0], c[2]), c[3]}
                    };
            }
            return null;
        }
    }
}
