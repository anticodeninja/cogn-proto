namespace Library
{
    using System;
    using System.Linq;

    internal class Simplex
    {
        public static double[] SimplexToCoord(double[] h)
        {
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
            var coord = new double[h.Length - 1];
            for (var i = 0; i < h.Length - 1; ++i)
            {
                height -= h[i];
                var length = height/Math.Cos(Math.PI/6);

                var newCoord = new double[h.Length - 1];
                Transform(coord, newCoord, angle[i], length);
                coord = newCoord;
            }

            coord[1] += h.Sum()*2.0/3.0;

            return coord;
        }

        public static double[][] SimplexToVector(double[] h)
        {
            double[][] angle;

            switch (h.Length)
            {
                case 3:
                    angle = new[]
                    {
                        new[] {-Math.PI/3.0},
                        new[] {-Math.PI}
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

        public static double[][] SimplexToMed(double[] h, double[] p)
        {
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

            var coord = CreateMatrix(h.Length, p.Length);
            for (var i = 0; i < h.Length; ++i)
            {
                Transform(p, coord[i], angle[i], h[i]);
            }

            return coord;
        }

        private static void Transform(double[] orig, double[] output, double[] angle, double length)
        {
            if (orig.Length == 2)
            {
                output[0] = orig[0] + length*Math.Cos(angle[0]);
                output[1] = orig[1] + length*Math.Sin(angle[0]);
            }
            else if(orig.Length == 3)
            {
                output[0] = orig[0] + length*Math.Sin(angle[0])*Math.Cos(angle[1]);
                output[1] = orig[1] + length*Math.Cos(angle[0]);
                output[2] = orig[2] + length*Math.Sin(angle[0])*Math.Sin(angle[1]);
            }
        }

        private static double[][] CreateMatrix(int height, int width)
        {
            var temp = new double[height][];
            for (var i = 0; i < height; ++i)
            {
                temp[i] = new double[width];
            }
            return temp;
        }
    }
}
;