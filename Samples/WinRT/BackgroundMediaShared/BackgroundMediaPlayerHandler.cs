﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tomato.Media;
using Tomato.Media.Codec;
using Windows.Media;
using Windows.Storage;

namespace BackgroundMediaShared
{
    public sealed class BackgroundMediaPlayerHandler : IBackgroundMediaPlayerHandler
    {
        private BackgroundMediaPlayer mediaPlayer;
        private SystemMediaTransportControls smtc;
        private CodecManager _codecManager;

        private static readonly string[] files = new[]
        {
            "04.花篝り.APE",
            "04 - irony -TV Mix-.mp3",
            "08. きらきらエブリディ.mp3"
        };

        public void OnActivated(BackgroundMediaPlayer mediaPlayer)
        {
            _codecManager = new CodecManager();
            _codecManager.RegisterDefaultCodecs();

            this.mediaPlayer = mediaPlayer;
            smtc = mediaPlayer.SystemMediaTransportControls;
            ConfigureSystemMediaTransportControls();
            mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            mediaPlayer.CurrentStateChanged += MediaPlayer_CurrentStateChanged;
            Switch();
        }

        private int t = 0;
        private async void Switch()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            t = t == 0 ? 0 : 0;
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/{files[t]}"));
            var stream = await file.OpenReadAsync();
            var mediaSource = await MediaSource.CreateFromStream(stream);
            var duration = mediaSource.Duration;
            //Debug.WriteLine($"Title: {mediaSource.Title}");
            //Debug.WriteLine($"Album: {mediaSource.Album}");
            //Debug.WriteLine($"Artist: {mediaSource.Artist}");
            //Debug.WriteLine($"AlbumArtist: {mediaSource.AlbumArtist}");
            //Debug.WriteLine($"Duration: {mediaSource.Duration}");
            //Debug.WriteLine($"Lyrics: {mediaSource.Lyrics}");

            smtc.DisplayUpdater.MusicProperties.Title = "ss";
            smtc.DisplayUpdater.Update();

            mediaPlayer.SetMediaSource(mediaSource);
        }

        private void MediaPlayer_MediaEnded(IMediaPlayer sender, object args)
        {
            Debug.WriteLine("Media ended.");
            mediaPlayer.Play();
        }

        private void MediaPlayer_CurrentStateChanged(IMediaPlayer sender, object args)
        {
            switch (mediaPlayer.State)
            {
                case Windows.Media.Playback.MediaPlayerState.Closed:
                    smtc.PlaybackStatus = MediaPlaybackStatus.Closed;
                    break;
                case Windows.Media.Playback.MediaPlayerState.Opening:
                    smtc.PlaybackStatus = MediaPlaybackStatus.Changing;
                    break;
                case Windows.Media.Playback.MediaPlayerState.Buffering:
                    smtc.PlaybackStatus = MediaPlaybackStatus.Changing;
                    break;
                case Windows.Media.Playback.MediaPlayerState.Playing:
                    smtc.PlaybackStatus = MediaPlaybackStatus.Playing;
                    break;
                case Windows.Media.Playback.MediaPlayerState.Paused:
                    smtc.PlaybackStatus = MediaPlaybackStatus.Paused;
                    break;
                case Windows.Media.Playback.MediaPlayerState.Stopped:
                    smtc.PlaybackStatus = MediaPlaybackStatus.Stopped;
                    break;
                default:
                    break;
            }
        }

        private void ConfigureSystemMediaTransportControls()
        {
            smtc.ButtonPressed += Smtc_ButtonPressed;
            smtc.IsPlayEnabled = smtc.IsPauseEnabled = true;
            smtc.IsEnabled = true;
            smtc.IsPreviousEnabled = smtc.IsNextEnabled = true;
            smtc.PlaybackStatus = MediaPlaybackStatus.Closed;
            smtc.DisplayUpdater.Type = MediaPlaybackType.Music;
            smtc.DisplayUpdater.Update();
        }

        private void Smtc_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    mediaPlayer.Play();
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    mediaPlayer.Pause();
                    break;
                case SystemMediaTransportControlsButton.Stop:
                    mediaPlayer.Pause();
                    break;
                case SystemMediaTransportControlsButton.Record:
                    break;
                case SystemMediaTransportControlsButton.FastForward:
                    break;
                case SystemMediaTransportControlsButton.Rewind:
                    break;
                case SystemMediaTransportControlsButton.Next:
                    Switch();
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    Switch();
                    break;
                case SystemMediaTransportControlsButton.ChannelUp:
                    break;
                case SystemMediaTransportControlsButton.ChannelDown:
                    break;
                default:
                    break;
            }
        }

        public void OnReceiveMessage(string tag, string message)
        {
            if (message == "Play")
            {
                mediaPlayer.Position = TimeSpan.FromSeconds(285);
                mediaPlayer.Play();
                mediaPlayer.SendMessage("M", "Playing");
            }
            Debug.WriteLine($"Client Message: {tag}, {message}");
        }

        private void MediaPlayer_MediaOpened(IMediaPlayer sender, object args)
        {
            mediaPlayer.SendMessage("M", "Ready to Play");
        }

        public void OnCanceled()
        {

        }
    }
}
