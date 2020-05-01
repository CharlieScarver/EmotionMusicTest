using Emotion.Primitives;
using System.Numerics;

namespace MusicTest.Core.Collision
{
    public static class Collision
    {
        #region CollisionWithPlatforms
        public static CollisionPlatform IntersectsWithPlatforms(Rectangle rect)
        {
            for (int i = 0; i < GameContext.Scene.CollisionPlatforms.Count; i++)
            {
                CollisionPlatform platform = GameContext.Scene.CollisionPlatforms[i];
                if (LineSegmentIntesectsRectangle(platform.PointA, platform.PointB, rect))
                {
                    return platform;
                }
            }

            return null;
        }
        public static CollisionPlatform IntersectsWithAxisAlignedPlatforms(Rectangle rect)
        {
            for (int i = 0; i < GameContext.Scene.AxisAlignedCollisionPlatforms.Count; i++)
            {
                CollisionPlatform platform = GameContext.Scene.AxisAlignedCollisionPlatforms[i];
                if (LineSegmentIntesectsRectangle(platform.PointA, platform.PointB, rect))
                {
                    return platform;
                }
            }

            return null;
        }

        public static CollisionPlatform? IntersectsWithSlopedPlatforms(Rectangle rect)
        {
            for (int i = 0; i < GameContext.Scene.SlopedCollisionPlatforms.Count; i++)
            {
                CollisionPlatform platform = GameContext.Scene.SlopedCollisionPlatforms[i];
                if (LineSegmentIntesectsRectangle(platform.PointA, platform.PointB, rect))
                {
                    return platform;
                }
            }

            return null;
        }

        #endregion

        #region Rectangle Intersection
        /// <summary>
        /// Checks if a line segment intersects a rectangle
        /// </summary>
        static bool LineSegmentIntesectsRectangle(Vector2 A, Vector2 B, Rectangle rect)
        {
            Vector2 bottomLeft = new Vector2(rect.X, rect.Bottom);
            return LineSegmentsIntersect3(A, B, bottomLeft, rect.BottomRight) ||
                LineSegmentsIntersect3(A, B, rect.TopLeft, rect.TopRight) ||
                LineSegmentsIntersect3(A, B, rect.TopLeft, bottomLeft) ||
                LineSegmentsIntersect3(A, B, rect.TopRight, rect.BottomRight);
        }

        /// <summary>
        /// Checks if an Emotion.Ray2D intersects a Rectangle using Emotion's RayIntersects()
        /// </summary>
        //static bool RayIntesectsRectangle(Vector2 A, Vector2 B, Rectangle rect)
        //{
        //    Ray2D ab = new Ray2D(A, B);
        //    float distance;
        //    // Uses ref to make it faster as the value is not copied ?
        //    return rect.RayIntersects(ref rect, ref ab, out distance);
        //}

        #endregion

        #region Helpers

        /// <summary>
        /// Calculate the vector cross product of two vectors.
        /// </summary>
        public static float VectorCrossProduct(Vector2 vec1, Vector2 vec2)
        {
            return (vec1.X * vec2.Y) - (vec1.Y * vec2.X);
        }

        /// <summary>
        /// Calculate the vector dot product of two vectors.
        /// </summary>
        public static float VectorDotProduct(Vector2 vec1, Vector2 vec2)
        {
            return (vec1.X * vec2.X) + (vec1.Y * vec2.Y);
        }

        /// <summary>
        /// Subtract the second point from the first.
        /// </summary>
        public static Vector2 SubtractPoints(Vector2 vec1, Vector2 vec2)
        {
            return new Vector2(vec1.X - vec2.X, vec1.Y - vec2.Y);
        }

        #endregion

        #region Line Segments Intersection

