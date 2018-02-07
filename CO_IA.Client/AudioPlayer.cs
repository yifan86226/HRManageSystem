using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CO_IA.Client
{
    public class AudioPlayer
    {
        private AudioPlayer()
        {
        }
        private System.Windows.Media.MediaPlayer mediaPlayer = new System.Windows.Media.MediaPlayer();
        private static AudioPlayer CurrentAudioPlayer = new AudioPlayer();

        static AudioPlayer()
        {
            CurrentAudioPlayer.mediaPlayer.MediaEnded += mediaPlayer_MediaEnded;
        }

        static void mediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            Stop();
        }
        private Action OnStopPlaying;
        private Window playingWnd;

        public static void Play(UIElement originUI, Uri uriFile, Action OnStopPlay = null)
        {
            Stop();
            CurrentAudioPlayer.mediaPlayer.Open(uriFile);
            CurrentAudioPlayer.OnStopPlaying = OnStopPlay;
            CurrentAudioPlayer.mediaPlayer.Play();
            CurrentAudioPlayer.RegisterUI(originUI);
        }

        private void RegisterUI(UIElement originUI)
        {
            playingWnd = AT_BC.Common.VisualTreeHelperExtension.GetParentObject<Window>(originUI);
            if (playingWnd != null)
            {
                playingWnd.Closed -= PlayingWindow_Closed;
                playingWnd.Closed += PlayingWindow_Closed;
            }
        }

        private void PlayingWindow_Closed(object sender, EventArgs e)
        {
            if (sender == this.playingWnd)
            {
                this.mediaPlayer.Stop();
            }
        }

        public static void Stop()
        {
            CurrentAudioPlayer.mediaPlayer.Stop();
            if (CurrentAudioPlayer.OnStopPlaying != null)
            {
                CurrentAudioPlayer.OnStopPlaying();
                CurrentAudioPlayer.OnStopPlaying = null;
            }
        }
    }
}
