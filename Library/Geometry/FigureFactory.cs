namespace Library.Geometry
{
    using System;
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
                vertexes.Add(new TPoint().Set<TPoint>(Simplex.ToCoord(size,
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

            for (var i = 0; i < dimension; ++i)
            {
                vertexes.Add(new TPoint().Set<TPoint>(Prism.ToCoord(size, length,
                    Enumerable.Range(0, dimension + 1).Select(a => ((a != 0 && a == (i + 1)) ? 1.0 : 0.0)).ToArray())));
                vertexes.Add(new TPoint().Set<TPoint>(Prism.ToCoord(size, length,
                    Enumerable.Range(0, dimension + 1).Select(a => ((a == 0 || a == (i + 1)) ? 1.0 : 0.0)).ToArray())));
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

        public static Figure<TPoint, TMatrix> CreateSimplexIjk(
            double size, double[] h, double width, Color color, Color[] colors, float[] dashPattern)
        {
            var temp = new Figure<TPoint, TMatrix>
            {
                Width = width,
                DashPattern = dashPattern
            };

            var vertex = Simplex.ToCoord(size, h);
            var vertex2 = Simplex.ToMedian(size, h, vertex);

            var cVertex = new TPoint().Set<TPoint>(vertex);
            for (var i = 0; i < h.Length; ++i)
            {
                temp.AddEdge(new[] { cVertex, new TPoint().Set<TPoint>(vertex2[i])}, colors[i]);
            }

            temp.AddEdge(new[] { cVertex, cVertex }, color);
            return temp;
        }

        public static Figure<TPoint, TMatrix> CreatePrismIjk(
            double size, double length, double[] h, double width, Color color, Color[] colors, float[] dashPattern)
        {
            var temp = new Figure<TPoint, TMatrix>
            {
                Width = width,
                DashPattern = dashPattern
            };

            var vertex = Prism.ToCoord(size, length, h);
            var vertex2 = Prism.ToMedian(size, length, h, vertex);

            var cVertex = new TPoint().Set<TPoint>(vertex);
            for (var i = 0; i < h.Length; ++i)
            {
                temp.AddEdge(new[] { cVertex, new TPoint().Set<TPoint>(vertex2[i])}, colors[i]);
            }

            temp.AddEdge(new[] { cVertex, cVertex }, color);
            return temp;
        }

        public static Figure<TPoint, TMatrix> CreateSimplexVector(
            double size, double[] h, double width, Color color, float[] dashPattern)
        {
            var temp = new Figure<TPoint, TMatrix>
            {
                Width = width,
                DashPattern = dashPattern
            };

            var vertex = Simplex.ToVector(size, h);
            var cVertex1 = new TPoint().Set<TPoint>(vertex[0]);
            for (var i = 1; i < h.Length; ++i)
            {
                var cVertex2 = new TPoint().Set<TPoint>(vertex[i]);
                temp.AddEdge(new[] { cVertex1, cVertex2 }, color);
                cVertex1 = cVertex2;
            }

            return temp;
        }

        public static Figure<TPoint, TMatrix> CreatePrismVector(
            double size, double length, double[] h, double width, Color color, float[] dashPattern)
        {
            var temp = new Figure<TPoint, TMatrix>
            {
                Width = width,
                DashPattern = dashPattern
            };

            var vertex = Prism.ToVector(size, length, h);
            var cVertex1 = new TPoint().Set<TPoint>(vertex[0]);
            for (var i = 1; i < h.Length; ++i)
            {
                var cVertex2 = new TPoint().Set<TPoint>(vertex[i]);
                temp.AddEdge(new[] { cVertex1, cVertex2 }, color);
                cVertex1 = cVertex2;
            }

            return temp;
        }

        public static Figure<TPoint, TMatrix> CreateSimplexPoint(
            double size, double[] h, double width, Color color, PointType pointType)
        {
            return CreatePoint(Simplex.ToCoord(size, h), width, color, pointType);
        }

        public static Figure<TPoint, TMatrix> CreatePrismPoint(
            double size, double length, double[] h, double width, Color color, PointType pointType)
        {
            return CreatePoint(Prism.ToCoord(size, length, h), width, color, pointType);
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
            double size, double[] h1, double[] h2, double width, Color color, float[] dashPattern)
        {
            var temp = new Figure<TPoint, TMatrix>
            {
                Width = width,
                DashPattern = dashPattern
            };

            temp.AddEdge(new[]
            {
                new TPoint().Set<TPoint>(Simplex.ToCoord(size, h1)),
                new TPoint().Set<TPoint>(Simplex.ToCoord(size, h2))
            }, color);

            return temp;
        }
    }
}
