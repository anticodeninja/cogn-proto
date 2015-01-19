namespace Simplex3D.Primitives
{
    public class Point
    {
        private readonly double[] _coordinates;

        public Point()
        {
            _coordinates = new double[4];
            for (var i = 0; i < 3; ++i)
                _coordinates[i] = 0;
            _coordinates[3] = 1;
        }

        public Point(double x, double y, double z)
        {
            _coordinates = new double[4];
            _coordinates[0] = x;
            _coordinates[1] = y;
            _coordinates[2] = z;
            _coordinates[3] = 1;
        }

        public double X
        {
            set
            {
                _coordinates[0] = value;
            }
            get
            {
                return _coordinates[0];
            }
        }

        public double Y
        {
            set
            {
                _coordinates[1] = value;
            }
            get
            {
                return _coordinates[1];
            }
        }

        public double Z
        {
            set
            {
                _coordinates[2] = value;
            }
            get
            {
                return _coordinates[2];
            }
        }

        public void Transform(Matrix matrix, Point output)
        {
            for (var j = 0; j < 4; ++j)
            {
                double sum = 0;
                for (var k = 0; k < 4; ++k)
                    sum += _coordinates[k] * matrix[k, j];
                output._coordinates[j] = sum;
            }
            for (var i = 0; i < 4; ++i)
            {
                output._coordinates[i] /= output._coordinates[3];
            }
        }
    }
}
