using Emotion.Primitives;
using MusicTest.GameObjects;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MusicTest.Core.Collision
{
    public static class Collision
    {
        public static bool IntersectsWithPlatforms(Rectangle rect)
        {
            for (int i = 0; i < GameContext.Scene.CollisionPlatforms.Count; i++)
            {
                CollisionPlatform platform = GameContext.Scene.CollisionPlatforms[i];
                if (LineIntesectsRectangle(platform.PointA, platform.PointB, rect))
                {
                    return true;
                }
            }

            return false;
        }

        public static CollisionPlatform? IntersectsWithSlopedPlatforms(Rectangle rect)
        {
            for (int i = 0; i < GameContext.Scene.SlopedCollisionPlatforms.Count; i++)
            {
                CollisionPlatform platform = GameContext.Scene.SlopedCollisionPlatforms[i];
                if (LineIntesectsRectangle(platform.PointA, platform.PointB, rect))
                {
                    return platform;
                }
            }

            return null;
        }


        // Determines if the lines AB and CD intersect.
        static bool LinesIntersect(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
        {
            Vector2 CmP = new Vector2(C.X - A.X, C.Y - A.Y);
            Vector2 r = new Vector2(B.X - A.X, B.Y - A.Y);
            Vector2 s = new Vector2(D.X - C.X, D.Y - C.Y);

            float CmPxr = CmP.X * r.Y - CmP.Y * r.X;
            float CmPxs = CmP.X * s.Y - CmP.Y * s.X;
            float rxs = r.X * s.Y - r.Y * s.X;

            if (CmPxr == 0f)
            {
                // Lines are collinear, and so intersect if they have any overlap

                return ((C.X - A.X < 0f) != (C.X - B.X < 0f))
                    || ((C.Y - A.Y < 0f) != (C.Y - B.Y < 0f));
            }

            if (rxs == 0f)
                return false; // Lines are parallel.

            float rxsr = 1f / rxs;
            float t = CmPxs * rxsr;
            float u = CmPxr * rxsr;

            return (t >= 0f) && (t <= 1f) && (u >= 0f) && (u <= 1f);
        }

        static bool LineIntesectsRectangle(Vector2 A, Vector2 B, Rectangle rect)
        {
            return LinesIntersect(A, B, rect.BottomLeft, rect.BottomRight) ||
                LinesIntersect(A, B, rect.TopLeft, rect.TopRight) ||
                LinesIntersect(A, B, rect.TopLeft, rect.BottomLeft) ||
                LinesIntersect(A, B, rect.TopRight, rect.BottomRight);
        }
    }
}
