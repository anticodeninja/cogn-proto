namespace Library
{
    using System;
    using System.Linq;

    class Simplex
    {
        public static double[] SimplexToCoord(double[] h)
        {
            switch (h.Length)
            {
                case 3:
                    return Simplex2ToCoord(h);
                case 4:
                    return Simplex3ToCoord(h);
                default:
                    return null;
            }
        }

        private static double[] Simplex2ToCoord(double[] h)
        {
            var angle = new[] { Math.PI / 3.0, 0 };

            var coord = new double[h.Length - 1];
            for (var i = 0; i < h.Length - 1; ++i)
            {
                var length = h[i];

                var newCoord = new double[h.Length - 1];
                newCoord[0] = coord[0] + length * Math.Cos(angle[i]);
                newCoord[1] = coord[1] + length * Math.Sin(angle[i]);
                coord = newCoord;
            }

            coord[0] /= Math.Sin(Math.PI / 3);
            coord[1] /= Math.Sin(Math.PI / 3);

            coord[0] -= h.Sum() / Math.Sin(Math.PI / 3) / 2;
            coord[1] -= h.Sum() / 3.0;

            return coord;
        }

        private static double[] Simplex3ToCoord(double[] h)
        {
            var angle = new[,] {
                {Math.PI - Math.Acos(Math.Sqrt(2.0/3.0)), 0.0},
                {Math.PI / 2, Math.PI * 5 / 6},
                {Math.PI / 2, -Math.PI / 2}
            };

            var height = h.Sum();
            var coord = new double[h.Length - 1];
            for (var i = 0; i < coord.Length; ++i)
            {
                height -= h[i];
                var length = height / Math.Sqrt(2.0 / 3.0);

                var newCoord = new double[h.Length - 1];
                newCoord[0] = coord[0] + length * Math.Sin(angle[i, 0]) * Math.Cos(angle[i, 1]);
                newCoord[1] = coord[1] + length * Math.Cos(angle[i, 0]);
                newCoord[2] = coord[2] + length * Math.Sin(angle[i, 0]) * Math.Sin(angle[i, 1]);
                coord = newCoord;
            }

            coord[1] += h.Sum() * 2.0 / 3.0;

            return coord;
        }

        public static double[][] SimplexToVector(double[] h)
        {
            switch (h.Length)
            {
                case 3:
                    return Simplex2ToVector(h);
                case 4:
                    return Simplex3ToVector(h);
                default:
                    return null;
            }
        }

        private static double[][] Simplex3ToVector(double[] h)
        {
            var angle = new[,] {
                {Math.PI - Math.Acos(Math.Sqrt(2.0/3.0)), 0.0},
                {Math.PI / 2, Math.PI * 5 / 6},
                {Math.PI / 2, -Math.PI / 2}
            };
            var height = h.Sum();

            var coord = CreateMatrix(4, h.Length - 1);
            for (var i = 0; i < 3; ++i)
            {
                height -= h[i];
                var length = height / Math.Sqrt(2.0 / 3.0);

                coord[i + 1][0] = coord[i][0] + length * Math.Sin(angle[i, 0]) * Math.Cos(angle[i, 1]);
                coord[i + 1][1] = coord[i][1] + length * Math.Cos(angle[i, 0]);
                coord[i + 1][2] = coord[i][2] + length * Math.Sin(angle[i, 0]) * Math.Sin(angle[i, 1]);
            }

            for (var i = 0; i < 4; ++i)
                coord[i][1] += h.Sum() * 2.0 / 3.0;

            return coord;
        }

        private static double[][] Simplex2ToVector(double[] h)
        {
            var angle = new[] { -Math.PI / 3.0, -Math.PI };
            var height = h.Sum();

            var coord = CreateMatrix(3, h.Length - 1);
            for (var i = 0; i < 2; ++i)
            {
                height -= h[i];
                var length = height / Math.Cos(Math.PI / 6);

                coord[i + 1][0] = coord[i][0] + length * Math.Cos(angle[i]);
                coord[i + 1][1] = coord[i][1] + length * Math.Sin(angle[i]);
            }

            for (var i = 0; i < 3; ++i)
                coord[i][1] += h.Sum() * 2.0 / 3.0;

            return coord;
        }

        public static double[][] SimplexToMed(double[] h, double[] p)
        {
            switch (h.Length)
            {
                case 3:
                    return Simplex2ToMed(h, p);
                case 4:
                    return Simplex3ToMed(h, p);
                default:
                    return null;
            }
        }

        private static double[][] Simplex3ToMed(double[] h, double[] p)
        {
            var angle = new[,] {
                {Math.PI, 0.0},
                {Math.Atan(Math.Sqrt(2)), Math.PI},
                {Math.Atan(Math.Sqrt(2)), -Math.PI / 3},
                {Math.Atan(Math.Sqrt(2)), Math.PI / 3}
            };

            var coord = CreateMatrix(h.Length, p.Length);
            for (var i = 0; i < h.Length; ++i)
            {
                coord[i][0] = p[0] + h[i] * Math.Sin(angle[i, 0]) * Math.Cos(angle[i, 1]);
                coord[i][1] = p[1] + h[i] * Math.Cos(angle[i, 0]);
                coord[i][2] = p[2] + h[i] * Math.Sin(angle[i, 0]) * Math.Sin(angle[i, 1]);
            }

            return coord;
        }

        private static double[][] Simplex2ToMed(double[] h, double[] p)
        {
            var angle = new[] { -Math.PI / 2, Math.PI * 5 / 6, Math.PI / 6 };

            var coord = CreateMatrix(h.Length, p.Length);
            for (var i = 0; i < h.Length; ++i)
            {
                coord[i][0] = p[0] + h[i] * Math.Cos(angle[i]);
                coord[i][1] = p[1] + h[i] * Math.Sin(angle[i]);
            }

            return coord;
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