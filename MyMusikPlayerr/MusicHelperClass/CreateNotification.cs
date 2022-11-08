using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Media.Session;
using AndroidX.Core.App;

namespace MyMusikPlayerr.MusicHelperClass
{
    public class CreateNotification
    {
        private static CreateNotification notification;
        private static Context context;
        private static NotificationManager notificationManager;
        private static Intent playOrPause = null;
        private static Intent previous, pause, next, play, openact, stopself;
        private static PendingIntent pi; 
        private static NotificationChannel chan;
        private static MediaSessionCompat mediaSession;
        private static bool isPlayPausePressed = false;
        private static int _position;
        public static CreateNotification GetInstance(Context contextIn, NotificationManager notificationManagerIn,int position, bool isPlayPausePressedIn)
        {
            isPlayPausePressed = isPlayPausePressedIn;
            context = contextIn;
            if(position<0)
            {
                _position = MusicPlayerStaticCLass.GetCurrentSongIndex();
            }
            else
            {
                _position = position;
            }
            notificationManager = notificationManagerIn;    
            if (notification == null)
            {
                notification = new CreateNotification();
            }
            CreateNotificationInitials(isPlayPausePressed);
            return notification;
        }
        private static void CreateNotificationInitials(bool isPlayPausePressedIn)
        {
            mediaSession = new MediaSessionCompat(Application.Context, "Player");
            var activity = new Intent(context, typeof(SelectedMusicActivity));
            chan = new NotificationChannel("MyId", "moosikplay", NotificationImportance.Low);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            {
                PendingIntent.GetActivity(context, 0, activity, PendingIntentFlags.Mutable | PendingIntentFlags.UpdateCurrent);
            }
            else
            {
                PendingIntent.GetActivity(context, 0, activity, PendingIntentFlags.UpdateCurrent);
            }
            SetUpIntents();
        }

        private static void SetUpIntents()
        {
            stopself = new Intent(MusicPlayService.ActionStopService, null, context, typeof(MusicPlayService));
            stopself.PutExtra("playpausebool", false);
            previous = new Intent(MusicPlayService.ActionPrevious, null, context, typeof(MusicPlayService));
            previous.PutExtra("position", _position);
            previous.PutExtra("playpausebool", true);
            pause = new Intent(MusicPlayService.ActionPause, null, context, typeof(MusicPlayService));
            pause.PutExtra("position", _position);
            pause.PutExtra("playpausebool", false);
            next = new Intent(MusicPlayService.ActionNext, null, context, typeof(MusicPlayService));
            next.PutExtra("position", _position);
            next.PutExtra("playpausebool", true);
            play = new Intent(MusicPlayService.ActionResume, null, context, typeof(MusicPlayService));
            play.PutExtra("position", _position);
            play.PutExtra("playpausebool", true);
            openact = new Intent(context, typeof(MainActivity));
            pi = PendingIntent.GetActivity(context, 0, openact, PendingIntentFlags.Mutable);
        }

        public Notification NewCreateNotification()
        {
            int button;
            string title;
            if(isPlayPausePressed)
            {
                button = Resource.Drawable.pause_button;
                title = "Pause";
                playOrPause = pause;
            }
            else
            {
                button = Resource.Drawable.play_buton;
                title = "Play";
                playOrPause = play;
            }
            
            if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            {
                notificationManager.CreateNotificationChannel(chan);
                var notification = new NotificationCompat.Builder(context, "MyId")
                 .SetSmallIcon(Android.Resource.Drawable.IcMediaPlay)
                 .SetChannelId("MyId")
                 .SetContentTitle("Playing Now")
                 .SetContentText(StaticDataClass.GetSongList()[_position].Name)
                 .SetStyle(new AndroidX.Media.App.NotificationCompat.MediaStyle().SetMediaSession(mediaSession.SessionToken))
                 .SetAutoCancel(false)
                 .SetDeleteIntent(PendingIntent.GetService(context, 0, stopself, PendingIntentFlags.Mutable |PendingIntentFlags.UpdateCurrent))
                 .SetContentIntent(pi)
                 .AddAction(Resource.Drawable.rewind_music, "Prev", PendingIntent.GetService(context, 0, previous, PendingIntentFlags.Mutable | PendingIntentFlags.UpdateCurrent))
                 .AddAction(button, title, PendingIntent.GetService(context, 0, playOrPause, PendingIntentFlags.Mutable | PendingIntentFlags.UpdateCurrent))
                 .AddAction(Resource.Drawable.forward_music, "Next", PendingIntent.GetService(context, 0, next, PendingIntentFlags.Mutable | PendingIntentFlags.UpdateCurrent))
                 .Build();
                return notification;
            }
            else
            {
                notificationManager.CreateNotificationChannel(chan);
                var notification = new NotificationCompat.Builder(context, "MyId")
                 .SetSmallIcon(Android.Resource.Drawable.IcMediaPlay)
                 .SetChannelId("MyId")
                 .SetContentTitle("Playing Now")
                 .SetContentText(StaticDataClass.GetSongList()[_position].Name)
                 .SetStyle(new AndroidX.Media.App.NotificationCompat.MediaStyle().SetMediaSession(mediaSession.SessionToken))
                 .SetAutoCancel(false)
                 .SetDeleteIntent(PendingIntent.GetService(context, 0, stopself, PendingIntentFlags.UpdateCurrent))
                 .SetContentIntent(pi)
                 .AddAction(Resource.Drawable.rewind_music, "Prev", PendingIntent.GetService(context, 0, previous, PendingIntentFlags.UpdateCurrent))
                 .AddAction(button, title, PendingIntent.GetService(context, 0, playOrPause, PendingIntentFlags.UpdateCurrent))
                 .AddAction(Resource.Drawable.forward_music, "Next", PendingIntent.GetService(context, 0, next, PendingIntentFlags.UpdateCurrent))
                 .Build();
                return notification;
            }
            
        }
    }
}