        /// <summary>
        /// Determines if the lines AB and CD intersect.
        /// 
        /// Original Author: Matt, commented on Dec 17 '12 at 0:42
        /// https://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect
        /// https://ideone.com/PnPJgb
        /// <summary>
        public static bool LineSegmentsIntersect1(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
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


        /// <summary>
        /// See if two line segments intersect. (p-p2 and q-q2)
        /// This uses the vector cross product approach described below: 
        /// http://stackoverflow.com/a/565282/786339
        /// 
        /// Original Author: Peter Kelley (pgkelley, commented on Aug 29 '13 at 4:10)
        /// https://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect
        /// https://github.com/pgkelley4/line-segments-intersect/blob/master/js/line-segments-intersect.js
        /// </summary>
        public static bool LineSegmentsIntersect2(Vector2 p, Vector2 p2, Vector2 q, Vector2 q2)
        {
            Vector2 r = SubtractPoints(p2, p);
            Vector2 s = SubtractPoints(q2, q);

            double uNumerator = VectorCrossProduct(SubtractPoints(q, p), r);
            double denominator = VectorCrossProduct(r, s);

            if (uNumerator == 0 && denominator == 0)
            {
                // The segments are collinear

                // Do they touch? (Are any of the points equal?)
                if (p == q || p == q2 || p2 == q || p2 == q2)
                {
                    return true;
                }

                // Do they overlap? (Are all the point differences in )
                return !((q.X - p.X < 0) && (q.X - p2.X < 0) && (q2.X - p.X < 0) && (q2.X - p2.X < 0)) ||
                     !((q.Y - p.Y < 0) && (q.Y - p2.Y < 0) && (q2.Y - p.Y < 0) && (q2.Y - p2.Y < 0));

            }

            if (denominator == 0)
            {
                // Lines are parallel
                return false;
            }

            double u = uNumerator / denominator;
            double t = VectorCrossProduct(SubtractPoints(q, p), s) / denominator;

            return (t >= 0) && (t <= 1) && (u >= 0) && (u <= 1);
        }


        /// <summary>
        /// Returns if the line segments p-p2 and q-q2 intersect or not.
        ///
        /// Original Author: Gavin, answered Dec 28 '09 at 7:16
        /// https://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect
        /// 
        /// Supposedly at least twice as fast as any of the other posted algorithms.
        /// </summary>
        public static bool LineSegmentsIntersect3(Vector2 p, Vector2 p2, Vector2 q, Vector2 q2)
        {
            double s1_x, s1_y, s2_x, s2_y;
            s1_x = p2.X - p.X;
            s1_y = p2.Y - p.Y;
            s2_x = q2.X - q.X;
            s2_y = q2.Y - q.Y;

            double s, t;
            s = (-s1_y * (p.X - q.X) + s1_x * (p.Y - q.Y)) / (-s2_x * s1_y + s1_x * s2_y);
            t = (s2_x * (p.Y - q.Y) - s2_y * (p.X - q.X)) / (-s2_x * s1_y + s1_x * s2_y);

            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                // Collision detected
                return true;
            }

            return false; // No collision
        }

        #endregion

        #region Points

        /// <summary>
        /// Checks if a point is on a line segment.
        /// </summary>
        /// <param name="point">The point</param>
        /// <param name="segmPoint1">The first point of the line segment</param>
        /// <param name="segmPoint2">The second point of the line segment</param>
        /// <returns>Returns true if the point is on the line segment, otherwise false.</returns>
        public static bool PointIsOnLineSegment(Vector2 point, Vector2 segmPoint1, Vector2 segmPoint2)
        {
            return (point.X >= segmPoint1.X && point.X <= segmPoint2.X &&
                point.Y >= segmPoint1.Y && point.Y <= segmPoint2.Y) ||
                (point.X >= segmPoint2.X && point.X <= segmPoint1.X &&
                point.Y >= segmPoint2.Y && point.Y <= segmPoint1.Y);
        }

        public static bool PointIsInRectangleInclusive(Vector2 point, Rectangle rect)
        {
            return (point.X >= rect.Left && point.X <= rect.Right) && (point.Y >= rect.Top && point.Y <= rect.Bottom);
        }

        #endregion
    }
}
