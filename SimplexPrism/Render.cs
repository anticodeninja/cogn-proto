namespace Simplex3D
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    using Library;
    using Library.Graphics;
    using Library.Geometry;

    public partial class Render : Form
    {
        private readonly List<Figure<Point3D, Matrix3D>> _mainFigures;

        private readonly Matrix3D _projectionMatrix;
        private readonly Matrix3D _transformMatrix;
        private readonly Matrix3D _mouseXMatrix;
        private readonly Matrix3D _mouseYMatrix;

        private Point? _mouseCoord;

        public Render(string inputFile)
        {
            InitializeComponent();

            _mainFigures = new List<Figure<Point3D, Matrix3D>>();
            _projectionMatrix = new Matrix3D().IdentMatrix<Matrix3D>().Chain(
                new Matrix3D().MovementMatrix(0, 0, 800),
                new Matrix3D().ProjectionMatrix());
            _transformMatrix = new Matrix3D().IdentMatrix<Matrix3D>();
            _mouseXMatrix = new Matrix3D();
            _mouseYMatrix = new Matrix3D();

            var parser = new Parser(inputFile);
            InitializeParser(parser);
            Shown += (sender, args) => parser.Start();

            renderPanel.Paint += (sender, args) => RenderPicture(args.Graphics);
            InitializeMouse();
        }

        private void InitializeParser(Parser parser)
        {
            parser.Add("addPrism", new Action<string, int, object[], int, int>(
                (color, width, pattern, size, length) => 
                AddFigure(FigureFactory3D.CreatePrismBody(
                    size, length, width, ColorTranslator.FromHtml(color), ParsePattern(pattern)))));

            parser.Add("addIJK", new Action<string, int, object[], int, int, object[], object[]>(
                (color, width, pattern, size, length, point, colors) =>
                AddFigure(FigureFactory3D.CreatePrismIjk(
                    size, length, ParseValue(point),
                    width, ColorTranslator.FromHtml(color),
                    colors.Select(c => ColorTranslator.FromHtml((string)c)).ToArray(),
                    ParsePattern(pattern)))));

            parser.Add("addVector", new Action<string, int, object[], int, int, object[]>(
                (color, width, pattern, size, length, point) =>
                AddFigure(FigureFactory3D.CreatePrismVector(
                    size, length, ParseValue(point), width, ColorTranslator.FromHtml(color), ParsePattern(pattern)))));

            parser.Add("addPath", new Action<string, int, object[], int, object[]>(
                (color, width, pattern, size, path) =>
            {
                for (var i = 0; i < path.Length - 1; ++i)
                {
                    AddFigure(FigureFactory3D.CreateLine(size, ParseValue(path[i]), ParseValue(path[i + 1]),
                        width, ColorTranslator.FromHtml(color), ParsePattern(pattern))); 
                }
            }));

            parser.Add("addPoint", new Action<string, int, string, int, int, object>(
                (color, width, pattern, size, length, point) =>
                    AddFigure(FigureFactory3D.CreatePrismPoint(size, length, ParseValue(point),
                        width, ColorTranslator.FromHtml(color), (PointType) Enum.Parse(typeof(PointType), pattern)))));

            parser.Add("setViewPort", new Action<double, double>((a1, a2) =>
            {
                _mouseXMatrix.RotateMatrixX(a1 * Math.PI / 180);
                _mouseYMatrix.RotateMatrixY(a2 * Math.PI / 180);

                _transformMatrix.IdentMatrix<Matrix3D>().Chain(
                    _mouseYMatrix,
                    _mouseXMatrix);
            }));

            parser.Reloaded += (sender, args) => Invoke(new Action(() =>
            {
                if (args.Message == null)
                {
                    renderPanel.Invalidate();
                }
                else
                {
                    MessageBox.Show(args.Message);
                }
            }));
            parser.Reloading += (sender, args) => ResetData();
        }

        private void AddFigure(Figure<Point3D, Matrix3D> figure)
        {
            figure.SetProjection(_projectionMatrix);
            _mainFigures.Add(figure);
        }

        private static float[] ParsePattern(IEnumerable<object> array)
        {
            return array.Select(a => (float) (double) a).ToArray();
        }

        private double[] ParseValue(object array)
        {
            return ((object[])array).Select(a => (double)a).ToArray();
        }

        private void ResetData()
        {
            _mainFigures.Clear();
        }

        private void RenderPicture(Graphics g)
        {
            foreach (var figure in _mainFigures)
            {
                figure.SetTransform(_transformMatrix);
                figure.Render(g);
            }
        }

        private void InitializeMouse()
        {
            renderPanel.MouseDown += (sender, args) =>
            {
                _mouseCoord = new Point(args.X, args.Y);
            };
            renderPanel.MouseUp += (sender, args) =>
            {
                _mouseCoord = null;
            };
            renderPanel.MouseMove += (sender, args) =>
            {
                if (_mouseCoord == null)
                    return;

                var mouseDelta = new Point(args.X - _mouseCoord.Value.X, args.Y - _mouseCoord.Value.Y);
                _mouseCoord = new Point(args.X, args.Y);

                const double koef = 0.01;
                _mouseXMatrix.RotateMatrixX(koef * mouseDelta.Y);
                _mouseYMatrix.RotateMatrixY(koef * mouseDelta.X);

                _transformMatrix.Chain(
                    _mouseYMatrix,
                    _mouseXMatrix);

                renderPanel.Invalidate();
            };
        }
    }
}
