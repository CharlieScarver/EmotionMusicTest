using Emotion.Graphics;
using Emotion.Primitives;
using System;
using System.Numerics;

namespace MusicTest.Core
{
    public class CollisionPlatform
    {
        public CollisionPlatform(Vector2 pointA, Vector2 pointB, bool isStraight = true)
        {
            PointA = pointA;
            PointB = pointB;

            // Set the slope (this is m from the line equation y = mx + b) (needed for the angle)
            double divident = Math.Abs(PointB.Y - PointA.Y);
            double divisor = Math.Abs(PointB.X - PointA.X);

            if (divisor == 0)
            {
                throw new DivideByZeroException();
            }

            Slope = divident / divisor;

            // Set the incline angle
            InclineAngleWithX = Math.Atan(Slope);

            // Set the Y axis intercept (this is b from the line equation y = mx + b)
            YIntercept = PointA.Y - (Slope * PointA.X);

            // Set the X axis intercept
            XIntercept = (0 - YIntercept) / Slope;

            // Set the length
            Length = Math.Sqrt(Math.Pow(Math.Abs(PointB.X - PointA.X), 2) + Math.Pow(Math.Abs(PointB.Y - PointA.Y), 2));
        }

        public CollisionPlatform(float pointAX, float pointAY, float pointBX, float pointBY) : this(new Vector2(pointAX, pointAY), new Vector2(pointBX, pointBY))
        { }

        public Vector2 PointA { get; set; }

        public Vector2 PointB { get; set; }

        // This is m from the line equation y = mx + b
        public double Slope { get; set; }

        public double InclineAngleWithX { get; set; }

        // This is b from the line equation y = mx + b
        public double YIntercept { get; set; }

        public double XIntercept { get; set; }

        public double Length { get; set; }

        public void Render(RenderComposer composer)
        {
            composer.RenderLine(new Vector3(PointA, 6), new Vector3(PointB, 6), Color.Red, 1);
        }
    }
}
