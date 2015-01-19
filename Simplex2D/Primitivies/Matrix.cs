namespace Simplex2D.Primitivies
{
    using System;

    public class Matrix
    {
        private readonly double[,] _matrix;

        public Matrix()
        {
            _matrix = new double[3, 3];
            ResetMatrix();
        }

        public double this[int j, int i]
        {
            get
            {
                return _matrix[j, i];
            }
            set
            {
                _matrix[j, i] = value;
            }
        }

        public void ResetMatrix()
        {
            for (var i = 0; i < 3; ++i)
                for (var j = 0; j < 3; ++j)
                    _matrix[i, j] = 0;
        }

        public Matrix IdentMatrix()
        {
            ResetMatrix();
            _matrix[0, 0] = 1;
            _matrix[1, 1] = 1;
            _matrix[2, 2] = 1;
            return this;
        }

        public Matrix ResizeMatrix(double kx, double ky) {
            ResetMatrix();
            _matrix[0, 0] = kx;
            _matrix[1, 1] = ky;
            _matrix[2, 2] = 1;
            return this;
        }

        public Matrix RotateMatrix(double angle)
        {
            ResetMatrix();
            _matrix[0, 0] = Math.Cos(angle);
            _matrix[0, 1] = Math.Sin(angle);
            _matrix[1, 0] = -Math.Sin(angle);
            _matrix[1, 1] = Math.Cos(angle);
            _matrix[2, 2] = 1;
            return this;
        }

        public Matrix MovementMatrix(double dx, double dy)
        {
            ResetMatrix();
            _matrix[0, 0] = 1;
            _matrix[1, 1] = 1;
            _matrix[2, 2] = 1;
            _matrix[2, 0] = dx;
            _matrix[2, 1] = dy;
            return this;
        }

        public static void Multiply(Matrix m1, Matrix m2, Matrix output)
        {
            for (var i = 0; i < 3; ++i)
            {
                for (var j = 0; j < 3; ++j)
                {
                    double sum = 0;
                    for (var k = 0; k < 3; ++k)
                        sum += m1[i, k] * m2[k, j];
                    output[i, j] = sum;
                }
            }
        }
    }
}
