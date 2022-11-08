using Android.App;
using Android.Media;
using Android.Net;
using System;

namespace MyMusikPlayerr.MusicHelperClass
{
    public static class MusicPlayerStaticCLass
    {
        private static MediaPlayer _mediaPlayer;
        private static int _position=-2;
        public static event EventHandler completed;
        public static event EventHandler nextSong;
        public static event EventHandler previousSong;
        public static event EventHandler Play;
        public static event EventHandler Pause;

        public static void PlaySong(string pathIn, int position)
        {
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Release();
            }
            _position =position;
            _mediaPlayer = MediaPlayer.Create(Application.Context, Android.Net.Uri.Parse(pathIn));
            _mediaPlayer.Start();
            _mediaPlayer.Completion += _mediaPlayer_Completion;

        }
        public static void InvokeNextSong()
        {
            object sender = new object();
            if(nextSong != null)
            {
                nextSong?.Invoke(Application.Context, EventArgs.Empty);
            }
            else
            {
                ManipulateMusicFromNotificationWithoutActivity.GetInstance().LoadNext();
            }
        }
        public static void InvokePreviousSong()
        {
            object sender = new object();
            if (previousSong != null)
            {
                previousSong?.Invoke(Application.Context, EventArgs.Empty);
            }
            else
            {
                ManipulateMusicFromNotificationWithoutActivity.GetInstance().LoadPrevious();
            }
        }
        public static void InvokePlaySong()
        {
            object sender = new object();
            if (Play != null)
            {
                Play?.Invoke(Application.Context, EventArgs.Empty);
            }
            else
            {
                ResumeSong();
            }
        }
        public static void InvokePauseSong()
        {
            object sender = new object();
            if (Pause != null)
            {
                Pause?.Invoke(Application.Context, EventArgs.Empty);
            }
            else
            {
                PauseSong();
            }
        }
        private static void _mediaPlayer_Completion(object sender, System.EventArgs e)
        {
            completed?.Invoke(sender, e);
        }

        public static void UnSubscribCompletion()
        {
            _mediaPlayer.Completion -= _mediaPlayer_Completion;
        }

        public static int GetCurrentSongIndex()
        {
            return _position;
        }
        public static MediaPlayer SendObjectOfMediaPlayer()
        {
            if (_mediaPlayer != null)
            {
                return _mediaPlayer;
            }
            else
            {
                return null;
            }
        }
        public static void OnComplete()
        {
            _mediaPlayer.SetOnCompletionListener(null);
        }
        public static void PauseSong()
        {
            if(_mediaPlayer!=null)
            {
                _mediaPlayer.Pause();
            }
        }
        public static void StopSong()
        {
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Stop();
            }
        }
        public static void ReleaseResource()
        {
            _mediaPlayer.Stop();
            _mediaPlayer.Release();
            _mediaPlayer = null;
        }
        public static void ResumeSong()
        {
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Start();
            }
        }
        public static void SetSongOnProgressChange(int currentVal)
        {
            _mediaPlayer.SeekTo(currentVal);
        }
        public static int GetCurrentPosition()
        {
            if (_mediaPlayer == null)
            {
                return 0;
            }
            else
            {
                return _mediaPlayer.CurrentPosition;
            }
        }
    }
}