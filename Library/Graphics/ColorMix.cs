using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.Graphics
{
    using System.Drawing;

    public class ColorMix
    {
        public static Color Mix(params Color[] c)
        {
            var cmyka = new double[5];

            foreach (var temp in c.Select(GetCmyka))
            {
                for (var j = 0; j < cmyka.Length; ++j)
                {
                    cmyka[j] += temp[j];
                }
            }

            for (var j = 0; j < cmyka.Length; ++j)
            {
                cmyka[j] /= c.Length;
            }

            return GetColor(cmyka);
        }

        private static double[] GetCmyka(Color c)
        {
            double cyan = 255 - c.R;
            double magenta = 255 - c.G;
            double yellow = 255 - c.B;
            var black = Math.Min(Math.Min(cyan, magenta), yellow);
            cyan    = ((cyan - black) / (255 - black));
            magenta = ((magenta - black) / (255 - black));
            yellow  = ((yellow  - black) / (255 - black));

            return new[] {cyan, magenta, yellow, black/255.0, c.A/255.0};
        }

        private static Color GetColor(double[] k)
        {
            var r = k[0] * (1.0 - k[3]) + k[3];
            var g = k[1] * (1.0 - k[3]) + k[3];
            var b = k[2] * (1.0 - k[3]) + k[3];
            r = Math.Round((1.0 - r) * 255.0 + 0.5);
            g = Math.Round((1.0 - g) * 255.0 + 0.5);
            b = Math.Round((1.0 - b) * 255.0 + 0.5);
            return Color.FromArgb((int) (255*k[4]), (int) r, (int) g, (int) b);
        }
    }
}
