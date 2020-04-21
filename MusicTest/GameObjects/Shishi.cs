﻿using Emotion.Common;
using Emotion.Primitives;
using System.Collections.Generic;
using System.Numerics;

namespace MusicTest.GameObjects
{
    public class Shishi : Unit
    {
        public Shishi(string name, string textureName, Vector3 position, Vector2 size) : base(name, textureName, position, size)
        {
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

            CollisionBox = new Transform(
                X + 50,
                Y,
                Z,
                Width / 2,
                Height
            );
        }

        protected override void SetCollisionBoxX(float x)
        {
            CollisionBox.X = x;
            X = x - 50;
        }

        protected override void SetCollisionBoxY(float y)
        {
            CollisionBox.Y = y;
            Y = y;
        }
    }
}
