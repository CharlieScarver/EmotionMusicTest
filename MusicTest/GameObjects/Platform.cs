using Emotion.Graphics;
using Emotion.Primitives;
using MusicTest.Extensions;
using System.Numerics;

namespace MusicTest.GameObjects
{
    public class Platform : Decoration
    {
        public Platform(string type, string fileName, Vector2 size, Vector3 position, Line collisionLine) : base (type, fileName, size, position)
        {
            CollisionLine = collisionLine;
        }

        public Line CollisionLine { get; set; }

        public override void Render(RenderComposer composer)
        {
            base.Render(composer);

            composer.RenderLine(
                CollisionLine.AwithZ(Position.Z),
                CollisionLine.BwithZ(Position.Z),
                Color.Yellow
            ); 
        }
    }
}
