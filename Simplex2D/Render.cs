namespace Simplex2D
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    using Library;
    using Library.Geometry;
    using Library.Graphics;
    using Library.Transformation;

    public partial class Render : Form
    {
        private readonly List<Figure<Point2D, Matrix2D>[]> _mainFigures;
        private readonly Matrix2D[] _matrices;
        private readonly ReduceTransform _transform;

        public Render(string inputFile)
        {
            InitializeComponent();

            _mainFigures = new List<Figure<Point2D, Matrix2D>[]>();
            _matrices = new Matrix2D[4];
            for (var i = 0; i < _matrices.Length; ++i)
                _matrices[i] = new Matrix2D().IdentMatrix<Matrix2D>();
            _transform = new ReduceTransform();

            var parser = new Parser(inputFile);
            InitializeParser(parser);
            Shown += (sender, args) => parser.Start();

            renderPanel.Paint += (sender, args) => RenderPicture(args.Graphics);
        }

        private void InitializeParser(Parser parser)
        {
            parser.Add("addTetraedron", new Action<string, int, object[], int>(
                (color, width, pattern, size) =>
                AddFigure(()=>FigureFactory2D.CreateSimplexBody(
                    size, width, ColorTranslator.FromHtml(color), ParsePattern(pattern)))));

            parser.Add("addIJK", new Action<string, int, object[], int, object[], object[]>(
                (color, width, pattern, size, point, colors) =>
                    AddFigure((a, c) => FigureFactory2D.CreateIjk(Simplex.ToMedian(size, a),
                        width, ColorTranslator.FromHtml(color),
                        c,
                        ParsePattern(pattern)), ParseValue(point),
                        colors.Select(c => ColorTranslator.FromHtml((string) c)).ToArray())));

            parser.Add("addVector", new Action<string, int, object[], int, object[]>(
                (color, width, pattern, size, point) =>
                    AddFigure(a => FigureFactory2D.CreateVector(Simplex.ToVector(size, a),
                        width, ColorTranslator.FromHtml(color),
                        ParsePattern(pattern)), ParseValue(point))));

            parser.Add("addPath", new Action<string, int, object[], int, object[]>(
                (color, width, pattern, size, path) => 
                    AddFigure(a=>FigureFactory2D.CreateVector(
                        a.Select(c=>Simplex.ToPoint(size, c)).ToArray(),
                        width, ColorTranslator.FromHtml(color),
                        ParsePattern(pattern)), path.Select(ParseValue).ToArray())));

            parser.Add("addPoint", new Action<string, int, string, int, object[]>(
                (color, width, pattern, size, point) =>
                    AddFigure(a=>FigureFactory2D.CreatePoint(Simplex.ToPoint(size, a),
                        width, ColorTranslator.FromHtml(color),
                        (PointType)Enum.Parse(typeof(PointType), pattern)),ParseValue(point))));

            parser.Add("setTransform", new Action<int>(type =>
            {
                _transform.Type = type;
            }));

            parser.Add("setView", new Action<int>(RecalculateMatrix));

            parser.Reloaded += (sender, args) => Invoke(new Action(() =>
            {
                if (args.Message == null)
                {
                    foreach (var figure in _mainFigures)
                    {
                        for (var j = 0; j < _matrices.Length; ++j)
                        {
                            figure[j].SetTransform(_matrices[j]);
                        }
                    }
                    renderPanel.Invalidate();
                }
                else
                {
                    MessageBox.Show(args.Message);
                }
            }));
            parser.Reloading += (sender, args) => ResetData();
        }

        private static float[] ParsePattern(IEnumerable<object> array)
        {
            return array.Select(a => (float)(double)a).ToArray();
        }

        private double[] ParseValue(object array)
        {
            return ((object[])array).Select(a => (double)a).ToArray();
        }

        private void AddFigure(Func<Figure<Point2D, Matrix2D>> create)
        {
            _mainFigures.Add(Enumerable.Range(0, 4).Select(a=>create()).ToArray());
        }

        private void AddFigure(Func<double[], Figure<Point2D, Matrix2D>> create, double[] h)
        {
            var transformed = _transform.Transform(h);
            _mainFigures.Add(Enumerable.Range(0, 4).Select(a=>create(transformed[a])).ToArray());
        }

        private void AddFigure(Func<double[], Color[], Figure<Point2D, Matrix2D>> create, double[] h, Color[] c)
        {
            var transformed = _transform.Transform(h);
            var colors = _transform.Transform(c);
            _mainFigures.Add(Enumerable.Range(0, 4).Select(a=>create(transformed[a], colors[a])).ToArray());
        }

        private void AddFigure(Func<double[][], Figure<Point2D, Matrix2D>> create, double[][] h)
        {
            var transformed = new double[h.Length][][];
            for (var i = 0; i < h.Length; ++i)
            {
                transformed[i] = _transform.Transform(h[i]);
            }
            _mainFigures.Add(Enumerable.Range(0, 4).Select(
                a => create(Enumerable.Range(0, h.Length).Select(b => transformed[b][a]).ToArray())).ToArray());
        }

        private void ResetData()
        {
            _mainFigures.Clear();
        }

        private void RenderPicture(Graphics g)
        {
            foreach (var figure in _mainFigures)
            {
                for (var j = 0; j < _matrices.Length; ++j)
                {
                    figure[j].Render(g);
                }
            }
        }

        private void RecalculateMatrix(int type)
        {
            var moveventMatrix = new Matrix2D();
            var resizeMatrix = new Matrix2D();

            switch (type)
            {
                case 0:
                    MatrixAbstract.Multiply(
                        moveventMatrix.MovementMatrix(0, 1.0 / 3.0 * 200),
                        resizeMatrix.ResizeMatrix(1.0, 1.0),
                        _matrices[0]);
                    MatrixAbstract.Multiply(
                        moveventMatrix.MovementMatrix(0, 1.0 / 3.0 * 200),
                        resizeMatrix.ResizeMatrix(1.0, -1.0),
                        _matrices[1]);
                    MatrixAbstract.Multiply(
                        moveventMatrix.MovementMatrix(-200 / Math.Sqrt(3.0), -2.0 / 3.0 * 200),
                        resizeMatrix.ResizeMatrix(1.0, 1.0),
                        _matrices[2]);
                    MatrixAbstract.Multiply(
                        moveventMatrix.MovementMatrix(200 / Math.Sqrt(3.0), -2.0 / 3.0 * 200),
                        resizeMatrix.ResizeMatrix(1.0, 1.0),
                        _matrices[3]);
                    break;
                case 1:
                    MatrixAbstract.Multiply(
                        moveventMatrix.MovementMatrix(0, -1.0 / 3.0 * 100),
                        resizeMatrix.ResizeMatrix(2.0, 2.0),
                        _matrices[0]);
                    MatrixAbstract.Multiply(
                        moveventMatrix.MovementMatrix(-1000, -1000),
                        resizeMatrix.ResizeMatrix(1, 1),
                        _matrices[1]);
                    MatrixAbstract.Multiply(
                        moveventMatrix.MovementMatrix(-1000, -1000),
                        resizeMatrix.ResizeMatrix(1, 1),
                        _matrices[2]);
                    MatrixAbstract.Multiply(
                        moveventMatrix.MovementMatrix(-1000, -1000),
                        resizeMatrix.ResizeMatrix(1, 1),
                        _matrices[3]);
                    break;
                case 2:
                    MatrixAbstract.Multiply(
                        moveventMatrix.MovementMatrix(-1000, -1000),
                        resizeMatrix.ResizeMatrix(1, 1),
                        _matrices[0]);
                    MatrixAbstract.Multiply(
                        moveventMatrix.MovementMatrix(0, -1.0 / 3.0 * 100),
                        resizeMatrix.ResizeMatrix(2.0, 2.0),
                        _matrices[1]);
                    MatrixAbstract.Multiply(
                        moveventMatrix.MovementMatrix(-1000, -1000),
                        resizeMatrix.ResizeMatrix(1, 1),
                        _matrices[2]);
                    MatrixAbstract.Multiply(
                        moveventMatrix.MovementMatrix(-1000, -1000),
                        resizeMatrix.ResizeMatrix(1, 1),
                        _matrices[3]);
                    break;
                case 3:
                    MatrixAbstract.Multiply(
                        moveventMatrix.MovementMatrix(-1000, -1000),
                        resizeMatrix.ResizeMatrix(1, 1),
                        _matrices[0]);
                    MatrixAbstract.Multiply(
                        moveventMatrix.MovementMatrix(-1000, -1000),
                        resizeMatrix.ResizeMatrix(1, 1),
                        _matrices[1]);
                    MatrixAbstract.Multiply(
                        moveventMatrix.MovementMatrix(0, -1.0 / 3.0 * 100),
                        resizeMatrix.ResizeMatrix(2.0, 2.0),
                        _matrices[2]);
                    MatrixAbstract.Multiply(
                        moveventMatrix.MovementMatrix(-1000, -1000),
                        resizeMatrix.ResizeMatrix(1, 1),
                        _matrices[3]);
                    break;
                case 4:
                    MatrixAbstract.Multiply(
                        moveventMatrix.MovementMatrix(-1000, -1000),
                        resizeMatrix.ResizeMatrix(1, 1),
                        _matrices[0]);
                    MatrixAbstract.Multiply(
                        moveventMatrix.MovementMatrix(-1000, -1000),
                        resizeMatrix.ResizeMatrix(1, 1),
                        _matrices[1]);
                    MatrixAbstract.Multiply(
                        moveventMatrix.MovementMatrix(-1000, -1000),
                        resizeMatrix.ResizeMatrix(1, 1),
                        _matrices[2]);
                    MatrixAbstract.Multiply(
                        moveventMatrix.MovementMatrix(0, -1.0 / 3.0 * 100),
                        resizeMatrix.ResizeMatrix(2.0, 2.0),
                        _matrices[3]);
                    break;
            }
        }
    }
}
