namespace Library.Transformation
{
    using System;
    using System.Linq;

    public class BaseTransformation
    {
        protected static double[] Normalize(double[] coords, double length)
        {
            var koef = length/coords.Sum();
            return coords.Select(s => s*koef).ToArray();
        }

        protected static void Transform(double[] orig, double[] output, double[] angle, double length)
        {
            if (orig.Length == 2)
            {
                output[0] = orig[0] + length*Math.Cos(angle[0]);
                output[1] = orig[1] + length*Math.Sin(angle[0]);
            }
            else if (orig.Length == 3)
            {
                output[0] = orig[0] + length*Math.Sin(angle[0])*Math.Cos(angle[1]);
                output[1] = orig[1] + length*Math.Cos(angle[0]);
                output[2] = orig[2] + length*Math.Sin(angle[0])*Math.Sin(angle[1]);
            }
        }

        protected static double[][] CreateMatrix(int height, int width)
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
