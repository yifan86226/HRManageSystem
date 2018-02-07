using System.IO;
using System.Threading;
using log4net.Util;
using NAudio.Utils;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Best.VWPlatform.Controls.Audio
{
    enum StreamingPlaybackState
    {
        Stopped,
        Playing,
        Buffering,
        Paused
    }
    public class AudioPlayer
    {
        private BufferedWaveProvider bufferedWaveProvider;
        private IWavePlayer waveOut;
        private volatile StreamingPlaybackState playbackState;
        private VolumeWaveProvider16 volumeProvider;

        public AudioPlayer()
        {
            
        }

        private IWavePlayer CreateWaveOut()
        {
            return new WaveOut();
        }

        private void Pause()
        {
            playbackState = StreamingPlaybackState.Buffering;
            waveOut.Pause();
        }

        private void Play1()
        {
            waveOut.Play();
            playbackState = StreamingPlaybackState.Playing;
        }

        private void StopPlayback()
        {
            if (playbackState != StreamingPlaybackState.Stopped)
            {
                playbackState = StreamingPlaybackState.Stopped;
                if (waveOut != null)
                {
                    waveOut.Stop();
                    waveOut.Dispose();
                    waveOut = null;
                }
            }
        }

        private BufferedData _bufferedData = null;
        private byte[] buffer = new byte[65536];

        public void Push(byte[] pData)
        {
            if (_bufferedData == null)
            {
                return;
            }

            int re = _bufferedData.Write(pData, 0, pData.Length);
            while (re == 0)
            {
                Thread.Sleep(100);
                re = _bufferedData.Write(pData, 0, pData.Length);
            }
        }

        private static CAcmMp3FrameDecompressor CreateFrameDecompressor(CMp3Frame frame)
        {
            WaveFormat waveFormat = new Mp3WaveFormat(frame.SampleRate, frame.ChannelMode == ChannelMode.Mono ? 1 : 2,
                frame.FrameLength, frame.BitRate);
            return new CAcmMp3FrameDecompressor(waveFormat);
        }

        public void Stop()
        {
            StopPlayback();
        }

        public void Play(double pVolumeValue)
        {
            if (playbackState == StreamingPlaybackState.Stopped)
            {
                playbackState = StreamingPlaybackState.Buffering;
                bufferedWaveProvider = null;

                _bufferedData = new BufferedData(65536);
                
                ThreadPool.QueueUserWorkItem(Mp3Running);
                ThreadPool.QueueUserWorkItem(PlayingMp3, pVolumeValue);
            }
            else if (playbackState == StreamingPlaybackState.Paused)
            {
                playbackState = StreamingPlaybackState.Buffering;
            }
        }

        public void AdjustVolume(double pVolumeValue)
        {
            if (volumeProvider != null)
            {
                volumeProvider.Volume = (float)pVolumeValue;
            }
        }

        private void Mp3Running(object state)
        {
            CAcmMp3FrameDecompressor decompressor = null;
            try
            {
                do
                {
                    CMp3Frame frame = CMp3Frame.LoadFromBytes(_bufferedData);

                    if (frame == null)
                    {
                        continue;
                    }

                    if (decompressor == null)
                    {
                        decompressor = CreateFrameDecompressor(frame);
                        bufferedWaveProvider = new BufferedWaveProvider(decompressor.OutputFormat);
                        bufferedWaveProvider.BufferDuration = TimeSpan.FromSeconds(20);
                    }

                    int decompressed = decompressor.DecompressFrame(frame, buffer, 0);
                    bufferedWaveProvider.AddSamples(buffer, 0, decompressed);
                }
                while (playbackState != StreamingPlaybackState.Stopped);
            }
            catch (Exception)
            {

            }
            finally
            {
                StopPlayback();
                if (decompressor != null)
                {
                    decompressor.Dispose();
                }
            }
        }

        private void PlayingMp3(object state)
        {
            while (playbackState != StreamingPlaybackState.Stopped)
            {
                if (waveOut == null && bufferedWaveProvider != null)
                {
                    waveOut = CreateWaveOut();
                    volumeProvider = new VolumeWaveProvider16(bufferedWaveProvider);
                    volumeProvider.Volume = float.Parse(state.ToString());
                    waveOut.Init(volumeProvider);
                }
                else if (bufferedWaveProvider != null)
                {
                    var bufferedSeconds = bufferedWaveProvider.BufferedDuration.TotalSeconds;
                    if (bufferedSeconds < 0.1 && playbackState == StreamingPlaybackState.Playing)
                    {
                        Pause();
                    }
                    else if (bufferedSeconds > 1 && playbackState == StreamingPlaybackState.Buffering)
                    {
                        Play1();
                    }
                }

                Thread.Sleep(500);
            }
        }
    }
}
