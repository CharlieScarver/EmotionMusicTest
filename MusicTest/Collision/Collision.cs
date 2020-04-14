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
        public static bool IntersectsWithPlatforms(ICollidable collidable)
        {
            for (int i = 0; i < GameContext.Scene.CollisionPlatforms.Count; i++)
            {
                // TODO: Rework
                CollisionPlatform platform = GameContext.Scene.CollisionPlatforms[i];
                if (collidable.CollisionRectangle.IntersectsInclusive(new Rectangle(platform.PointA, new Vector2((float)platform.Length, 1))))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
