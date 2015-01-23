namespace Library.Geometry
{
    using System;

    public class Matrix2D : MatrixAbstract
    {
        private const int DIMENSION = 2;

        public Matrix2D()
            : base(DIMENSION)
        {
        }

        public Matrix2D ResizeMatrix(double[] sizes)
        {
            return ResizeMatrix<Matrix2D>(sizes);
        }

        public Matrix2D ResizeMatrix(double kx, double ky)
        {
            return ResizeMatrix<Matrix2D>(new []{kx, ky});
        }

        public Matrix2D MovementMatrix(double[] deltas)
        {
            return MovementMatrix<Matrix2D>(deltas);
        }

        public Matrix2D MovementMatrix(double dx, double dy)
        {
            return MovementMatrix<Matrix2D>(new[] { dx, dy });
        }

        public Matrix2D RotateMatrix(double angle)
        {
            ResetMatrix<Matrix2D>();
            _matrix[0, 0] = Math.Cos(angle);
            _matrix[0, 1] = Math.Sin(angle);
            _matrix[1, 0] = -Math.Sin(angle);
            _matrix[1, 1] = Math.Cos(angle);
            _matrix[2, 2] = 1;
            return this;
        }
    }
}
