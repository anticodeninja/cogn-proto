namespace Simplex2D
{
    using System.Drawing;

    class ReduceTransform
    {
        public int Type { get; set; }

        public double[][] SimplexTransform(double[] h)
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
                    return new[]{
                        new []{CalcMediumKoef(h[0], h[1]), h[2], h[3]},
                        new []{CalcMediumKoef(h[0], h[1]), CalcMediumKoef(h[0], h[3]), CalcMediumKoef(h[0], h[2])},
                        new []{h[1], h[2], CalcMediumKoef(h[0], h[3])},
                        new []{h[1], CalcMediumKoef(h[0], h[2]), h[3]}
                    };
            }
            return null;
        }

        public double CalcMediumKoef(double c1, double c2) {
            return c1 + c2;
        }

        public Color CalcMediumColor(Color c1, Color c2) {
            return Color.FromArgb((c1.R + c2.R) / 2, (c1.G + c2.G) / 2, (c1.B + c2.B) / 2);
        }

        public Color[][] SimplexTransform(int type, Color[] c)
        {
            switch (type)
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
                    return new[]{
                        new []{CalcMediumColor(c[0], c[1]), c[2], c[3]},
                        new []{CalcMediumColor(c[0], c[1]), CalcMediumColor(c[0], c[3]), CalcMediumColor(c[0], c[2])},
                        new []{c[1], c[2], CalcMediumColor(c[0], c[3])},
                        new []{c[1], CalcMediumColor(c[0], c[2]), c[3]}
                    };
            }
            return null;
        }
    }
}
