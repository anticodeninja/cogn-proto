namespace HexRender
{
    using System;
    using System.Drawing.Drawing2D;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using Library;

    public partial class Render : Form
    {
        private enum Mode
        {
            Pie,
            ColorRound,
            BlackRound,
        }

        internal class Student
        {
            public string Name { get; set; }
            public float[][] Grades { get; set; }
            public string Info { get; set; }
        }

        private Dictionary<string, string> _localizations;
        private List<Student> _students;
        private List<int> _part;
        private List<float> _grades;
        private List<string> _dates;
        private List<Brush> _brushes;
        private List<Pen> _additionalData;
        private List<string> _additionalName;
        private int _width;
        private int _height;
        private float _ratio;
        private Mode _mode;
        private List<float> _modeParam;
        private int _hatchRatio;

        private Font _font;
        private Font _smallFont;
        private StringFormat _leftFormat;
        private StringFormat _rightFormat;

        public Render(string inputFile)
        {
            InitializeComponent();
            InitializeResources();

            var parser = new Parser(inputFile);
            InitializeParser(parser);
            Shown += (sender, args) => parser.Start();

            renderPanel.Paint += (sender, args) => RenderPicture(args.Graphics);
        }

        private void InitializeParser(Parser parser)
        {
            parser.Add("addStudent", new Action<dynamic>(student =>
                _students.Add(new Student
                {
                    Name = student.n,
                    Info = student.a,
                    Grades = ((object[]) student.g)
                        .Select(a => ((object[]) a)
                            .Select(b => (float) (double) b)
                            .ToArray())
                        .ToArray()
                })));

            parser.Add("addDelimiter", new Action(() => 
                _part.Add(_students.Count)
                ));

            parser.Add("setDates", new Action<object[]>(dates =>
                _dates.AddRange(dates.Cast<string>())
                ));


            parser.Add("setViewPort", new Action<int, int, float, int>((width, height, ratio, render) =>
            {
                _width = width;
                _height = height;
                _ratio = ratio;
            }));

            parser.Add("setTitles", new Action<dynamic>(parameters =>
            {
                foreach (dynamic parameter in parameters)
                {
                    _localizations.Add(parameter.Key, parameter.Value);
                }
            }));

            parser.Add("setThresholds", new Action<object[]>(grades =>
                _grades.AddRange(grades.Select(a => (float)(double)a))
                ));

            parser.Add("setAdditionalsParam", new Action<object[]>(parameters =>
            {
                foreach (dynamic param in parameters)
                {
                    _additionalName.Add(param.t);
                    _additionalData.Add(new Pen(ColorTranslator.FromHtml(param.c), (float)param.w)
                    {
                        DashPattern = ((object[])param.p).Select(a=>(float)(double)a).ToArray()
                    });
                }
            }));


            parser.Add("setPieMode", new Action<dynamic>(colors =>
            {
                _mode = Mode.Pie;
                foreach (dynamic color in colors)
                {
                    _brushes.Add(new HatchBrush(
                        (HatchStyle)Enum.Parse(typeof(HatchStyle), color.p),
                        ColorTranslator.FromHtml(color.m), ColorTranslator.FromHtml(color.s)));
                }
            }));

            parser.Add("setColorRoundMode", new Action<object[]>(parameters =>
            {
                _mode = Mode.ColorRound;
                _modeParam.AddRange(parameters.Select(a=>(float)(double) a));
            }));

            parser.Add("setBlackRoundMode", new Action<int>(ratio =>
            {
                _mode = Mode.BlackRound;
                _hatchRatio = ratio;
            }));


            parser.Reloaded += (sender, args) => Invoke(new Action(() =>
            {
                if (args.Message == null)
                {
                    Width += _width - renderPanel.Width;
                    Height += _height - renderPanel.Height;
                    renderPanel.Invalidate();
                }
                else
                {
                    MessageBox.Show(args.Message);
                }
            }));
            parser.Reloading += (sender, args) => ResetData();
        }

        private void ResetData()
        {
            _localizations = new Dictionary<string, string>();
            _students = new List<Student>();
            _grades = new List<float>();
            _dates = new List<string>();
            _brushes = new List<Brush>();
            _part = new List<int> {0};
            _additionalData = new List<Pen>();
            _mode = 0;
            _modeParam = new List<float>();
            _additionalName = new List<string>();
        }

        public float TranslateGradeToLum(float grade)
        {
            var index = 0;
            while (grade > _grades[index])
                index += 1;

            return _modeParam[index + 2] + (_modeParam[index + 3] - _modeParam[index + 2])*grade/100.0f;
        }

        public Brush TranslateGradeToHatch(float grade)
        {
            var matches = new[]
            {
                Tuple.Create(10, HatchStyle.Percent05),
                Tuple.Create(20, HatchStyle.Percent10),
                Tuple.Create(30, HatchStyle.Percent20),
                Tuple.Create(40, HatchStyle.Percent25),
                Tuple.Create(50, HatchStyle.Percent30),
                Tuple.Create(60, HatchStyle.Percent40),
                Tuple.Create(70, HatchStyle.Percent50),
                Tuple.Create(80, HatchStyle.Percent60),
                Tuple.Create(90, HatchStyle.Percent70),
            };

            for (var i = matches.Length - 1; i >= 0; i -= _hatchRatio)
            {
                if (grade >= matches[i].Item1)
                {
                    return new HatchBrush(matches[i].Item2, Color.Black, Color.White);
                }
            }

            return new SolidBrush(Color.White);
        }

        private void RenderPicture(Graphics g)
        {
            var angleOffset = -Math.PI / 2;
            var anglePerStudent = Math.PI * 2 / _students.Count;
            var center = new Point(_width / 2, _height / 2);
            var maxRadius = Math.Min(center.X, center.Y) * _ratio / 100.0f;

            Func<int, float, float, Point> getPoint = (id, delta, lRadius) =>
            {
                var radius = maxRadius * lRadius / 100.0f;
                var angle = angleOffset + anglePerStudent * (id + delta / 100.0f);
                return center + new Size((int)(radius * Math.Cos(angle)), (int)(radius * Math.Sin(angle)));
            };

            Func<float, Rectangle> getRectangle = lRadius =>
            {
                var delta = maxRadius*lRadius/100.0f;
                return new Rectangle((int)(center.X - delta), (int)(center.Y - delta), (int) (2*delta), (int) (2*delta));
            };

            Func<double, float> toDeg = rad => (float) (rad / Math.PI * 180);

            // Пороговые линии
            for (var j = 0; j < _grades.Count; ++j)
            {
                g.DrawPie(new Pen(Color.Black, j == (_grades.Count - 1) ? 4 : 2) {DashPattern = new[] {3.0f, 5.0f}},
                    getRectangle(_grades[j]), 0.0f, 360.0f);
            }

            // Основные и дополнительные зависимости
            for (var i = 0; i < _students.Count; ++i)
            {
                var grades = _students[i].Grades;

                var mainGrades = (_mode == Mode.Pie || _mode == Mode.Pie)
                        ? new[]{0.0f}.Concat(grades[0]).ToList()
                        : Enumerable.Range(0, grades[0].Length + 1).Select(a => a * 100.0f / grades[0].Length).ToList();

                // Основная зависимость
                for (var j = mainGrades.Count - 1; j >= 1; --j)
                {
                    if (Math.Abs(mainGrades[j]) > Parameters.Tolerance)
                    {
                        var brush = GetBrush(j, grades);

                        g.FillPie(brush, getRectangle(mainGrades[j]),
                            toDeg(angleOffset + anglePerStudent * i), toDeg(anglePerStudent));
                        g.DrawPie(new Pen(Color.Black, 2), getRectangle(mainGrades[j]),
                            toDeg(angleOffset + anglePerStudent * i), toDeg(anglePerStudent));
                    }
                }

                // Дополнительные зависимости
                for (var k = 1; k < grades.Length; ++k)
                {
                    var addGrades = new List<float> {0.0f};
                    addGrades.AddRange(grades[k]);

                    if (addGrades.All(a => a < Parameters.Tolerance))
                        continue;

                    for (var j = 0; j < addGrades.Count - 1; ++j)
                        g.DrawLine(_additionalData[(k - 1) % _additionalData.Count],
                            getPoint(i, addGrades[j], mainGrades[j]),
                            getPoint(i, addGrades[j + 1], mainGrades[j + 1]));
                }
            }

            // Разделительные линии между секторами
            for (var i = 0; i < _students.Count; ++i)
            {
                g.DrawLine(new Pen(Color.Black, 2), center, getPoint(i, 0, 110));
            }

            // Разделители между секторами
            for (var j = 0; j < _part.Count; ++j)
            {
                g.DrawLine(new Pen(Color.Black, 6), center, getPoint(_part[j], 0, 110));
            }

            var half = (int)Math.Ceiling(_students.Count / 2.0);

            // Имена студентов
            for (var i = 0; i < _students.Count; ++i)
            {
                var posRight = i < half;
                var point = getPoint(i, 50, 0.6f * maxRadius);
                var message = string.Format("{0} ({1})", _students[i].Name, _students[i].Info);
                g.DrawString(message, _font, Brushes.Black, point, posRight ? _rightFormat : _leftFormat);
            }

            var lOffset = new Point(470, 30);
            var lOffset2 = center + new Size(lOffset.X, lOffset.Y);
            const int lSize = 30;

            if (_mode == Mode.BlackRound || _mode == Mode.ColorRound)
            {
                lOffset = new Point(530, 30);
                lOffset2 = center + new Size(lOffset.X, lOffset.Y);

                // Дата прохождения теста
                g.DrawString(_localizations["passingTestDate"], _smallFont, Brushes.Black,
                    lOffset2.X - 30, lOffset2.Y - 215);

                for (var j = 0; j < _dates.Count; ++j)
                {
                    var rect = getRectangle(100 / _dates.Count * (j + 1));
                    rect.Offset(lOffset);
                    var center2 = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

                    g.DrawPie(new Pen(Color.Black, j == (_dates.Count - 1) ? 4 : 2) { DashPattern = new[] { 3.0f, 5.0f } },
                        rect, 270, 40);
                    g.DrawString(_dates[j], _smallFont, Brushes.Black,
                        center2 + new Size(-5, (int)(-(j + 1) * maxRadius / _dates.Count)), _leftFormat);

                    if ((_grades.Count - 1) == j)
                    {
                        g.DrawLine(new Pen(Brushes.Black, 4.0f),
                            center2, center2 + new Size((int)(maxRadius * Math.Cos(-Math.PI / 180 * 90)),
                                (int)(maxRadius * Math.Sin(-Math.PI / 180 * 90))));
                        g.DrawLine(new Pen(Brushes.Black, 4.0f),
                            center2, center2 + new Size((int)(maxRadius * Math.Cos(-Math.PI / 180 * 50)),
                                (int)(maxRadius * Math.Sin(-Math.PI / 180 * 50))));
                    }
                }

                var gradeOffsetY = lOffset2.Y + (int)(1.5 * lSize);
                var gradeSizeY = lSize * 4;
                var gradeOffset = 1.0f;

                // Количество баллов за тест
                g.DrawString(_localizations["scorePerTest"], _smallFont, Brushes.Black,
                    lOffset2.X - 30, lOffset2.Y + 15);

                var grades2 = new[] { 0.0f }.Concat(_grades.Take(_grades.Count - 1)).ToArray();
                if (_mode == Mode.ColorRound)
                {
                    for (var i = grades2.Length - 1; i >= 0; --i)
                    {
                        var grade = grades2[i] / 100.0f;
                        var fullRect = getRectangle(grade * 100.0f);
                        fullRect.Offset(lOffset);

                        var pgb =
                            new LinearGradientBrush(
                                new Point { X = 0, Y = gradeOffsetY + (int)(gradeSizeY * (1 - gradeOffset)) },
                                new Point { X = 0, Y = gradeOffsetY + (int)(gradeSizeY * (1 - grade)) },
                                new HslColor(_modeParam[0], _modeParam[1], _modeParam[i + 3]),
                                new HslColor(_modeParam[0], _modeParam[1], _modeParam[i + 2])
                                );

                        g.FillRectangle(pgb,
                            lOffset2.X, gradeOffsetY + (int)(gradeSizeY * (1 - gradeOffset)),
                            lSize, (int)(gradeSizeY * gradeOffset));

                        gradeOffset = grade;
                    }
                }
                else
                {
                    gradeOffset = 1.0f;
                    for (var i = 100; i >= 0; i -= 10)
                    {
                        var fullRect = getRectangle(i);
                        fullRect.Offset(lOffset);

                        var pgb = TranslateGradeToHatch(i);

                        g.FillRectangle(pgb,
                            lOffset2.X, gradeOffsetY + (int)(gradeSizeY * (1 - gradeOffset)),
                            lSize, (int)(gradeSizeY * (gradeOffset - i / 100.0f)));

                        gradeOffset = i/100.0f;
                    }
                }

                gradeOffset = 1.0f;
                for (var i = grades2.Length - 1; i >= 0; --i)
                {
                    var grade = grades2[i] / 100.0f;
                    var fullRect = getRectangle(grade * 100.0f);
                    fullRect.Offset(lOffset);

                    g.DrawString(_grades[i].ToString(), _smallFont, Brushes.Black, lOffset2.X + 2 * lSize,
                        gradeOffsetY + (int)(gradeSizeY * (1 - gradeOffset)), _leftFormat);

                    gradeOffset = grade;
                }

                g.DrawRectangle(Pens.Black, lOffset2.X, gradeOffsetY, lSize, gradeSizeY);
            }
            else
            {
                // Количество баллов за тест
                g.DrawString(_localizations["scorePerTest"], _smallFont, Brushes.Black,
                    lOffset2.X - 30, lOffset2.Y - 215);
                for (var j = 0; j < _grades.Count; ++j)
                {
                    var rect = getRectangle(_grades[j]);
                    rect.Offset(lOffset);
                    var center2 = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

                    g.DrawPie(new Pen(Color.Black, j == (_grades.Count - 1) ? 4 : 2) { DashPattern = new[] { 3.0f, 5.0f } },
                        rect, 270, 40);
                    g.DrawString(_grades[j].ToString(), _smallFont, Brushes.Black,
                        center2 + new Size(-5, (int)(-_grades[j] * maxRadius / 100)), _leftFormat);

                    if ((_grades.Count - 1) == j)
                    {
                        g.DrawLine(new Pen(Brushes.Black, 4.0f),
                            center2, center2 + new Size((int)(maxRadius * Math.Cos(-Math.PI / 180 * 90)),
                                (int)(maxRadius * Math.Sin(-Math.PI / 180 * 90))));
                        g.DrawLine(new Pen(Brushes.Black, 4.0f),
                            center2, center2 + new Size((int)(maxRadius * Math.Cos(-Math.PI / 180 * 50)),
                                (int)(maxRadius * Math.Sin(-Math.PI / 180 * 50))));
                    }
                }

                // Дата прохождения теста
                g.DrawString(_localizations["passingTestDate"], _smallFont, Brushes.Black, lOffset2.X - 30, lOffset2.Y + 15);
                for (var j = 0; j < _dates.Count; ++j)
                {
                    var lYOffset = (int)(lOffset2.Y + (j + 1) * lSize * 1.5);

                    if (_mode == 0)
                        g.FillRectangle(_brushes[j % _brushes.Count], lOffset2.X, lYOffset, lSize, lSize);
                    g.DrawRectangle(new Pen(Brushes.Black, 2.0f), lOffset2.X, lYOffset, lSize, lSize);
                    g.DrawString(_dates[j], _smallFont, Brushes.Black, lOffset2.X + lSize, lYOffset);
                }
            }

            // Легенда дополнительных зависимостей
            var additionalLegendStartPoint = center + new Size((int)(-2.4 * maxRadius), -30);
            for (var k = 0; k < _additionalName.Count; ++k)
            {
                var nameSize = g.MeasureString(_additionalName[k], _smallFont);

                g.DrawLine(_additionalData[k],
                    additionalLegendStartPoint,
                    additionalLegendStartPoint + new Size(-(int)nameSize.Width, 0));
                additionalLegendStartPoint -= new Size(0, (int)_additionalData[k].Width + 10);

                g.DrawString(_additionalName[k], _smallFont, Brushes.Black,
                    additionalLegendStartPoint, _leftFormat);
                additionalLegendStartPoint -= new Size(0, (int)nameSize.Height + 30);
            }
        }

        private void InitializeResources()
        {
            _font = new Font("Arial", 16);
            _smallFont = new Font("Arial", 12);

            _leftFormat = new StringFormat
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Center
            };
            _rightFormat = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center
            };
        }

        private Brush GetBrush(int j, float[][] grades)
        {
            switch (_mode)
            {
                case Mode.Pie:
                    return _brushes[(j - 1)%_brushes.Count];
                case Mode.ColorRound:
                    return new SolidBrush(new HslColor(_modeParam[0], _modeParam[1],
                        TranslateGradeToLum(grades[0][j - 1])));
                case Mode.BlackRound:
                    return TranslateGradeToHatch(grades[0][j - 1]);
                default:
                    throw new ArgumentException("Incorrect mode");
            }
        }
    }
}
