namespace Simplex2D
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using Library;
    using Primitivies;

    public partial class Render : Form
    {
        private readonly List<Figure> _mainFigures;
        private readonly Matrix[] _matrices;
        private int _transformType;

        public Render(string inputFile)
        {
            InitializeComponent();

            _mainFigures = new List<Figure>();
            _matrices = new Matrix[4];
            for (var i = 0; i < _matrices.Length; ++i)
                _matrices[i] = new Matrix().IdentMatrix();

            var parser = new Parser(inputFile);
            InitializeParser(parser);
            Shown += (sender, args) => parser.Start();

            renderPanel.Paint += (sender, args) => RenderPicture(args.Graphics);
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
                    ParsePattern(pattern), _transformType))));

            parser.Add("addVector", new Action<string, int, object[], int, object[]>(
                (color, width, pattern, size, point) =>
                _mainFigures.Add(Figure.CreateVector(
                    size, ParseValue(point),
                    width, ColorTranslator.FromHtml(color),
                    ParsePattern(pattern), _transformType))));

            parser.Add("addPath", new Action<string, int, object[], int, object[]>(
                (color, width, pattern, size, path) =>
                {
                    for (var i = 0; i < path.Length - 1; ++i)
                    {
                        _mainFigures.Add(Figure.CreateLine(
                            size, ParseValue(path[i]), ParseValue(path[i + 1]),
                            width, ColorTranslator.FromHtml(color),
                            ParsePattern(pattern), _transformType));
                    }
                }));

            parser.Add("addPoint", new Action<string, int, string, int, object>(
                (color, width, pattern, size, point) =>
                    _mainFigures.Add(Figure.CreatePoint(
                        size, ParseValue(point),
                        width, ColorTranslator.FromHtml(color),
                        (PointType)Enum.Parse(typeof(PointType), pattern), _transformType))));

            parser.Add("setTransform", new Action<int>(transform =>
            {
                _transformType = transform;
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
                            figure.SetTransform(j, _matrices[j]);
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

        private void ResetData()
        {
            _mainFigures.Clear();
        }

        private void RenderPicture(Graphics g)
        {
            foreach (var figure in _mainFigures)
            {
                figure.Render(g);
            }
        }

        private void RecalculateMatrix(int type)
        {
            var moveventMatrix = new Matrix();
            var resizeMatrix = new Matrix();

            switch (type)
            {
                case 0:
                    Matrix.Multiply(
                        moveventMatrix.MovementMatrix(0, 1.0 / 3.0 * 200),
                        resizeMatrix.ResizeMatrix(1.0, 1.0),
                        _matrices[0]);
                    Matrix.Multiply(
                        moveventMatrix.MovementMatrix(0, 1.0 / 3.0 * 200),
                        resizeMatrix.ResizeMatrix(1.0, -1.0),
                        _matrices[1]);
                    Matrix.Multiply(
                        moveventMatrix.MovementMatrix(-200 / Math.Sqrt(3.0), -2.0 / 3.0 * 200),
                        resizeMatrix.ResizeMatrix(1.0, 1.0),
                        _matrices[2]);
                    Matrix.Multiply(
                        moveventMatrix.MovementMatrix(200 / Math.Sqrt(3.0), -2.0 / 3.0 * 200),
                        resizeMatrix.ResizeMatrix(1.0, 1.0),
                        _matrices[3]);
                    break;
                case 1:
                    Matrix.Multiply(
                        moveventMatrix.MovementMatrix(0, -1.0 / 3.0 * 100),
                        resizeMatrix.ResizeMatrix(2.0, 2.0),
                        _matrices[0]);
                    Matrix.Multiply(
                        moveventMatrix.MovementMatrix(-1000, -1000),
                        resizeMatrix.ResizeMatrix(1, 1),
                        _matrices[1]);
                    Matrix.Multiply(
                        moveventMatrix.MovementMatrix(-1000, -1000),
                        resizeMatrix.ResizeMatrix(1, 1),
                        _matrices[2]);
                    Matrix.Multiply(
                        moveventMatrix.MovementMatrix(-1000, -1000),
                        resizeMatrix.ResizeMatrix(1, 1),
                        _matrices[3]);
                    break;
                case 2:
                    Matrix.Multiply(
                        moveventMatrix.MovementMatrix(-1000, -1000),
                        resizeMatrix.ResizeMatrix(1, 1),
                        _matrices[0]);
                    Matrix.Multiply(
                        moveventMatrix.MovementMatrix(0, -1.0 / 3.0 * 100),
                        resizeMatrix.ResizeMatrix(2.0, 2.0),
                        _matrices[1]);
                    Matrix.Multiply(
                        moveventMatrix.MovementMatrix(-1000, -1000),
                        resizeMatrix.ResizeMatrix(1, 1),
                        _matrices[2]);
                    Matrix.Multiply(
                        moveventMatrix.MovementMatrix(-1000, -1000),
                        resizeMatrix.ResizeMatrix(1, 1),
                        _matrices[3]);
                    break;
                case 3:
                    Matrix.Multiply(
                        moveventMatrix.MovementMatrix(-1000, -1000),
                        resizeMatrix.ResizeMatrix(1, 1),
                        _matrices[0]);
                    Matrix.Multiply(
                        moveventMatrix.MovementMatrix(-1000, -1000),
                        resizeMatrix.ResizeMatrix(1, 1),
                        _matrices[1]);
                    Matrix.Multiply(
                        moveventMatrix.MovementMatrix(0, -1.0 / 3.0 * 100),
                        resizeMatrix.ResizeMatrix(2.0, 2.0),
                        _matrices[2]);
                    Matrix.Multiply(
                        moveventMatrix.MovementMatrix(-1000, -1000),
                        resizeMatrix.ResizeMatrix(1, 1),
                        _matrices[3]);
                    break;
                case 4:
                    Matrix.Multiply(
                        moveventMatrix.MovementMatrix(-1000, -1000),
                        resizeMatrix.ResizeMatrix(1, 1),
                        _matrices[0]);
                    Matrix.Multiply(
                        moveventMatrix.MovementMatrix(-1000, -1000),
                        resizeMatrix.ResizeMatrix(1, 1),
                        _matrices[1]);
                    Matrix.Multiply(
                        moveventMatrix.MovementMatrix(-1000, -1000),
                        resizeMatrix.ResizeMatrix(1, 1),
                        _matrices[2]);
                    Matrix.Multiply(
                        moveventMatrix.MovementMatrix(0, -1.0 / 3.0 * 100),
                        resizeMatrix.ResizeMatrix(2.0, 2.0),
                        _matrices[3]);
                    break;
            }
        }
    }
}
