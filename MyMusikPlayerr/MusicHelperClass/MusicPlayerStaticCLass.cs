using Android.App;
using Android.Media;
using Android.Net;

namespace MyMusikPlayerr.MusicHelperClass
{
    public static class MusicPlayerStaticCLass 
    {
        private static MediaPlayer _mediaPlayer;

      

        public static void PlaySong(string pathIn)
        {
            if (_mediaPlayer != null)
                _mediaPlayer.Release();
            _mediaPlayer = MediaPlayer.Create(Application.Context, Uri.Parse(pathIn));
            _mediaPlayer.Start();
           
        }

        
        public static MediaPlayer SendObjectOfMediaPlayer()
        {
            if(_mediaPlayer!=null)
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
            _mediaPlayer.Pause();
        }
        public static void StopSong()
        {
            if(_mediaPlayer!=null)
            _mediaPlayer.Stop();
        }
        public static void ReleaseResource()
        {
            _mediaPlayer.Stop();
            _mediaPlayer.Release();
        }
        public static void ResumeSong()
        {
            _mediaPlayer.Start();
        }
        public static void SetSongOnProgressChange(int currentVal)
        {
            _mediaPlayer.SeekTo(currentVal);
        }
        public static int GetCurrentPosition()
        {
            return _mediaPlayer.CurrentPosition;
        }

        
    }
}