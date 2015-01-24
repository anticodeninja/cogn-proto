namespace Library.Transformation
{
    using System;

    public class Prism : BaseTransformation
    {
        public static double[] ToPoint(double size, double length, double[] k)
        {
            var h = Reduce(k);
            h = Simplex.ToPoint(size, h);
            return Expand(h, k[0], length);
        }

        public static double[][] ToVector(double size, double length, double[] k)
        {
            var h = Reduce(k);
            var hm = Simplex.ToVector(size, h);
            Array.Resize(ref hm, hm.Length + 1);
            for (var i = hm.Length - 1; i > 0; --i)
            {
                hm[i] = Expand(hm[i - 1], k[0], length);
            }
            hm[0] = Expand(hm[0], 0, length);
            return hm;
        }

        public static double[][] ToMedian(double size, double length, double[] k)
        {
            var h = Reduce(k);
            var hm = Simplex.ToMedian(size, h);
            Array.Resize(ref hm, hm.Length + 1);
            for (var i = hm.Length - 1; i >= 0; --i)
            {
                hm[i] = Expand(hm[i != 0 ? i - 1 : i], i != 1 ? k[0] : 0, length);
            }
            return hm;
        }

        private static double[] Reduce(double[] h)
        {
            var temp = new double[h.Length - 1];
            Array.Copy(h, 1, temp, 0, temp.Length);
            return temp;
        }

        private static double[] Expand(double[] p, double value, double length)
        {
            var temp = new double[p.Length + 1];
            Array.Copy(p, 0, temp, 0, p.Length);
            temp[temp.Length - 1] = -length*(value - 0.5);
            return temp;
        }
    }
}
;