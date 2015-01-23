namespace Library.Geometry
{
    public class Point2D : PointAbstract
    {
        private const int DIMENSION = 2;

        public Point2D()
            : base(DIMENSION)
        {
        }

        public Point2D(double x, double y)
            : base(DIMENSION)
        {
            Set<Point2D>(new[] {x, y});
        }

        public Point2D(double[] coords)
            : base(DIMENSION)
        {
            Set<Point2D>(coords);
        }

        public double X { set { _coordinates[0] = value; } get { return _coordinates[0]; } }
        public double Y { set { _coordinates[1] = value; } get { return _coordinates[1]; } }
    }
}