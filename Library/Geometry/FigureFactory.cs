namespace Library.Geometry
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using Library.Graphics;

    public class FigureFactory<TPoint, TMatrix>
        where TPoint : PointAbstract, new()
        where TMatrix : MatrixAbstract, new()
    {
        public static Figure<TPoint, TMatrix> CreateTetrahedron(
            double length, double width, Color color, float[] dashPattern)
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
                vertexes.Add(new TPoint().Set<TPoint>(Simplex.SimplexToCoord(Normalize(
                    Enumerable.Range(0, dimension+1).Select(a=>a==i ? 1.0 : 0.0).ToArray(), length))));
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

        public static Figure<TPoint, TMatrix> CreateIjk(
            double length, double[] origH, double width, Color color, Color[] colors, float[] dashPattern)
        {
            var temp = new Figure<TPoint, TMatrix>
            {
                Width = width,
                DashPattern = dashPattern
            };

            var h = Normalize(origH, length);
            var vertex = Simplex.SimplexToCoord(h);
            var vertex2 = Simplex.SimplexToMed(h, vertex);

            var cVertex = new TPoint().Set<TPoint>(vertex);
            for (var i = 0; i < origH.Length; ++i)
            {
                temp.AddEdge(new[] { cVertex, new TPoint().Set<TPoint>(vertex2[i])}, colors[i]);
            }

            temp.AddEdge(new[] { cVertex, cVertex }, color);
            return temp;
        }

        public static Figure<TPoint, TMatrix> CreateVector(
            double length, double[] origH, double width, Color color, float[] dashPattern)
        {
            var temp = new Figure<TPoint, TMatrix>
            {
                Width = width,
                DashPattern = dashPattern
            };

            var vertex = Simplex.SimplexToVector(Normalize(origH, length));
            var cVertex1 = new TPoint().Set<TPoint>(vertex[0]);
            for (var i = 1; i < origH.Length; ++i)
            {
                var cVertex2 = new TPoint().Set<TPoint>(vertex[i]);
                temp.AddEdge(new[] { cVertex1, cVertex2 }, color);
                cVertex1 = cVertex2;
            }

            return temp;
        }

        public static Figure<TPoint, TMatrix> CreateSimplexPoint(
            double length, double[] h, double width, Color color, PointType pointType)
        {
            return CreatePoint(Simplex.SimplexToCoord(Normalize(h, length)), width, color, pointType);
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

        public static Figure<TPoint, TMatrix> CreateLine(
            double length, double[] h1, double[] h2, double width, Color color, float[] dashPattern)
        {
            var temp = new Figure<TPoint, TMatrix>
            {
                Width = width,
                DashPattern = dashPattern
            };

            temp.AddEdge(new[]
            {
                new TPoint().Set<TPoint>(Simplex.SimplexToCoord(Normalize(h1, length))),
                new TPoint().Set<TPoint>(Simplex.SimplexToCoord(Normalize(h2, length)))
            }, color);

            return temp;
        }

        protected static double[] Normalize(double[] coords, double length)
        {
            var koef = length / coords.Sum();
            return coords.Select(s => s * koef).ToArray();
        }
    }
}
