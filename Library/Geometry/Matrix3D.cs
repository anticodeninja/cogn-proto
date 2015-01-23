namespace Library.Geometry
{
    using System;

    public class Matrix3D : MatrixAbstract
    {
        private const int DIMENSION = 3;

        public Matrix3D()
            : base(DIMENSION)
        {
        }

        public Matrix3D ResizeMatrix(double[] sizes)
        {
            return ResizeMatrix<Matrix3D>(sizes);
        }

        public Matrix3D ResizeMatrix(double kx, double ky, double kz)
        {
            return ResizeMatrix<Matrix3D>(new[] { kx, ky, kz });
        }

        public Matrix3D MovementMatrix(double[] deltas)
        {
            return MovementMatrix<Matrix3D>(deltas);
        }

        public Matrix3D MovementMatrix(double dx, double dy, double dz)
        {
            return MovementMatrix<Matrix3D>(new[] { dx, dy, dz });
        }

        public Matrix3D ProjectionMatrix()
        {
            ResetMatrix<Matrix3D>();

            _matrix[0, 0] = 1;
            _matrix[1, 1] = 1;
            _matrix[2, 2] = 1;
            _matrix[3, 3] = 1;
            _matrix[2, 3] = -0.0005f;

            return this;
        }

        public Matrix3D RotateMatrixX(double angle)
        {
            ResetMatrix<Matrix3D>();
            _matrix[0, 0] = 1;
            _matrix[1, 1] = Math.Cos(angle);
            _matrix[1, 2] = Math.Sin(angle);
            _matrix[2, 2] = Math.Cos(angle);
            _matrix[2, 1] = -Math.Sin(angle);
            _matrix[3, 3] = 1;
            return this;
        }

        public Matrix3D RotateMatrixY(double angle)
        {
            ResetMatrix<Matrix3D>();
            _matrix[0, 0] = Math.Cos(angle);
            _matrix[1, 1] = 1;
            _matrix[0, 2] = -Math.Sin(angle);
            _matrix[2, 2] = Math.Cos(angle);
            _matrix[2, 0] = Math.Sin(angle);
            _matrix[3, 3] = 1;
            return this;
        }

        public Matrix3D RotateMatrixZ(double angle)
        {
            ResetMatrix<Matrix3D>();
            _matrix[0, 0] = Math.Cos(angle);
            _matrix[1, 1] = Math.Cos(angle);
            _matrix[0, 1] = Math.Sin(angle);
            _matrix[2, 2] = 1;
            _matrix[1, 0] = -Math.Sin(angle);
            _matrix[3, 3] = 1;
            return this;
        }
    }
}
