using Android.Content;
using Android.OS;
using System;
using static Android.Icu.Text.Transliterator;
using Application = Android.App.Application;

namespace MyMusikPlayerr.MusicHelperClass
{
    public class ManipulateMusicFromNotificationWithoutActivity
    {
        public static ManipulateMusicFromNotificationWithoutActivity GetInstance()
        {
            return new ManipulateMusicFromNotificationWithoutActivity();
        }
        public void LoadNext()
        {
            Intent intent = new Intent(MusicPlayService.ActionPlay, null, Application.Context, typeof(MusicPlayService));
            intent.PutExtra("position", FetchNextPosition());
            intent.PutExtra("playpausebool", true);
            Application.Context.StartService(intent);
        }
        public void PlayResume()
        {
            Intent intent = new Intent(MusicPlayService.ActionPause, null, Application.Context, typeof(MusicPlayService));
            intent.PutExtra("position", MusicPlayerStaticCLass.GetCurrentSongIndex());
            intent.PutExtra("playpausebool", true);
            Application.Context.StartService(intent);
        }
        public void LoadPrevious()
        {
            Intent intent = new Intent(MusicPlayService.ActionPlay, null, Application.Context, typeof(MusicPlayService));
            intent.PutExtra("position", FetchPreviousPosition());
            intent.PutExtra("playpausebool", true);
            Application.Context.StartService(intent);
        }

        private int FetchNextPosition()
        {
            var DataList = StaticDataClass.GetSongList();
            MusicPlayerStaticCLass.UnSubscribCompletion();

            int position = MusicPlayerStaticCLass.GetCurrentSongIndex();
            if (((DataList.Count) - 1) > position)
            {
                position++;
            }
            else
            {
                position = 0;
            }
            return position;
        }

        private int FetchPreviousPosition()
        {
            int length = StaticDataClass.GetSongList().Count;
            MusicPlayerStaticCLass.UnSubscribCompletion();
            int position = MusicPlayerStaticCLass.GetCurrentSongIndex();
            if (position == 0)
            {
                position = length - 1; ;
            }
            else
            {
                position--;
            }
            return position;
        }
    }
}