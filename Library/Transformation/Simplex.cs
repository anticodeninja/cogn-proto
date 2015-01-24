namespace Library.Transformation
{
    using System;
    using System.Linq;

    public class Simplex : BaseTransformation
    {
        public static double[] ToPoint(double size, double[] h)
        {
            var v = ToVector(size, h);
            return v[v.Length - 1];
        }

        public static double[][] ToVector(double size, double[] h)
        {
            h = Normalize(h, size);

            double[][] angle;

            switch (h.Length)
            {
                case 3:
                    angle = new[]
                    {
                        new[] {-Math.PI/3.0},
                        new[] {Math.PI}
                    };
                    break;
                case 4:
                    angle = new[]
                    {
                        new[] {Math.PI - Math.Acos(Math.Sqrt(2.0/3.0)), 0.0},
                        new[] {Math.PI/2, Math.PI*5/6},
                        new[] {Math.PI/2, -Math.PI/2}
                    };
                    break;
                default:
                    throw new ArgumentException("Incorrect dimensions");
            }

            var height = h.Sum();

            var coord = CreateMatrix(h.Length, h.Length - 1);
            for (var i = 0; i < h.Length - 1; ++i)
            {
                height -= h[i];
                var length = height/Math.Cos(Math.PI/6);

                Transform(coord[i], coord[i + 1], angle[i], length);
            }

            for (var i = 0; i < h.Length; ++i)
                coord[i][1] += h.Sum()*2.0/3.0;

            return coord;
        }

        public static double[][] ToMedian(double size, double[] h)
        {
            h = Normalize(h, size);

            double[][] angle;

            switch (h.Length)
            {
                case 3:
                    angle = new[]
                    {
                        new[] {-Math.PI/2},
                        new[] {Math.PI*5/6},
                        new[] {Math.PI/6}
                    };
                    break;
                case 4:
                    angle = new[]
                    {
                        new[] {Math.PI, 0.0},
                        new[] {Math.Atan(Math.Sqrt(2)), Math.PI},
                        new[] {Math.Atan(Math.Sqrt(2)), -Math.PI/3},
                        new[] {Math.Atan(Math.Sqrt(2)), Math.PI/3}
                    };
                    break;
                default:
                    throw new ArgumentException("Incorrect dimensions");
            }

            var coord = CreateMatrix(h.Length + 1, h.Length - 1);
            coord[0] = ToVector(size, h).Last();
            for (var i = 0; i < h.Length; ++i)
            {
                Transform(coord[0], coord[i + 1], angle[i], h[i]);
            }

            return coord;
        }
    }
}
;