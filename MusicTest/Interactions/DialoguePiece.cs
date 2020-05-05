using System.Collections.Generic;

namespace MusicTest.Interactions
{
    public class DialoguePiece
    {
        /// <summary>
        /// The minimal Progress Number required for this dialogue piece to be said.
        /// </summary>
        public int RequiredProgress { get; set; }

        /// <summary>
        /// The separate dialogue lines. The player will advance manually from one to the next.
        /// </summary>
        public List<string> DialogueLines { get; set; }

        public DialoguePiece(int requiredProgress, List<string> dialogueLines)
        {
            RequiredProgress = requiredProgress;
            DialogueLines = dialogueLines;
        }

    }
}
