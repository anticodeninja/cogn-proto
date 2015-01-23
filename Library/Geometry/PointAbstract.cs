namespace Library.Geometry
{
    using System.Linq;

    public abstract class PointAbstract
    {
        protected readonly double[] _coordinates;

        public int Dimension { get; private set; }

        protected PointAbstract(int dimension)
        {
            Dimension = dimension;
            _coordinates = new double[Dimension + 1];
            ResetPoint<PointAbstract>();
        }

        public double this[int i]
        {
            get
            {
                return _coordinates[i];
            }
            set
            {
                _coordinates[i] = value;
            }
        }

        public T Set<T>(double[] coords) where T : PointAbstract
        {
            for (var i = 0; i < Dimension; ++i)
            {
                _coordinates[i] = coords[i];
            }
            return (T) this;
        }

        protected T ResetPoint<T>() where T : PointAbstract
        {
            for (var i = 0; i < Dimension; ++i)
            {
                _coordinates[i] = 0;
            }
            _coordinates[Dimension] = 1;
            return (T) this;
        }

        public void Transform(MatrixAbstract matrix, PointAbstract output)
        {
            for (var j = 0; j <= Dimension; ++j)
            {
                output._coordinates[j] = _coordinates.Select((t, k) => t*matrix[k, j]).Sum();
            }
            for (var i = 0; i <= Dimension; ++i)
            {
                output._coordinates[i] /= output._coordinates[Dimension];
            }
        }
    }
}