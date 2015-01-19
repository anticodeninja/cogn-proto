namespace Simplex3D.Primitives
{
    using System;
    using System.Drawing;

    class Edge
    {
        public int[] P = new int[2];
        public Color C;

        public Edge(int[] p, Color color)
        {
            Array.Copy(p, P, p.Length);
            C = color;
        }
    }
}
