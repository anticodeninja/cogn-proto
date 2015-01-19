namespace Simplex3D
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    using Library;
    using Primitives;

    public partial class Render : Form
    {
        private readonly List<Figure> _mainFigures;

        private readonly Matrix _transformMatrix;
        private readonly Matrix _mouseXMatrix;
        private readonly Matrix _mouseYMatrix;
        private readonly Matrix _tempMatrix;

        private System.Drawing.Point? _mouseCoord;

        public Render(string inputFile)
        {
            InitializeComponent();

            _mainFigures = new List<Figure>();
            _transformMatrix = new Matrix().IdentMatrix();
            _mouseXMatrix = new Matrix();
            _mouseYMatrix = new Matrix();
            _tempMatrix = new Matrix();

            var parser = new Parser(inputFile);
            InitializeParser(parser);
            Shown += (sender, args) => parser.Start();

            renderPanel.Paint += (sender, args) => RenderPicture(args.Graphics);
            InitializeMouse();
        }

        private void InitializeParser(Parser parser)
        {
            parser.Add("addTetraedron", new Action<string, int, object[], int>(
                (color, width, pattern, size) => 
                _mainFigures.Add(Figure.CreateTetrahedron(
                    size, width, ColorTranslator.FromHtml(color), ParsePattern(pattern)))));

            parser.Add("addIJK", new Action<string, int, object[], int, object[], object[]>(
                (color, width, pattern, size, point, colors) =>
                _mainFigures.Add(Figure.CreateIjk(
                    size, ParseValue(point),
                    width, ColorTranslator.FromHtml(color),
                    colors.Select(c => ColorTranslator.FromHtml((string)c)).ToArray(),
                    ParsePattern(pattern)))));

            parser.Add("addVector", new Action<string, int, object[], int, object[]>(
                (color, width, pattern, size, point) =>
                _mainFigures.Add(Figure.CreateVector(
                    size, ParseValue(point), width, ColorTranslator.FromHtml(color), ParsePattern(pattern)))));

            parser.Add("addPath", new Action<string, int, object[], int, object[]>(
                (color, width, pattern, size, path) =>
            {
                for (var i = 0; i < path.Length - 1; ++i)
                {
                    _mainFigures.Add(Figure.CreateLine(size, ParseValue(path[i]), ParseValue(path[i + 1]),
                        width, ColorTranslator.FromHtml(color), ParsePattern(pattern))); 
                }
            }));

            parser.Add("addPoint", new Action<string, int, string, int, object>(
                (color, width, pattern, size, point) =>
                    _mainFigures.Add(Figure.CreatePoint(size, ParseValue(point),
                        width, ColorTranslator.FromHtml(color), (PointType) Enum.Parse(typeof(PointType), pattern)))));

            parser.Add("setViewPort", new Action<double, double>((a1, a2) =>
            {
                _mouseXMatrix.RotateMatrixX(a1 * Math.PI / 180);
                _mouseYMatrix.RotateMatrixY(a2 * Math.PI / 180);

                _transformMatrix.IdentMatrix();
                Matrix.Multiply(_transformMatrix, _mouseYMatrix, _tempMatrix);
                Matrix.Multiply(_tempMatrix, _mouseXMatrix, _transformMatrix);
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
                _mouseCoord = new System.Drawing.Point(args.X, args.Y);
            };
            renderPanel.MouseUp += (sender, args) =>
            {
                _mouseCoord = null;
            };
            renderPanel.MouseMove += (sender, args) =>
            {
                if (_mouseCoord == null)
                    return;

                var mouseDelta = new System.Drawing.Point(args.X - _mouseCoord.Value.X, args.Y - _mouseCoord.Value.Y);
                _mouseCoord = new System.Drawing.Point(args.X, args.Y);

                const double koef = 0.01;
                _mouseXMatrix.RotateMatrixX(koef * mouseDelta.Y);
                _mouseYMatrix.RotateMatrixY(koef * mouseDelta.X);
                Matrix.Multiply(_transformMatrix, _mouseXMatrix, _tempMatrix);
                Matrix.Multiply(_tempMatrix, _mouseYMatrix, _transformMatrix);

                renderPanel.Invalidate();
            };
        }
    }
}
