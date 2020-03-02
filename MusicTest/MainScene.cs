using Emotion.Audio;
using Emotion.Common;
using Emotion.Graphics;
using Emotion.IO;
using Emotion.Scenography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTest
{
    class MainScene : IScene
    {
        public AudioLayer introLayer { get; set; }
        public AudioLayer mainLayer { get; set; }

        public AudioTrack BackgroundMusic { get; set; }
        public AudioTrack BackgroundMusicIntro { get; set; }

        public void Draw(RenderComposer composer)
        {
            
        }

        public void Load()
        {
            BackgroundMusicIntro = new AudioTrack(Engine.AssetLoader.Get<AudioAsset>("fc-intro.wav"));
            BackgroundMusic = new AudioTrack(Engine.AssetLoader.Get<AudioAsset>("fc.wav"));

            introLayer = Engine.Host.Audio.CreateLayer("Intro layer", 1);
            introLayer.PlayNext(BackgroundMusicIntro);

            mainLayer = Engine.Host.Audio.CreateLayer("Main layer", 1);
        }

        public void Unload()
        {
            Engine.AssetLoader.Destroy("fc-intro.wav");
        }

        public void Update()
        {
            if (Math.Round(BackgroundMusicIntro.Playback, 2) >= 11.29 && Math.Round(BackgroundMusicIntro.Playback, 2) <= 11.3)
            {
                mainLayer.PlayNext(BackgroundMusic);
                mainLayer.LoopingCurrent = true;
            }
        }
    }
}
