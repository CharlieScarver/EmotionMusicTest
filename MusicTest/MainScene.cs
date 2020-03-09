using Emotion.Audio;
using Emotion.Common;
using Emotion.Graphics;
using Emotion.IO;
using Emotion.Scenography;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTest
{
    class MainScene : IScene
    {
        public byte CodeVariant { get; set; }
        public AudioLayer IntroLayer { get; set; }
        public AudioLayer MainLayer { get; set; }
        public AudioLayer SecondaryLayer { get; set; }

        public AudioAsset BackgroundMusic { get; set; }
        public AudioAsset BackgroundMusicIntro { get; set; }

        public void PlayMainTrackOnMainLayer()
        {
            CustomAudioTrack fcMain = new CustomAudioTrack(BackgroundMusic, 90.35f, PlayMainTrackLoopOnIntroLayer);
            MainLayer.AddToQueue(fcMain);
        }

        public void PlayMainTrackLoopOnIntroLayer()
        {
            CustomAudioTrack fcMain = new CustomAudioTrack(BackgroundMusic, 90.35f, PlayMainTrackOnMainLayer);
            IntroLayer.AddToQueue(fcMain);
        }

        public void Draw(RenderComposer composer)
        {
            
        }

        public void Load()
        {
            CodeVariant = 2;

            if (CodeVariant == 1)
            {
                BackgroundMusicIntro = Engine.AssetLoader.Get<AudioAsset>("Frozen Cave Intro.wav");
                BackgroundMusic = Engine.AssetLoader.Get<AudioAsset>("Frozen Cave 12.wav");

                IntroLayer = Engine.Host.Audio.CreateLayer("Intro layer", 1);
                CustomAudioTrack fcIntro = new CustomAudioTrack(BackgroundMusicIntro, 11.29f, PlayMainTrackOnMainLayer);
                IntroLayer.AddToQueue(fcIntro);

                MainLayer = Engine.Host.Audio.CreateLayer("Main layer", 1);
            }
            else if (CodeVariant == 2)
            {
                BackgroundMusicIntro = Engine.AssetLoader.Get<AudioAsset>("Frozen Cave Intro 2.wav");
                BackgroundMusic = Engine.AssetLoader.Get<AudioAsset>("Frozen Cave 12.wav");
                
                SecondaryLayer = Engine.Host.Audio.CreateLayer("Secondary layer", 1);
                SecondaryLayer.AddToQueue(BackgroundMusicIntro);
                SecondaryLayer.AddToQueue(BackgroundMusic);
                SecondaryLayer.AddToQueue(BackgroundMusic);
            }
        }

        public void Unload()
        {
        }

        public void Update()
        {
            if (CodeVariant == 2)
            {
                if (SecondaryLayer.CurrentTrack.File.Name == "Frozen Cave 12.wav")
                {
                    SecondaryLayer.LoopingCurrent = true;
                }
            }
        }
    }
}
