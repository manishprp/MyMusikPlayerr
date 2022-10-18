using Android.App;
using Android.Content;
using Android.Database;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Provider;
using Android.Text;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using static Android.Provider.MediaStore;
using Environment = Android.OS.Environment;

namespace MyMusikPlayerr
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private Toolbar _toolbar;
        private RecyclerView _musicRecyclerView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            UiConnection();
            SetupToolbar();
            FetchMusic();
        }

        private void FetchMusic()
        {
            List<string> title = new List<string>();
            List<string> path = new List<string>();
            ContentResolver cr = this.ContentResolver;
            var uri = MediaStore.Audio.Media.InternalContentUri;
            ICursor cur = cr.Query(uri, null, null, null, null);
            int count = 0;
            if(cur!=null)
            {
                while(cur.MoveToNext())
                {
                    count++;
                    var names = cur.GetColumnNames();
                    title.Add(cur.GetString(cur.GetColumnIndex("title")));
                    path.Add(cur.GetString(cur.GetColumnIndex("_data")));
                }
            }
        }

        private void SetupToolbar()
        {
            SetSupportActionBar(_toolbar);
            SupportActionBar.Title = "Songs";
        }

        private void UiConnection()
        {
            _toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            _musicRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
        }
    }
}