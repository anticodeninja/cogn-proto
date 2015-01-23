namespace Library.Geometry
{
    using System;
    using System.Drawing;

    class Edge
    {
        public int[] P { get; set; }
        public Color C { get; set; }

        public Edge(int[] p, Color color)
        {
            P = new int[2];
            Array.Copy(p, P, p.Length);
            C = color;
        }
    }
}
