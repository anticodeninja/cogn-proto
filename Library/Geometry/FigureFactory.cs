namespace Library.Geometry
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using Library.Graphics;
    using Library.Transformation;

    public class FigureFactory<TPoint, TMatrix>
        where TPoint : PointAbstract, new()
        where TMatrix : MatrixAbstract, new()
    {
        public static Figure<TPoint, TMatrix> CreateSimplexBody(
            double size, double width, Color color, float[] dashPattern)
        {
            var temp = new Figure<TPoint, TMatrix>
            {
                Width = width,
                DashPattern = dashPattern
            };

            var dimension = new TPoint().Dimension;
            var vertexes = new List<TPoint>();

            for (var i = 0; i <= dimension; ++i)
            {
                vertexes.Add(new TPoint().Set<TPoint>(Simplex.ToPoint(size,
                    Enumerable.Range(0, dimension+1).Select(a=>a==i ? 1.0 : 0.0).ToArray())));
            }

            for (var i = 0; i < dimension; ++i)
            {
                for (var j = i + 1; j <= dimension; ++j)
                {
                    temp.AddEdge(new[] {vertexes[i], vertexes[j]}, color);
                }
            }

            return temp;
        }

        public static Figure<TPoint, TMatrix> CreatePrismBody(
            double size, double length, double width, Color color, float[] dashPattern)
        {
            var temp = new Figure<TPoint, TMatrix>
            {
                Width = width,
                DashPattern = dashPattern
            };

            var dimension = new TPoint().Dimension;
            var vertexes = new List<TPoint>();
            var bound = new double[dimension + 1];

            for (var i = 0; i < dimension; ++i)
            {
                bound[i + 1] = 1.0;
                bound[0] = 0.0;
                vertexes.Add(new TPoint().Set<TPoint>(Prism.ToPoint(size, length, bound)));
                bound[0] = 1.0;
                vertexes.Add(new TPoint().Set<TPoint>(Prism.ToPoint(size, length, bound)));
                bound[i + 1] = 0.0;
            }

            for (var i = 0; i < dimension; ++i)
            {
                temp.AddEdge(new[] {vertexes[2*i], vertexes[2*i + 1]}, color);
            }

            for (var i = 0; i < dimension - 1; ++i)
            {
                for (var j = i + 1; j < dimension; ++j)
                {
                    temp.AddEdge(new[] {vertexes[2*i], vertexes[2*j]}, color);
                    temp.AddEdge(new[] {vertexes[2*i + 1], vertexes[2*j + 1]}, color);
                }
            }

            return temp;
        }

        public static Figure<TPoint, TMatrix> CreateIjk(
            double[][] c, double width, Color color, Color[] colors, float[] dashPattern)
        {
            var temp = new Figure<TPoint, TMatrix>
            {
                Width = width,
                DashPattern = dashPattern
            };

            var pVertex0 = new TPoint().Set<TPoint>(c[0]);
            for (var i = 0; i < c.Length-1; ++i)
            {
                temp.AddEdge(new[] {pVertex0, new TPoint().Set<TPoint>(c[i + 1])}, colors[i]);
            }

            temp.AddEdge(new[] { pVertex0, pVertex0 }, color);
            return temp;
        }

        public static Figure<TPoint, TMatrix> CreateVector(
            double[][] c, double width, Color color, float[] dashPattern)
        {
            var temp = new Figure<TPoint, TMatrix>
            {
                Width = width,
                DashPattern = dashPattern
            };

            var cVertex1 = new TPoint().Set<TPoint>(c[0]);
            for (var i = 1; i < c.Length; ++i)
            {
                var cVertex2 = new TPoint().Set<TPoint>(c[i]);
                temp.AddEdge(new[] { cVertex1, cVertex2 }, color);
                cVertex1 = cVertex2;
            }

            return temp;
        }

        public static Figure<TPoint, TMatrix> CreatePoint(
            double[] c, double width, Color color, PointType pointType)
        {
            var temp = new Figure<TPoint, TMatrix>
            {
                Width = width,
                PointType = pointType
            };

            var vertex = new TPoint().Set<TPoint>(c);
            temp.AddEdge(new[] {vertex, vertex}, color);

            return temp;
        }
    }
}
