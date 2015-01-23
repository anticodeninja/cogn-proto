using System;
using System.Drawing;

namespace Library.Graphics
{
    public static class PointDrawer
    {
        public static void DrawPoint(System.Drawing.Graphics g, PointType type, Color c, float w, float x, float y)
        {
            w = w/2;

            switch (type)
            {
                case PointType.TriangleUp:
                    var triangleUp = new[]
                            {
                                new PointF(x - w, y + w),
                                new PointF(x + w, y + w),
                                new PointF(x, y - w)
                            };
                    g.FillPolygon(new SolidBrush(c), triangleUp);
                    g.DrawPolygon(new Pen(Color.Black, 1), triangleUp);
                    break;

                case PointType.TriangleDown:
                    var triangleDown = new[]
                            {
                                new PointF(x - w, y - w),
                                new PointF(x + w, y - w),
                                new PointF(x, y + w)
                            };
                    g.FillPolygon(new SolidBrush(c), triangleDown);
                    g.DrawPolygon(new Pen(Color.Black, 1), triangleDown);
                    break;

                case PointType.Circle:
                    g.FillEllipse(new SolidBrush(c), x - w, y - w, 2 * w, 2 * w);
                    g.DrawEllipse(new Pen(Color.Black, 1), x - w, y - w, 2 * w, 2 * w);
                    break;
                case PointType.Square:
                    g.FillRectangle(new SolidBrush(c), x - w, y - w, 2 * w, 2 * w);
                    g.DrawRectangle(new Pen(Color.Black, 1), x - w, y - w, 2 * w, 2 * w);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
