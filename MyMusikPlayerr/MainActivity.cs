using Android.App;
using Android.Content;
using Android.Database;
using Android.Media;
using Android.OS;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using MyMusikPlayerr.Adapters;
using MyMusikPlayerr.Model;
using MyMusikPlayerr.MusicHelperClass;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using static Android.Provider.MediaStore;
using Uri = Android.Net.Uri;

namespace MyMusikPlayerr
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private Toolbar _toolbar;
        private MusicListAdapter _musicListAdapter;
        private RecyclerView _musicRecyclerView;
        private List<SongData> _songData = new List<SongData>();
        private MediaPlayer _mediaPlayer ;
        private PermissionStatus _permissionStatus;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            UiConnection();
            SetupToolbar();
             await FetchMusicAsync();
            if(_permissionStatus != PermissionStatus.Denied)
            SetupRecyclerView();
        }

        private void SetupRecyclerView()
        {
            StaticDataClass.SetSongList(_songData);
            _musicRecyclerView.SetLayoutManager(new LinearLayoutManager(this));
            _musicListAdapter = new MusicListAdapter(_songData);
            _musicRecyclerView.SetAdapter(_musicListAdapter);
            _musicListAdapter.ItemClick += _musicListAdapter_ItemClick;
        }

        private void _musicListAdapter_ItemClick(object sender, MusicListAdapterClickEventArgs e)
        {
            //if (_mediaPlayer != null)
            //    _mediaPlayer.Release();
            _mediaPlayer = MediaPlayer.Create(this, Uri.Parse(_songData[e.Position].Path));
            //_mediaPlayer.Start();
            Intent intent = new Intent(this, typeof(SelectedMusicActivity));
            intent.PutExtra("path", _songData[e.Position].Path);
            intent.PutExtra("name", _songData[e.Position].Name);
            intent.PutExtra("position", e.Position);
            intent.PutExtra("duration", _songData[e.Position].Duration);
            StartActivity(intent);
        }

        private async Task FetchMusicAsync()
        {
            var selection = Audio.Media.InterfaceConsts.IsMusic + " !=0";
            string[] projection =
            {
                Audio.Media.InterfaceConsts.Title,
                Audio.Media.InterfaceConsts.Data,
                Audio.Media.InterfaceConsts.Duration,
                Audio.Media.InterfaceConsts.Id,
                Audio.Media.InterfaceConsts.IsDownload
            };

           
            _permissionStatus = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            if (_permissionStatus == PermissionStatus.Denied)
            {
                _permissionStatus = await Permissions.RequestAsync<Permissions.StorageRead>();

            }
            ContentResolver cr = this.ContentResolver;
            Uri uri;
            if(Build.VERSION.SdkInt>=BuildVersionCodes.Q)
                uri = Audio.Media.ExternalContentUri;
            else
                uri = Audio.Media.GetContentUri(VolumeExternal);
            ICursor cur = cr.Query(uri, projection, selection, null, null);
            int count = 0;
            if(cur!=null)
            {
                //var names = cur.GetColumnNames();
               while(cur.MoveToNext())
                {
                    SongData song = new SongData();
                    song.Name = cur.GetString(cur.GetColumnIndex(Audio.Media.InterfaceConsts.Title)); 
                    song.Duration = cur.GetString(cur.GetColumnIndex(Audio.Media.InterfaceConsts.Duration));
                    song.Id = int.Parse(cur.GetString(cur.GetColumnIndex(Audio.Media.InterfaceConsts.Id))); 
                    song.Path = cur.GetString(cur.GetColumnIndex(Audio.Media.InterfaceConsts.Data));
                    _songData.Add(song);    
                    count++;
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
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}