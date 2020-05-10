using Emotion.Graphics;
using MusicTest.Collision;
using System.Numerics;

namespace MusicTest.GameObjects
{
    public class Platform : Decoration
    {
        public Platform(string name, string textureName, Vector2 size, Vector3 position, LineSegment collisionLine) : base (name, textureName, size, position)
        {
            CollisionLine = collisionLine;
        }

        public LineSegment CollisionLine { get; set; }

        public override void Render(RenderComposer composer)
        {
            base.Render(composer);

            //composer.RenderLine(
            //    CollisionLine.AwithZ(Position.Z),
            //    CollisionLine.BwithZ(Position.Z),
            //    Color.Yellow
            //); 
        }
    }
}
