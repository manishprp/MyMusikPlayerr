using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;

namespace MyMusikPlayerr.MusicHelperClass
{
    [Service]
    public class MusicPlayService : Service,AudioManager.IOnAudioFocusChangeListener
    {
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
        public const string ActionPrevious = "ActionPrevious";
        public AudioManager manager;
        public const string ActionNext = "ActionNext";
        public const string ActionStopService = "ActionStopService";
        public const string ActionPlay = "ActionPlay";
        public const string ActionPause = "ActionPause";
        public const string ActionResume = "ActionResume";
        public const string ActionStop = "ActionStop";
        public int position= -1;
     
        public override bool StopService(Intent name)
        {
            StopSelf();
            StopForeground(true);
            return base.StopService(name);
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            manager = AudioManager.FromContext(ApplicationContext);
            var isPlayPauseEnabler = intent.GetBooleanExtra("playpausebool", false);
            manager.RequestAudioFocus(this, Stream.Music, AudioFocus.Gain);
            position = intent.GetIntExtra("position", -1);
            StartForeground(1, CreateNotification.GetInstance(this, (NotificationManager)GetSystemService(NotificationService), position, isPlayPauseEnabler).NewCreateNotification());
            switch (intent.Action)
            {
                case ActionPlay:
                    StartPlaying();
                    break;
                case ActionPause:
                    MusicPlayerStaticCLass.InvokePauseSong();
                    break;
                case ActionStop:
                    StopPlaying();
                    CleanUpPlayer();
                    break;
                case ActionPrevious:
                    MusicPlayerStaticCLass.InvokePreviousSong();
                    break;
                case ActionNext:
                    MusicPlayerStaticCLass.InvokeNextSong();
                    break;
                case ActionResume:
                    MusicPlayerStaticCLass.InvokePlaySong();
                    break;
                case ActionStopService:
                    StopSelf();
                    break;
            }
            return StartCommandResult.Sticky;
        }

        //private void PlayNext()
        //{
        //    //int position = -1;
        //    var curPos = position;
        //    if(StaticDataClass.GetSongList().Count-1==curPos)
        //    {
        //        curPos = 0;
        //    }  
        //    else
        //    {
        //        curPos++;
        //    }
        //    MusicPlayerStaticCLass.ReleaseResource();
        //    MusicPlayerStaticCLass.PlaySong(StaticDataClass.GetSongList()[curPos].Path, curPos);
        //}

        //private void PlayPrevious()
        //{
        //    var curPos = position;
        //    if (0 == curPos)
        //    {
        //        curPos = StaticDataClass.GetSongList().Count - 1;
        //    }
        //    else
        //    {
        //        curPos--;
        //    }
        //    MusicPlayerStaticCLass.ReleaseResource();
        //    MusicPlayerStaticCLass.PlaySong(StaticDataClass.GetSongList()[curPos].Path, curPos);
        //}

        private void CleanUpPlayer()
        {
            MusicPlayerStaticCLass.ReleaseResource();
        }

        private void StopPlaying()
        {
            MusicPlayerStaticCLass.StopSong();
        }

        private void PausePlaying()
        {
            MusicPlayerStaticCLass.PauseSong();
        }

        private void StartPlaying()
        {
           MusicPlayerStaticCLass.PlaySong(StaticDataClass.GetSongList()[position].Path, position);
        }

        public void OnAudioFocusChange([GeneratedEnum] AudioFocus focusChange)
        {
            switch (focusChange)
            {
                case AudioFocus.Gain:
                    MusicPlayerStaticCLass.ResumeSong();
                    //MusicPlayerStaticCLass.SendObjectOfMediaPlayer().SetVolume(1.0f, 1.0f);
                    break;
                case AudioFocus.Loss:
                    StopPlaying();
                    CleanUpPlayer();
                    break;
                case AudioFocus.LossTransient:
                    PausePlaying();
                    break;
                case AudioFocus.LossTransientCanDuck:
                    MusicPlayerStaticCLass.SendObjectOfMediaPlayer().SetVolume(0.25f, 0.25f);
                    break;
            }
        }
    }
}