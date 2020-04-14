using Emotion.Audio;
using Emotion.IO;
using Emotion.Common;
using Emotion.Standard.Audio;
using System;
using System.Collections.Generic;
using System.Text;

namespace MusicTest
{
    class CustomAudioTrack : AudioTrack
    {
        public float PlaybackPoint { get; set; }

        public Action Callback { get; set; }

        private bool CallbackInvoked { get; set; }

        public CustomAudioTrack(AudioAsset file) : base(file)
        {
        }

        public CustomAudioTrack(AudioAsset file, float playbackPoint, Action callback) : base(file)
        {
            this.PlaybackPoint = playbackPoint;
            this.Callback = callback;
            this.CallbackInvoked = false;
        }

        public override int GetNextFrames(int frameCount, Span<byte> buffer)
        {
            if (!this.CallbackInvoked && this.PlaybackPoint != 0 && Math.Round(this.Playback, 2) >= this.PlaybackPoint)
            {
                this.Callback.Invoke();
                this.CallbackInvoked = true;
            }

            return base.GetNextFrames(frameCount, buffer);
        }
    }
}
