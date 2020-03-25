using Emotion.Common;
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
        }
    }
}
