namespace Library.Geometry
{
    public class Point3D : PointAbstract
    {
        private const int DIMENSION = 3;

        public Point3D()
            : base(DIMENSION)
        {
        }

        public Point3D(double x, double y, double z)
            : base(DIMENSION)
        {
            Set<Point3D>(new[] { x, y, z });
        }

        public Point3D(double[] coords)
            : base(DIMENSION)
        {
            Set<Point3D>(coords);
        }

        public double X { set { _coordinates[0] = value; } get { return _coordinates[0]; } }
        public double Y { set { _coordinates[1] = value; } get { return _coordinates[1]; } }
        public double Z { set { _coordinates[2] = value; } get { return _coordinates[2]; } }
    }
}