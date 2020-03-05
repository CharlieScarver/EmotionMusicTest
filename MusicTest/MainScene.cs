﻿using Emotion.Audio;
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
        public AudioLayer secondaryLayer { get; set; }

        public AudioTrack BackgroundMusic { get; set; }
        public AudioTrack BackgroundMusicIntro { get; set; }

        public void Draw(RenderComposer composer)
        {
            
        }

        public void Load()
        {
            BackgroundMusicIntro = new AudioTrack(Engine.AssetLoader.Get<AudioAsset>("fc-intro2.wav"));
            BackgroundMusic = new AudioTrack(Engine.AssetLoader.Get<AudioAsset>("fc2.wav"));


            secondaryLayer = Engine.Host.Audio.CreateLayer("Secondary layer", 1);
            secondaryLayer.PlayNext(BackgroundMusicIntro);
            secondaryLayer.PlayNext(BackgroundMusic);
            secondaryLayer.PlayNext(BackgroundMusic);

            introLayer = Engine.Host.Audio.CreateLayer("Intro layer", 1);
            //introLayer.PlayNext(BackgroundMusicIntro);
            //introLayer.PlayNext(BackgroundMusic);

            mainLayer = Engine.Host.Audio.CreateLayer("Main layer", 1);
        }

        public void Unload()
        {
            //Engine.AssetLoader.Destroy("fc-intro.wav");
            //Engine.AssetLoader.Destroy("fc.wav");
        }

        public void Update()
        {
            //if (Math.Round(BackgroundMusicIntro.Playback, 2) >= 11.29 && Math.Round(BackgroundMusicIntro.Playback, 2) <= 11.3)
            //{
            //    mainLayer.PlayNext(BackgroundMusic);
            //}
            //else if (Math.Round(BackgroundMusic.Playback, 2) >= 90.35 && Math.Round(BackgroundMusic.Playback, 2) <= 90.36 && introLayer.Status == PlaybackStatus.NotPlaying)
            //{
            //    introLayer.PlayNext(BackgroundMusic);
            //}
            //else if (Math.Round(BackgroundMusic.Playback, 2) >= 90.35 && Math.Round(BackgroundMusic.Playback, 2) < 90.36 && mainLayer.Status == PlaybackStatus.NotPlaying)
            //{
            //    mainLayer.PlayNext(BackgroundMusic);
            //}

            //if (introLayer.CurrentTrack.File.Name == "fc.wav")
            //{
            //    introLayer.LoopingCurrent = true;
            //}

            if (Math.Round(BackgroundMusic.Playback, 2) >= 0.0 && Math.Round(BackgroundMusicIntro.Playback, 2) <= 0.3)
            {
                secondaryLayer.Volume = 0.2f;
            }
            else if (Math.Round(BackgroundMusic.Playback, 2) >= 1.0 && Math.Round(BackgroundMusicIntro.Playback, 2) <= 1.1)
            {
                secondaryLayer.Volume = 1;
            }
        }
    }
}
