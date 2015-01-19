namespace Simplex3D.Primitives
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public enum PointType
    {
        TriangleUp,
        TriangleDown,
        Circle,
        Square,
    }

    public class Figure
    {
        private readonly List<Point> _originalPoints;
        private List<Point> _transformPoints;
        private List<Point> _renderPoints;

        private float[] _dashPattern;
        private PointType _pointType;

        private readonly List<Edge> _edges;

        private static readonly Matrix ProjectionMatrix;

        private Matrix _transformMatrix;

        public double Width { get; set; }

        static Figure() {
            ProjectionMatrix = new Matrix();
            Matrix.Multiply(new Matrix().MovementMatrix(0, 0, 800), new Matrix().ProjectionMatrix(), ProjectionMatrix);
        }

        public Figure()
        {
            _originalPoints = new List<Point>();
            _edges = new List<Edge>();

            _transformMatrix = new Matrix().IdentMatrix();
            _dashPattern = new[] { 1.0f };
        }

        private static double[] SimplexToCoord(double[] h) {
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
                var length = height/Math.Sqrt(2.0/3.0);

                var newCoord = new double[h.Length - 1];
                newCoord[0] = coord[0] + length * Math.Sin(angle[i, 0]) * Math.Cos(angle[i, 1]);
                newCoord[1] = coord[1] + length * Math.Sin(angle[i, 0]) * Math.Sin(angle[i, 1]);
                newCoord[2] = coord[2] + length * Math.Cos(angle[i, 0]);
                coord = newCoord;
            }

            coord[2] += h.Sum() * 2.0 / 3.0;

            return coord;
        }

        private static double[][] SimplexToVector(double[] h)
        {
            var angle = new[,] {
                {Math.PI - Math.Acos(Math.Sqrt(2.0/3.0)), 0.0},
                {Math.PI / 2, Math.PI * 5 / 6},
                {Math.PI / 2, -Math.PI / 2}
            };
            var height = h.Sum();

            var coord = new double[4][];
            for (var i = 0; i < 4;  ++i)
                coord[i] = new double[h.Length - 1];
                
            for (var i = 0; i < 3; ++i)
            {
                height -= h[i];
                var length = height / Math.Sqrt(2.0 / 3.0);

                coord[i+1][0] = coord[i][0] + length * Math.Sin(angle[i, 0]) * Math.Cos(angle[i, 1]);
                coord[i+1][1] = coord[i][1] + length * Math.Sin(angle[i, 0]) * Math.Sin(angle[i, 1]);
                coord[i+1][2] = coord[i][2] + length * Math.Cos(angle[i, 0]);
            }

            for (var i = 0; i < 4; ++i)
                coord[i][2] += h.Sum() * 2.0 / 3.0;

            return coord;
        }

        private static double[,] SimplexToMed(double[] h, double[] p) {
            var angle = new[,] {
                {Math.PI, 0.0},
                {Math.Atan(Math.Sqrt(2)), Math.PI},
                {Math.Atan(Math.Sqrt(2)), -Math.PI / 3},
                {Math.Atan(Math.Sqrt(2)), Math.PI / 3}
            };

            var coord = new double[h.Length, p.Length];
            for (var i = 0; i < h.Length; ++i)
            {
                coord[i, 0] = p[0] + h[i] * Math.Sin(angle[i, 0]) * Math.Cos(angle[i, 1]);
                coord[i, 1] = p[1] + h[i] * Math.Sin(angle[i, 0]) * Math.Sin(angle[i, 1]);
                coord[i, 2] = p[2] + h[i] * Math.Cos(angle[i, 0]);
            }

            return coord;
        }

        public static Point Convert(double[] coords) {
            return new Point((float) coords[0], (float) coords[2], (float) coords[1]);
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

        public static Figure CreateTetrahedron(double length, double width, Color color,
            float[] dashPattern)
        {
            var temp = new Figure {Width = width, _dashPattern = dashPattern};

            var vertex = Convert(new[] {
                SimplexToCoord(Normalize(new[] {1.0, 0, 0, 0}, length)),
                SimplexToCoord(Normalize(new[] {0, 1.0, 0, 0}, length)),
                SimplexToCoord(Normalize(new[] {0, 0, 1.0, 0}, length)),
                SimplexToCoord(Normalize(new[] {0, 0, 0, 1.0}, length))
            });

            temp.AddEdge(new[] { vertex[0], vertex[1] }, color);
            temp.AddEdge(new[] { vertex[1], vertex[2] }, color);
            temp.AddEdge(new[] { vertex[2], vertex[0] }, color);
            temp.AddEdge(new[] { vertex[0], vertex[3] }, color);
            temp.AddEdge(new[] { vertex[1], vertex[3] }, color);
            temp.AddEdge(new[] { vertex[2], vertex[3] }, color);

            return temp;
        }

        public static Figure CreatePoint(double length, double[] h, double width, Color color, PointType pointType)
        {
            var temp = new Figure { Width = width, _pointType = pointType};

            var vertex = Convert(SimplexToCoord(Normalize(h, length)));
            temp.AddEdge(new[] { vertex, vertex }, color);

            return temp;
        }

        public static Figure CreateLine(double length, double[] h1, double[] h2, double width, Color color, float[] dashPattern)
        {
            var temp = new Figure { Width = width, _dashPattern = dashPattern };

            var vertex = Convert(new []{
                SimplexToCoord(Normalize(h1, length)),
                SimplexToCoord(Normalize(h2, length))
            });

            temp.AddEdge(new[] { vertex[0], vertex[1] }, color);

            return temp;
        }

        public static Figure CreateIjk(double length, double[] origH, double width, Color color, Color[] colors, 
            float[] dashPattern)
        {
            var temp = new Figure { Width = width, _dashPattern = dashPattern};

            var h = Normalize(origH, length);
            var vertex = SimplexToCoord(h);
            var vertex2 = SimplexToMed(h, vertex);

            var cVertex = Convert(vertex);
            var cVertex2 = Convert(vertex2, h.Length, vertex.Length);

            temp.AddEdge(new[] { cVertex, cVertex }, color);
            temp.AddEdge(new[] { cVertex, cVertex2[0] }, colors[0]);
            temp.AddEdge(new[] { cVertex, cVertex2[1] }, colors[1]);
            temp.AddEdge(new[] { cVertex, cVertex2[2] }, colors[2]);
            temp.AddEdge(new[] { cVertex, cVertex2[3] }, colors[3]);
            return temp;
        }

        public static Figure CreateVector(double length, double[] origH, double width, Color color,
            float[] dashPattern)
        {
            var temp = new Figure { Width = width, _dashPattern = dashPattern };

            var vertex = SimplexToVector(Normalize(origH, length));

            var cVertex = Convert(vertex);

            temp.AddEdge(new[] { cVertex[0], cVertex[1] }, color);
            temp.AddEdge(new[] { cVertex[1], cVertex[2] }, color);
            temp.AddEdge(new[] { cVertex[2], cVertex[3] }, color);

            return temp;
        }

        public void AddEdge(Point[] p, Color color)
        {
            var vertex = new int[2];
            for (var i = 0; i < p.Length; ++i)
            {
                vertex[i] = _originalPoints.FindIndex(point =>
                    Math.Abs(point.X - p[i].X) < Epsilon &&
                    Math.Abs(point.Y - p[i].Y) < Epsilon &&
                    Math.Abs(point.Z - p[i].Z) < Epsilon);

                if (vertex[i] == -1)
                {
                    _originalPoints.Add(p[i]);
                    vertex[i] = _originalPoints.Count - 1;
                }
            }

            _edges.Add(new Edge(vertex, color));
        }

        protected double Epsilon {
            get { return 0.00000001; }
        }

        public void Clear()
        {
            _edges.Clear();
            _originalPoints.Clear();
            _transformPoints = null;
            _renderPoints = null;
        }

        public void SetTransform(Matrix newTransformMatrix)
        {
            _transformMatrix = newTransformMatrix;
            if (newTransformMatrix == null)
                _transformMatrix = new Matrix().IdentMatrix();
        }

        public void Render(Graphics dc)
        {
            ResizePointList(_originalPoints, ref _transformPoints);
            Transform(_transformMatrix, _originalPoints, _transformPoints);

            ResizePointList(_transformPoints, ref _renderPoints);
            Transform(ProjectionMatrix, _transformPoints, _renderPoints);

            var dX = dc.VisibleClipBounds.Width / 2;
            var dY = dc.VisibleClipBounds.Height / 2;

            foreach (var t in _edges) {
                if (t.P[0] != t.P[1]) {
                    dc.DrawLine(new Pen(t.C, (float)Width) { DashPattern = _dashPattern },
                                (float) (dX + _renderPoints[t.P[0]].X), (float) (dY - _renderPoints[t.P[0]].Y),
                                (float) (dX + _renderPoints[t.P[1]].X), (float) (dY - _renderPoints[t.P[1]].Y));
                } else {
                    var ox = (float)(dX + _renderPoints[t.P[0]].X);
                    var oy = (float)(dY - _renderPoints[t.P[0]].Y);
                    var ow = (float) (Width/2);

                    switch (_pointType)
                    {
                        case PointType.TriangleUp:
                            var triangleUp = new[]
                            {
                                new PointF(ox - ow, oy + ow),
                                new PointF(ox + ow, oy + ow),
                                new PointF(ox, oy - ow)
                            };
                            dc.FillPolygon(new SolidBrush(t.C), triangleUp);
                            dc.DrawPolygon(new Pen(Color.Black, 1), triangleUp);
                            break;

                        case PointType.TriangleDown:
                            var triangleDown = new[]
                            {
                                new PointF(ox - ow, oy - ow),
                                new PointF(ox + ow, oy - ow),
                                new PointF(ox, oy + ow)
                            };
                            dc.FillPolygon(new SolidBrush(t.C), triangleDown);
                            dc.DrawPolygon(new Pen(Color.Black, 1), triangleDown);
                            break;

                        case PointType.Circle:
                            dc.FillEllipse(new SolidBrush(t.C), ox - ow, oy - ow, 2 * ow, 2 * ow);
                            dc.DrawEllipse(new Pen(Color.Black, 1), ox - ow, oy - ow, 2 * ow, 2 * ow);
                            break;
                        case PointType.Square:
                            dc.FillRectangle(new SolidBrush(t.C), ox - ow, oy - ow, 2 * ow, 2 * ow);
                            dc.DrawRectangle(new Pen(Color.Black, 1), ox - ow, oy - ow, 2 * ow, 2 * ow);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
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
