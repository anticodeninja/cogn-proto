namespace Library.Geometry
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using Library.Graphics;

    public class Figure<TPoint, TMatrix>
        where TPoint : PointAbstract, new()
        where TMatrix : MatrixAbstract, new()
    {
        
        private readonly List<TPoint> _originalPoints;
        private readonly List<TPoint> _renderPoints;
        private readonly List<Edge> _edges;

        protected TMatrix _transformMatrix;
        protected TMatrix _projectionMatrix;
        protected TMatrix _fullMatrix;

        public float[] DashPattern { get; set; }
        public double Width { get; set; }
        public PointType PointType { get; set; }
        public int Dimension { get; private set; }

        public Figure()
        {
            _originalPoints = new List<TPoint>();
            _renderPoints = new List<TPoint>();
            _edges = new List<Edge>();

            _projectionMatrix = new TMatrix().IdentMatrix<TMatrix>();
            _transformMatrix = new TMatrix().IdentMatrix<TMatrix>();
            _fullMatrix = new TMatrix().IdentMatrix<TMatrix>();

            DashPattern = new[] { 1.0f };
            Width = 1.0;
            PointType = PointType.Circle;
            Dimension = _transformMatrix.Dimension;
        }

        public static double[] Normalize(double[] coords, double length) {
            var koef = length / coords.Sum();
            return coords.Select(s => s * koef).ToArray();
        }

        public void AddEdge(TPoint[] p, Color color)
        {
            var vertex = new int[2];
            for (var i = 0; i < p.Length; ++i)
            {
                vertex[i] = _originalPoints.FindIndex(point => Enumerable
                    .Range(0, Dimension)
                    .All(j => Math.Abs(point[j] - p[i][j]) < Parameters.Tolerance));

                if (vertex[i] != -1) continue;

                _originalPoints.Add(p[i]);
                _renderPoints.Add(new TPoint());

                vertex[i] = _originalPoints.Count - 1;
            }

            _edges.Add(new Edge(vertex, color));
        }

        public void Clear()
        {
            _edges.Clear();
            _originalPoints.Clear();
            _renderPoints.Clear();
        }

        public void SetProjection(TMatrix projection)
        {
            if (projection == null)
            {
                _projectionMatrix.IdentMatrix<TMatrix>();
            }
            else
            {
                MatrixAbstract.Multiply(new TMatrix().IdentMatrix<TMatrix>(), projection, _projectionMatrix);
            }
            _fullMatrix.IdentMatrix<TMatrix>().Chain(_transformMatrix, _projectionMatrix);
        }

        public void SetTransform(TMatrix transform)
        {
            if (transform == null)
            {
                _transformMatrix.IdentMatrix<TMatrix>();
            }
            else
            {
                MatrixAbstract.Multiply(new TMatrix().IdentMatrix<TMatrix>(), transform, _transformMatrix);
            }
            _fullMatrix.IdentMatrix<TMatrix>().Chain(_transformMatrix, _projectionMatrix);
        }

        public void Render(Graphics dc)
        {
            for (var i = 0; i < _originalPoints.Count; ++i)
                _originalPoints[i].Transform(_fullMatrix, _renderPoints[i]);

            var dX = dc.VisibleClipBounds.Width / 2;
            var dY = dc.VisibleClipBounds.Height / 2;

            foreach (var t in _edges) {
                if (t.P[0] != t.P[1]) {
                    dc.DrawLine(new Pen(t.C, (float)Width) { DashPattern = DashPattern },
                                (float) (dX + _renderPoints[t.P[0]][0]), (float) (dY - _renderPoints[t.P[0]][1]),
                                (float) (dX + _renderPoints[t.P[1]][0]), (float) (dY - _renderPoints[t.P[1]][1]));
                } else {
                    PointDrawer.DrawPoint(dc, PointType, t.C, (float)Width,
                        (float)(dX + _renderPoints[t.P[0]][0]), (float)(dY - _renderPoints[t.P[0]][1]));
                }
            }
        }
    }
}
