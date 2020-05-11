using Emotion.Primitives;
using MusicTest.Interactions;
using System.Collections.Generic;
using System.Numerics;

namespace MusicTest.GameObjects
{
    public class Shishi : Unit
    {
        const float _verticalVelocity = -40;
        const float _horizontalVelocity = 11;

        public Shishi(string name, string textureName, Vector3 position, Vector2 size) : base(name, textureName, position, size)
        {
            PortraitSize = new Vector2(674.181f, 822.543f); // Size(224.727f, 274.181f) * 3

            DialoguePiece d = new DialoguePiece(10, new List<string>() {
                "Magic be damned! A Dori!",
                "Ahem.. I mean.. Greetings, friend.",
                "What brings you to the slopes of Avash?"
            });

            List<DialoguePiece> dialogues = new List<DialoguePiece>();
            dialogues.Add(d);

            Dialogues = dialogues;

            IsFacingRight = true;
            InteractionOffset = new Vector2(-80, 250);

            //"X": 224.727,
            //"Y": 274.181
            CollisionBox = new Transform(
                X + 82,
                Y + 124,
                Z,
                60,
                150
            );

            VelocityX = _horizontalVelocity;
            StartingVelocityY = _verticalVelocity;
        }

        public override void ResetVelocities()
        {
            VelocityX = _horizontalVelocity;
            StartingVelocityY = _verticalVelocity;
        }

        protected override void SetCollisionBoxX(float x)
        {
            CollisionBox.X = x;
            X = x - 82;
        }

        protected override void SetCollisionBoxY(float y)
        {
            CollisionBox.Y = y;
            Y = y - 124;
        }
    }
}
