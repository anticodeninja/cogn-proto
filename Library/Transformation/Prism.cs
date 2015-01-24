namespace Library.Transformation
{
    using System;
    using System.Linq;

    internal class Prism : BaseTransformation
    {
        public static double[] ToCoord(double size, double length, double[] k)
        {
            var h = new double[k.Length - 1];
            Array.Copy(k, 1, h, 0, h.Length);
            h = Simplex.ToCoord(size, h);
            Expand(ref h, k[0], length);
            return h;
        }

        public static double[][] ToVector(double size, double length, double[] k)
        {
            var h = new double[k.Length - 1];
            Array.Copy(k, 1, h, 0, h.Length);
            var hm = Simplex.ToVector(size, h);
            Array.Resize(ref hm, hm.Length + 1);
            for (var i = hm.Length -1 ; i > 0; --i)
            {
                hm[i] = hm[i - 1];
                Expand(ref hm[i], k[0], length);
            }
            hm[0] = new double[hm[1].Length-1];
            Array.Copy(hm[1], 0, hm[0], 0, hm[0].Length);
            Expand(ref hm[0], 0, length);
            return hm;
        }

        public static double[][] ToMedian(double size, double length, double[] k, double[] hp)
        {
            var h = new double[k.Length - 1];
            Array.Copy(k, 1, h, 0, h.Length);
            var oh = Simplex.ToCoord(size, h);
            var hm = Simplex.ToMedian(size, h, oh);
            for (var i = 0; i < hm.Length; ++i)
            {
                Expand(ref hm[i], k[0], length);
            }
            Array.Resize(ref hm, hm.Length + 1);
            hm[hm.Length - 1] = new double[oh.Length];
            Array.Copy(oh, 0, hm[hm.Length - 1], 0, oh.Length);
            Expand(ref hm[hm.Length - 1], 0, length);
            return hm;
        }

        private static void Expand(ref double[] p, double value, double length)
        {
            Array.Resize(ref p, p.Length + 1);
            p[p.Length - 1] = -length*(value - 0.5);
        }
    }
}
;