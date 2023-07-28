using System;
using System.Collections.Generic;
using System.Text;

namespace Extra
{
    class Point
    {
        public float x, y;

        public Point()
        {
            x = 0.0f;
            y = 0.0f;
        }

        public Point(float input_x, float input_y)
        {
            x = input_x;
            y = input_y;
        }

        public void Add(Vector2D displacement)
        {
            x += displacement.x;
            y += displacement.y;
        }

        public Point Moved(Vector2D displacement)
        {
            return new Point(x + displacement.x, y + displacement.y);
        }

        // Operator overload.
        public static Point operator +(Point a) => a;

        public static Point operator -(Point a)
            => new Point(-a.x, -a.y);

        public static Point operator +(Point a, Vector2D displacement)
            => new Point(a.x + displacement.x, a.y + displacement.y);

        public static Point operator +(Vector2D displacement, Point a)
            => a + displacement;
     
        public static Point operator -(Point a, Vector2D displacement)
            => new Point(a.x - displacement.x, a.y - displacement.y);

        public static Point operator -(Vector2D displacement, Point a)
            => a - displacement;

        public static Vector2D operator -(Point a, Point b)
            => new Vector2D(a.x - b.x, a.y - b.y);
        //

        public void Print()
        {
            Console.WriteLine("Coordenadas do ponto: x = " + x + " y = " + y);
        }

        public Point ToSvgCoordinates(Point origin)
        {
            return new Point(x - origin.x, origin.y - y);
        }
        
        public Point Middle(Point to)
        {
            return new Point((x + to.x) / 2, (y + to.y) / 2);
        }

    }
}
