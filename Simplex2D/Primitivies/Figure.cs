using Library;

namespace Simplex2D.Primitivies
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public class Figure
    {
        private const int VisualizationCount = 4;

        private readonly List<Point>[] _originalPoints;
        private readonly List<Point>[] _renderPoints;

        private float[] _dashPattern;
        private PointType _pointType;

        private readonly List<Edge>[] _edges;

        private readonly Matrix[] _transformMatrix;

        public double Width { get; set; }

        public Figure()
        {
            _originalPoints = new List<Point>[VisualizationCount];
            for (var i = 0; i < VisualizationCount; ++i)
                _originalPoints[i] = new List<Point>();
            _renderPoints = new List<Point>[VisualizationCount];

            _edges = new List<Edge>[VisualizationCount];
            for (var i = 0; i < VisualizationCount; ++i)
                _edges[i] = new List<Edge>();

            _transformMatrix = new Matrix[VisualizationCount];
            for (var i = 0; i < VisualizationCount; ++i) {
                _transformMatrix[i] = new Matrix().IdentMatrix();
            }

            _dashPattern = new[] {1.0f};
        }

        private static double[][] SimplexTransform(int type, double[] h)
        {
            switch (type)
            {
                case 0:
                    return new[] {
                        new []{h[3], h[0], h[2]},
                        new []{h[3], h[2], h[1]},
                        new []{h[0], h[1], h[2]},
                        new []{h[0], h[1], h[3]}
                    };
                case 1:
                    return new[] {
                        new[] {h[0], h[1], h[2]},
                        new[] {0.0, 0.0, 0.0},
                        new[] {h[3], h[1], h[0]},
                        new[] {h[3], h[0], h[2]}
                    };
                case 2:
                    return new[]{
                        new []{CalcMediumKoef(h[0], h[1]), h[2], h[3]},
                        new []{CalcMediumKoef(h[0], h[1]), CalcMediumKoef(h[0], h[3]), CalcMediumKoef(h[0], h[2])},
                        new []{h[1], h[2], CalcMediumKoef(h[0], h[3])},
                        new []{h[1], CalcMediumKoef(h[0], h[2]), h[3]}
                    };
            }
            return null;
        }

        private static double CalcMediumKoef(double c1, double c2) {
            return c1 + c2;
        }

        private static Color CalcMediumColor(Color c1, Color c2) {
            return Color.FromArgb((c1.R + c2.R) / 2, (c1.G + c2.G) / 2, (c1.B + c2.B) / 2);
        }

        private static Color[][] SimplexTransform(int type, Color[] c)
        {
            switch (type)
            {
                case 0:
                    return new[] {
                        new []{c[3], c[0], c[2]},
                        new []{c[3], c[2], c[1]},
                        new []{c[0], c[1], c[2]},
                        new []{c[0], c[1], c[3]}
                    };
                case 1:
                    return new[] {
                        new[] {c[0], c[1], c[2]},
                        new[] {Color.Black, Color.Black, Color.Black},
                        new[] {c[3], c[1], c[0]},
                        new[] {c[3], c[0], c[2]}
                    };
                case 2:
                    return new[]{
                        new []{CalcMediumColor(c[0], c[1]), c[2], c[3]},
                        new []{CalcMediumColor(c[0], c[1]), CalcMediumColor(c[0], c[3]), CalcMediumColor(c[0], c[2])},
                        new []{c[1], c[2], CalcMediumColor(c[0], c[3])},
                        new []{c[1], CalcMediumColor(c[0], c[2]), c[3]}
                    };
            }
            return null;
        }

        private static double[] SimplexToCoord(double[] h) {
            var angle = new []{ Math.PI / 3.0, 0 };

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

        private static double[,] SimplexToMed(double[] h, double[] p) {
            var angle = new[] { -Math.PI / 2, Math.PI * 5 / 6, Math.PI / 6 };
            var coord = new double[h.Length, p.Length];
            for (var i = 0; i < h.Length; ++i)
            {
                coord[i, 0] = p[0] + h[i] * Math.Cos(angle[i]);
                coord[i, 1] = p[1] + h[i] * Math.Sin(angle[i]);
            }
            return coord;
        }

        private static double[][] SimplexToVector(double[] h)
        {
            var angle = new[] { -Math.PI / 3.0, -Math.PI };
            var height = h.Sum();

            var coord = new double[3][];
            for (var i = 0; i < 3; ++i)
                coord[i] = new double[h.Length - 1];

            for (var i = 0; i < 2; ++i)
            {
                height -= h[i];
                var length = height / Math.Cos(Math.PI / 6);

                coord[i+1][0] = coord[i][0] + length * Math.Cos(angle[i]);
                coord[i+1][1] = coord[i][1] + length * Math.Sin(angle[i]);
            }

            for (var i = 0; i < 3; ++i)
                coord[i][1] += h.Sum() * 2.0 / 3.0;

            return coord;
        }

        public static Point Convert(double[] coords) {
            return new Point((float) coords[0], (float) coords[1]);
        }

        public static Point[] Convert(double[][] coords)
        {
            return coords.Select(Convert).ToArray();
        }

        public static Point[] Convert(double[,] coords, int height, int width) {
            return Enumerable
                .Range(0, height)
                .Select(i => Convert(Enumerable.Range(0, width).Select(j => coords[i, j]).ToArray()))
                .ToArray();
        }

        public static double[] Normalize(double[] coords, double length) {
            var koef = length / coords.Sum();
            return coords.Select(s => s * koef).ToArray();
        }

        public static Figure CreateTetrahedron(double length, double width,
            Color color, float[] dashPattern)
        {
            var temp = new Figure {Width = width, _dashPattern = dashPattern};

            var vertex = Convert(new[] {
                SimplexToCoord(Normalize(new[] {1.0, 0, 0}, length)),
                SimplexToCoord(Normalize(new[] {0, 1.0, 0}, length)),
                SimplexToCoord(Normalize(new[] {0, 0, 1.0}, length))
            });

            for (var i = 0; i < VisualizationCount; ++i)
            {
                temp.AddEdge(i, new[] {vertex[0], vertex[1]}, color);
                temp.AddEdge(i, new[] {vertex[1], vertex[2]}, color);
                temp.AddEdge(i, new[] {vertex[2], vertex[0]}, color);
            }
            
            return temp;
        }

        public static Figure CreatePoint(double length, double[] h, double width, Color color,
            PointType pointType, int type)
        {
            var temp = new Figure { Width = width, _pointType = pointType };

            var vertex = Convert(SimplexTransform(type, h).Select(s => SimplexToCoord(Normalize(s, length))).ToArray());

            for (var i = 0; i < VisualizationCount; ++i)
                temp.AddEdge(i, new[] {vertex[i], vertex[i]}, color);

            return temp;
        }

        public static Figure CreateLine(double length, double[] h1, double[] h2, double width, Color color,
            float[] dashPattern, int type)
        {
            var temp = new Figure { Width = width, _dashPattern = dashPattern };

            var vertex1 = Convert(SimplexTransform(type, h1).Select(s => SimplexToCoord(Normalize(s, length))).ToArray());
            var vertex2 = Convert(SimplexTransform(type, h2).Select(s => SimplexToCoord(Normalize(s, length))).ToArray());

            for (var i = 0; i < VisualizationCount; ++i)
                temp.AddEdge(i, new[] {vertex1[i], vertex2[i]}, color);

            return temp;
        }

        public static Figure CreateIjk(double length, double[] origH, double width, Color color, Color[] colors, 
            float[] dashPattern, int type)
        {
            var temp = new Figure { Width = width, _dashPattern = dashPattern};

            var h = SimplexTransform(type, origH).Select(s => Normalize(s, length)).ToArray();
            var c = SimplexTransform(type, colors);

            for (var i = 0; i < VisualizationCount; ++i) {
                var vertex = SimplexToCoord(h[i]);
                var vertex2 = SimplexToMed(h[i], vertex);

                var cVertex = Convert(vertex);
                var cVertex2 = Convert(vertex2, h[i].Length, vertex.Length);

                temp.AddEdge(i, new[] { cVertex, cVertex2[0] }, c[i][0]);
                temp.AddEdge(i, new[] { cVertex, cVertex2[1] }, c[i][1]);
                temp.AddEdge(i, new[] { cVertex, cVertex2[2] }, c[i][2]);
            }
            
            return temp;
        }

        public static Figure CreateVector(double length, double[] origH, double width, Color color,
            float[] dashPattern, int type)
        {
            var temp = new Figure { Width = width, _dashPattern = dashPattern };

            var h = SimplexTransform(type, origH).Select(s => Normalize(s, length)).ToArray();

            for (var i = 0; i < VisualizationCount; ++i)
            {
                var vertex = SimplexToVector(Normalize(h[i], length));
                var cVertex = Convert(vertex);

                temp.AddEdge(i, new[] { cVertex[0], cVertex[1] }, Color.Black);
                temp.AddEdge(i, new[] { cVertex[1], cVertex[2] }, Color.Black);
            }

            return temp;
        }

        public void AddEdge(int id, Point[] p, Color color)
        {
            var vertex = new int[2];
            for (var i = 0; i < p.Length; ++i)
            {
                vertex[i] = _originalPoints[id].FindIndex(point =>
                    Math.Abs(point.X - p[i].X) < Epsilon &&
                    Math.Abs(point.Y - p[i].Y) < Epsilon);

                if (vertex[i] == -1)
                {
                    _originalPoints[id].Add(p[i]);
                    vertex[i] = _originalPoints[id].Count - 1;
                }
            }

            _edges[id].Add(new Edge(vertex, color));
        }

        protected double Epsilon {
            get { return 0.00000001; }
        }

        public void Clear()
        {
            for (var i = 0; i < VisualizationCount; i++)
            {
                _edges[i].Clear();
                _originalPoints[i].Clear();
                _renderPoints[i] = null;
            }
        }

        public void SetTransform(int id, Matrix newTransformMatrix)
        {
            _transformMatrix[id] = newTransformMatrix;
            if (newTransformMatrix == null)
                _transformMatrix[id] = new Matrix().IdentMatrix();
        }

        public void Render(Graphics dc)
        {
            for (var i = 0; i < VisualizationCount; ++i) {
                ResizePointList(_originalPoints[i], ref _renderPoints[i]);
                Transform(_transformMatrix[i], _originalPoints[i], _renderPoints[i]);

                var dX = dc.VisibleClipBounds.Width/2;
                var dY = dc.VisibleClipBounds.Height/2;

                foreach (var t in _edges[i]) {
                    if (t.P[0] != t.P[1]) {
                        dc.DrawLine(new Pen(t.C, (float) Width) {DashPattern = _dashPattern},
                            (float)(dX + _renderPoints[i][t.P[0]].X), (float)(dY - _renderPoints[i][t.P[0]].Y),
                            (float)(dX + _renderPoints[i][t.P[1]].X), (float)(dY - _renderPoints[i][t.P[1]].Y));
                    }
                    else {
                        PointDrawer.DrawPoint(dc, _pointType, t.C, (float)Width,
                            (float)(dX + _renderPoints[i][t.P[0]].X), (float)(dY - _renderPoints[i][t.P[0]].Y));
                    }
                }
            }
        }

        private void Transform(Matrix matrix, List<Point> input, List<Point> output)
        {
            for (int i = 0; i < input.Count; ++i)
                input[i].Transform(matrix, output[i]);
        }

        private void ResizePointList(List<Point> input, ref List<Point> output)
        {
            if (output != null && input.Count == output.Count)
                return;

            output = new List<Point>(input.Count);
            for (var i = 0; i < input.Count; ++i)
                output.Add(new Point());
        }
    }
}
