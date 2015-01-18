using System.Drawing.Drawing2D;
using System.Dynamic;

namespace HexRender
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using Library;

    public partial class Render : Form
    {
        private const string PassingTestDate = "Дата прохождения теста:";
        private const string ScorePerTest = "Количество баллов за тест:";

        //private const string PassingTestDate = "Passing Test Date:";
        //private const string ScorePerTest = "Score Per Test:";

        internal class Student
        {
            public string Name { get; set; }
            public float[][] Grades { get; set; }
            public string Info { get; set; }
        }

        private bool _correct;
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
        private int _render;
        private int _mode;
        private List<float> _modeParam;

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

            parser.Add("setColors", new Action<object[]>(colors =>
            {
                foreach (dynamic color in colors)
                {
                    _brushes.Add(new HatchBrush(
                        (HatchStyle)Enum.Parse(typeof(HatchStyle), color.p),
                        ColorTranslator.FromHtml(color.m), ColorTranslator.FromHtml(color.s)));
                }
            }));

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

            parser.Add("setThresholds", new Action<object[]>(grades =>
                _grades.AddRange(grades.Select(a => (float) (double) a))
                ));
            parser.Add("setParams", new Action<object[]>(param =>
                _modeParam.AddRange(param.Select(a => (float) (double) a))
                ));
            parser.Add("setDates", new Action<object[]>(dates =>
                _dates.AddRange(dates.Cast<string>())
                ));

            parser.Add("setViewPort", new Action<int, int, float, int>((width, height, ratio, render) => 
            {
                _width = width;
                _height = height;
                _ratio = ratio;
                _render = render;
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
            _correct = false;
            _students = new List<Student>();
            _grades = new List<float>();
            _dates = new List<string>();
            _brushes = new List<Brush>();
            _part = new List<int>() {0};
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
            HatchStyle style;

            if (grade < 60)
            {
                style = HatchStyle.Percent05;
            }
            else if (grade < 75)
            {
                style = HatchStyle.Percent20;
            }
            else if (grade < 90)
            {
                style = HatchStyle.Percent30;
            }
            else
            {
                style = HatchStyle.Percent50;
            }

            return new HatchBrush(style, Color.Black, Color.White);
        }

        private void renderPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            var center = new Point(_width / 2, _height / 2);
            var maxRadius = Math.Min(center.X, center.Y) * _ratio / 100.0f;
            var anglePerStudent = Math.PI * 2 / _students.Count;
            var angleOffset = -Math.PI / 2;
            var anglePerStudent2 = 360.0f / _students.Count;
            var angleOffset2 = -90.0f;

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

            for (var j = 0; j < _grades.Count; ++j)
                g.DrawPie(new Pen(Color.Black, j == (_grades.Count - 1) ? 4 : 2){DashPattern = new []{3.0f, 5.0f}},
                    getRectangle(_grades[j]), 0.0f, 360.0f);

            for (var i = 0; i < _students.Count; ++i)
            {
                var grades = _students[i].Grades;

                var mainGrades = _mode != 1
                        ? new[]{0.0f}.Concat(grades[0]).ToList()
                        : Enumerable.Range(0, grades[0].Length + 1).Select(a => a * 100.0f / grades[0].Length).ToList();

                // Main Info Rendering
                for (var j = mainGrades.Count - 1; j >= 1; --j)
                {
                    if (Math.Abs(mainGrades[j]) > Parameters.Tolerance)
                    {
                        var brush = GetBrush(j, grades);

                        g.FillPie(brush, getRectangle(mainGrades[j]),
                            angleOffset2 + anglePerStudent2 * i, anglePerStudent2);
                        g.DrawPie(new Pen(Color.Black, 2), getRectangle(mainGrades[j]),
                            angleOffset2 + anglePerStudent2 * i, anglePerStudent2);
                    }
                }

                // Additional Info Rendering
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

            for (var i = 0; i < _students.Count; ++i)
            {
                g.DrawLine(new Pen(Color.Black, 2), center, getPoint(i, 0, 110));
            }

            for (var j = 0; j < _part.Count; ++j)
            {
                g.DrawLine(new Pen(Color.Black, 6), center, getPoint(_part[j], 0, 110));
            }

            var half = (int) Math.Ceiling(_students.Count/2.0);
            var distance = (int)((2*maxRadius)/(half - 1));
            var leftOffset = center + new Size((int) (-maxRadius*1.2), (int) (-maxRadius));
            var rightOffset = center + new Size((int) (maxRadius * 1.2), (int) (-maxRadius));

            for (var i = 0; i < _students.Count; ++i)
            {
                var posRight = i < half;
                //var point = (posRight ? rightOffset : leftOffset) + new Size(0, distance*(posRight ? i : i - half));
                var point = getPoint(i, 50, 0.6f * maxRadius);
                var message = string.Format("{0} ({1})", _students[i].Name, _students[i].Info);
                g.DrawString(message, _font, Brushes.Black, point, posRight ? _rightFormat : _leftFormat);
            }

            var l_offset = new Point(470, 30);
            var l_offset2 = center + new Size(l_offset.X, l_offset.Y);
            var l_size = 30;

            if (_mode == 1)
            {
                l_offset = new Point(530, 30);
                l_offset2 = center + new Size(l_offset.X, l_offset.Y);

                g.DrawString(PassingTestDate, _smallFont, Brushes.Black, l_offset2.X - 30, l_offset2.Y - 215);

                var grades = new[] {0.0f}.Concat(_grades).ToArray();

                for (var j = 0; j < _dates.Count; ++j)
                {
                    var rect = getRectangle(100/_dates.Count*(j + 1));
                    rect.Offset(l_offset);
                    var center2 = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

                    g.DrawPie(new Pen(Color.Black, j == (_dates.Count - 1) ? 4 : 2) { DashPattern = new[] { 3.0f, 5.0f } },
                        rect, 270, 40);
                    g.DrawString(_dates[j], _smallFont, Brushes.Black,
                        center2 + new Size(-5, (int)(-(j+1) * maxRadius / _dates.Count)), _leftFormat);

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

                var gradeOffsetY = l_offset2.Y + (int)(1.5 * l_size);
                var gradeSizeY = l_size * 4;
                var gradeOffset = 1.0f;
                
                g.DrawString(ScorePerTest, _smallFont, Brushes.Black, l_offset2.X - 30, l_offset2.Y + 15);
                var grades2 = new[] {0.0f}.Concat(_grades.Take(_grades.Count - 1)).ToArray();
                for (var i = grades2.Length - 1; i >= 0; --i)
                {
                    var grade = grades2[i] / 100.0f;
                    var fullRect = getRectangle(grade * 100.0f);
                    fullRect.Offset(l_offset);

                    if (_mode == 0)
                    {
                        var pgb =
                            new LinearGradientBrush(
                                new Point() {X = 0, Y = gradeOffsetY + (int) (gradeSizeY*(1 - gradeOffset))},
                                new Point() {X = 0, Y = gradeOffsetY + (int) (gradeSizeY*(1 - grade))},
                                new HslColor(_modeParam[0], _modeParam[1], _modeParam[i + 3]),
                                new HslColor(_modeParam[0], _modeParam[1], _modeParam[i + 2])
                                );

                        g.FillRectangle(pgb,
                            l_offset2.X, gradeOffsetY + (int)(gradeSizeY * (1 - gradeOffset)),
                            l_size, (int)(gradeSizeY * gradeOffset));
                    }
                    else
                    {
                        var pgb = TranslateGradeToHatch(grades2[i]);

                        g.FillRectangle(pgb,
                            l_offset2.X, gradeOffsetY + (int)(gradeSizeY * (1 - gradeOffset)),
                            l_size, (int)(gradeSizeY * (gradeOffset - grade)));
                    }

                    g.DrawString(_grades[i].ToString(), _smallFont, Brushes.Black, l_offset2.X + 2 * l_size,
                        gradeOffsetY + (int)(gradeSizeY * (1 - gradeOffset)), _leftFormat);

                    gradeOffset = grade;
                }

                g.DrawRectangle(Pens.Black, l_offset2.X, gradeOffsetY, l_size, gradeSizeY);
            }
            else
            {
                g.DrawString(ScorePerTest, _smallFont, Brushes.Black, l_offset2.X - 30, l_offset2.Y - 215);

                for (var j = 0; j < _grades.Count; ++j)
                {
                    var rect = getRectangle(_grades[j]);
                    rect.Offset(l_offset);
                    var center2 = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

                    g.DrawPie(new Pen(Color.Black, j == (_grades.Count - 1) ? 4 : 2) { DashPattern = new[] { 3.0f, 5.0f } },
                        rect, 270, 40);
                    g.DrawString(_grades[j].ToString(), _smallFont, Brushes.Black,
                        center2 + new Size(-5, (int)(-_grades[j] * maxRadius / 100)), _leftFormat);

                    if (j == 0)
                        g.DrawString(PassingTestDate, _smallFont, Brushes.Black, l_offset2.X - 30, l_offset2.Y + 15);

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

                g.DrawString(PassingTestDate, _smallFont, Brushes.Black, l_offset2.X - 30, l_offset2.Y + 15);
                for (var j = 0; j < _dates.Count; ++j)
                {
                    var l_y_offset = (int)(l_offset2.Y + (j + 1) * l_size * 1.5);

                    if (_mode == 0)
                        g.FillRectangle(_brushes[j % _brushes.Count], l_offset2.X, l_y_offset, l_size, l_size);
                    g.DrawRectangle(new Pen(Brushes.Black, 2.0f), l_offset2.X, l_y_offset, l_size, l_size);
                    g.DrawString(_dates[j], _smallFont, Brushes.Black, l_offset2.X + l_size, l_y_offset);
                }
            }

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
                additionalLegendStartPoint -= new Size(0, (int) nameSize.Height + 30);
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
                case 0:
                    return _brushes[(j - 1)%_brushes.Count];
                case 2:
                    return new SolidBrush(new HslColor(_modeParam[0], _modeParam[1],
                        TranslateGradeToLum(grades[0][j - 1])));
                case 1:
                    return TranslateGradeToHatch(grades[0][j - 1]);
                default:
                    throw new ArgumentException("Incorrect mode");
            }
        }

        private void LineRender(Graphics g)
        {
            var font = new Font("Ubuntu", 16);
            var smallFont = new Font("Ubuntu", 12);

            var rightFormat = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center
            };

            var centerFormat = new StringFormat
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Near
            };

            var leftFormat = new StringFormat
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Center
            };

            var sizeKoef = (int)(Math.Min(_width/4, _height/3) * 0.85);
            var size = new Size(sizeKoef*4, sizeKoef*3);
            var center = new Point(50, _height - 30);
            Func<float, float, Point> getPoint = (deltaX, deltaY) =>
                center + new Size((int)(deltaX / 100.0f * size.Width), (int)(-deltaY / 100.0f * size.Height));
            Func<float, float, float, Rectangle> getRect = (deltaX, deltaY, raduis) =>
            {
                var point = center + new Size((int) (deltaX/100.0f*size.Width), (int) (-deltaY/100.0f*size.Height));
                return new Rectangle((int) (point.X - raduis), (int) (point.Y - raduis), (int) (2*raduis), (int) (2*raduis));
            };


            g.DrawLine(Pens.Black, getPoint(102, -2), getPoint(105, 0));
            g.DrawLine(Pens.Black, getPoint(102, 2), getPoint(105, 0));
            g.DrawLine(Pens.Black, getPoint(0, 0), getPoint(105, 0));
            g.DrawString("x", smallFont, Brushes.Black, getPoint(105, 4), rightFormat);

            g.DrawLine(Pens.Black, getPoint(0, 0), getPoint(0, 105));
            g.DrawLine(Pens.Black, getPoint(-2, 102), getPoint(0, 105));
            g.DrawLine(Pens.Black, getPoint(2, 102), getPoint(0, 105));
            g.DrawString("y", smallFont, Brushes.Black, getPoint(2, 107), rightFormat);

            var maxCount = _students.Select(a => a.Grades.Length).Max();

            for (var j = 0; j < _grades.Count; ++j)
            {
                g.DrawLine(new Pen(Color.Black, 2) {DashPattern = new[] {3.0f, 5.0f}},
                    getPoint(-1, _grades[j]), getPoint(100, _grades[j]));
                g.DrawString(_grades[j].ToString(), smallFont, Brushes.Black, getPoint(-1, _grades[j]), leftFormat);
            }

            for (var j = 0; j < maxCount; ++j)
            {
                g.DrawLine(new Pen(Color.Black, 2) { DashPattern = new[] { 3.0f, 5.0f } },
                    getPoint((float)(j+1) / maxCount * 100, -1), getPoint((float)(j+1) / maxCount * 100, 100));
                g.DrawString(_dates[j], smallFont, Brushes.Black, getPoint((float)(j + 1) / maxCount * 100 + 5, -1), centerFormat);
            }
            
            for (var i = 0; i < _students.Count; ++i)
            {
                var grades1 = new List<float> { 0.0f };
                grades1.AddRange(_students[i].Grades.Select(a => a[0]));

                for (var j = 0; j < grades1.Count - 1; ++j)
                {
                    g.DrawLine(new Pen(Color.Black, 4),
                        getPoint((float) j/maxCount*100, grades1[j]),
                        getPoint((float) (j + 1)/maxCount*100, grades1[j + 1]));
                    g.FillEllipse(Brushes.Black, getRect((float) (j + 1)/maxCount*100, grades1[j + 1], 5.0f));

                    if ((grades1.Count - 2) == j)
                    {
                        var ids = new List<int>();
                        for (var k = 0; k < _students.Count; ++k)
                        {
                            if (_students[k].Grades.Length == j + 1 &&
                                Math.Abs(_students[k].Grades.Last()[0] - grades1[j + 1]) < Parameters.Tolerance)
                                ids.Add(k+1);
                        }
                        g.DrawString(string.Join(",", ids), smallFont,
                            Brushes.Black, getPoint((float) (j + 1)/maxCount*100 + 1, grades1[j + 1]),
                            rightFormat);
                    }
                }
            }

            var half = (int)Math.Ceiling(_students.Count / 2.0);

            for (var i = 0; i < _students.Count; ++i)
            {
                var message = string.Format("{0}) {1} ({2})", i + 1, _students[i].Name, _students[i].Info);
                g.DrawString(message, font, Brushes.Black, getPoint(110, 100 - 100/(_students.Count-1) * i), rightFormat);
            }
        }
    }
}
