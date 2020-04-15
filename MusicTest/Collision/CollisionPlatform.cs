using Emotion.Graphics;
using Emotion.Primitives;
using System;
using System.Numerics;

namespace MusicTest.Core
{
    public class CollisionPlatform
    {
        public CollisionPlatform(Vector2 pointA, Vector2 pointB, bool isSloped = false)
        {
            PointA = pointA;
            PointB = pointB;
            IsSloped = isSloped;

            // Set the slope (this is m from the line equation y = mx + b) (needed for the angle)
            float divident = Math.Abs(PointB.Y - PointA.Y);
            float divisor = Math.Abs(PointB.X - PointA.X);

            if (divisor == 0)
            {
                throw new DivideByZeroException();
            }

            Slope = divident / divisor;

            // Set the incline angle
            InclineAngleWithX = (float) Math.Atan(Slope);

            // Set the Y axis intercept (this is b from the line equation y = mx + b)
            YIntercept = PointA.Y - (Slope * PointA.X);

            // Set the X axis intercept
            XIntercept = (0 - YIntercept) / Slope;

            // Set the length
            Length = (float) Math.Sqrt(Math.Pow(Math.Abs(PointB.X - PointA.X), 2) + Math.Pow(Math.Abs(PointB.Y - PointA.Y), 2));
        }

        public CollisionPlatform(float pointAX, float pointAY, float pointBX, float pointBY) : this(new Vector2(pointAX, pointAY), new Vector2(pointBX, pointBY))
        { }

        public Vector2 PointA { get; set; }

        public Vector2 PointB { get; set; }

        public bool IsSloped { get; set; }

        // This is m from the line equation y = mx + b
        public float Slope { get; set; }

        public float InclineAngleWithX { get; set; }

        // This is b from the line equation y = mx + b
        public float YIntercept { get; set; }

        public float XIntercept { get; set; }

        public float Length { get; set; }

        public void Render(RenderComposer composer)
        {
            composer.RenderLine(new Vector3(PointA, 6), new Vector3(PointB, 6), Color.Red, 1);
        }
    }
}
