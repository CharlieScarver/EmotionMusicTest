#region Using

using System;
using System.Numerics;

#endregion

namespace MusicTest.Extensions
{
    public struct Line
    {
        public float Ax;
        public float Ay;
        public float Bx;
        public float By;

        public Line(float ax, float ay, float bx, float by)
        {
            Ax = ax;
            Ay = ay;
            Bx = bx;
            By = by;
        }

        public Line(Vector2 a, Vector2 b) : this(a.X, a.Y, b.X, b.Y)
        {
        }

        public float Length
        {
            get => (float)Math.Sqrt(Math.Pow(Ax - Bx, 2) + Math.Pow(Ay - By, 2));
        }

        public Vector2 A 
        {
            get => new Vector2(Ax, Ay);
            set 
            {
                Ax = value.X;
                Ay = value.Y;
            }
        }

        public Vector2 B
        {
            get => new Vector2(Bx, By);
            set
            {
                Bx = value.X;
                By = value.Y;
            }
        }

        public Vector3 AwithZ(float z) 
        {
            return new Vector3(Ax, Ay, z);
        }

        public Vector3 BwithZ(float z)
        {
            return new Vector3(Bx, By, z);
        }

        public static bool intersectsAnotherLine(Line l1, Line l2)
        {
            if (l1.Ax > l2.Ax)
            {
            }
            else if (l1.Ay > l2.Ay)
            {
            }

            throw new NotImplementedException();
            //x1 < x < x2, assuming x1<x2, or
            //y1 < y < y2, assuming y1<y2, or
            //z1 < z < z2, assuming z1<z2
        }

        public override string ToString()
        {
            return $"Line - A: ({Ax}, {Ay}), B: ({Bx}, {By}), Length: {Length}";
        }
    }
}