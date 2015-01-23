namespace Library.Geometry
{
    public class MatrixAbstract
    {
        protected readonly double[,] _matrix;

        public int Dimension { get; private set; }

        public MatrixAbstract(int dimension)
        {
            Dimension = dimension;
            _matrix = new double[Dimension + 1, Dimension + 1];
            ResetMatrix<MatrixAbstract>();
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

        public T ResetMatrix<T>() where T : MatrixAbstract
        {
            for (var i = 0; i <= Dimension; ++i)
            {
                for (var j = 0; j <= Dimension; ++j)
                {
                    _matrix[i, j] = 0.0;
                }
            }
            return (T) this;
        }

        public T IdentMatrix<T>() where T : MatrixAbstract
        {
            ResetMatrix<T>();
            for (var i = 0; i <= Dimension; ++i)
            {
                _matrix[i, i] = 1.0;
            }
            return (T)this;
        }

        public T ResizeMatrix<T>(double[] sizes) where T : MatrixAbstract
        {
            ResetMatrix<T>();
            for (var i = 0; i < Dimension; ++i)
            {
                _matrix[i, i] = sizes[i];
            }
            _matrix[Dimension, Dimension] = 1.0;
            return (T)this;
        }

        public T MovementMatrix<T>(double[] deltas) where T : MatrixAbstract
        {
            ResetMatrix<T>();
            for (var i = 0; i <= Dimension; ++i)
            {
                _matrix[i, i] = 1.0;
            }
            for (var i = 0; i < Dimension; ++i)
            {
                _matrix[Dimension, i] = deltas[i];
            }
            return (T)this;
        }

        public T Chain<T>(params T[] matrices) where T : MatrixAbstract, new()
        {
            var temp = new T().IdentMatrix<T>();

            for (var i = 0; i < matrices.Length; ++i)
            {
                var toEnd = matrices.Length - i - 1;
                var one = toEnd%2 == 0 ? temp : this;
                var two = toEnd%2 == 0 ? this : temp;

                Multiply(one, matrices[i], two);
            }

            return (T) this;
        }

        public static void Multiply<T>(T m1, T m2, T output) where T : MatrixAbstract
        {
            for (var i = 0; i <= m1.Dimension; ++i)
            {
                for (var j = 0; j <= m1.Dimension; ++j)
                {
                    double sum = 0;
                    for (var k = 0; k <= m1.Dimension; ++k)
                        sum += m1[i, k] * m2[k, j];
                    output[i, j] = sum;
                }
            }
        }


    }
}
