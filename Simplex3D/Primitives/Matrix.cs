namespace Simplex3D.Primitives
{
    using System;

    public class Matrix
    {
        private readonly double[,] _matrix;

        public Matrix()
        {
            _matrix = new double[4, 4];
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
            for (var i = 0; i < 4; ++i)
                for (var j = 0; j < 4; ++j)
                    _matrix[i, j] = 0;
        }

        public Matrix IdentMatrix()
        {
            ResetMatrix();
            _matrix[0, 0] = 1;
            _matrix[1, 1] = 1;
            _matrix[2, 2] = 1;
            _matrix[3, 3] = 1;
            return this;
        }

        public Matrix ProjectionMatrix()
        {
            ResetMatrix();

            _matrix[0, 0] = 1;
            _matrix[1, 1] = 1;
            _matrix[2, 2] = 1;
            _matrix[3, 3] = 1;
            _matrix[2, 3] = -0.0005f;

            return this;
        }

        public Matrix RotateMatrixX(double angle)
        {
            ResetMatrix();
            _matrix[0, 0] = 1;
            _matrix[1, 1] = Math.Cos(angle);
            _matrix[1, 2] = Math.Sin(angle);
            _matrix[2, 2] = Math.Cos(angle);
            _matrix[2, 1] = -Math.Sin(angle);
            _matrix[3, 3] = 1;
            return this;
        }

        public Matrix RotateMatrixY(double angle)
        {
            ResetMatrix();
            _matrix[0, 0] = Math.Cos(angle);
            _matrix[1, 1] = 1;
            _matrix[0, 2] = -Math.Sin(angle);
            _matrix[2, 2] = Math.Cos(angle);
            _matrix[2, 0] = Math.Sin(angle);
            _matrix[3, 3] = 1;
            return this;
        }

        public Matrix RotateMatrixZ(double angle)
        {
            ResetMatrix();
            _matrix[0, 0] = Math.Cos(angle);
            _matrix[1, 1] = Math.Cos(angle);
            _matrix[0, 1] = Math.Sin(angle);
            _matrix[2, 2] = 1;
            _matrix[1, 0] = -Math.Sin(angle);
            _matrix[3, 3] = 1;
            return this;
        }

        public Matrix MovementMatrix(double dx, double dy, double dz)
        {
            ResetMatrix();
            _matrix[0, 0] = 1;
            _matrix[1, 1] = 1;
            _matrix[2, 2] = 1;
            _matrix[3, 3] = 1;
            _matrix[3, 0] = dx;
            _matrix[3, 1] = dy;
            _matrix[3, 2] = dz;
            return this;
        }

        public static void Multiply(Matrix m1, Matrix m2, Matrix output)
        {
            for (var i = 0; i < 4; ++i)
            {
                for (var j = 0; j < 4; ++j)
                {
                    double sum = 0;
                    for (var k = 0; k < 4; ++k)
                        sum += m1[i, k] * m2[k, j];
                    output[i, j] = sum;
                }
            }
        }
    }
}
