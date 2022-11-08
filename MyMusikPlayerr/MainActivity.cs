using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using MyMusikPlayerr.Adapters;
using MyMusikPlayerr.Model;
using MyMusikPlayerr.MusicHelperClass;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using static Android.Provider.MediaStore;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;
using Uri = Android.Net.Uri;

namespace MyMusikPlayerr
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, SearchView.IOnQueryTextListener
    {
        private Toolbar _toolbar;
        private MusicListAdapter _musicListAdapter;
        private RecyclerView _musicRecyclerView;
        private List<SongData> _songData = new List<SongData>();
        private List<SongData> _filteredList = new List<SongData>();
        private PermissionStatus _permissionStatus;
        private Intent _stopServiceIntent;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            UiConnection();
            SetupToolbar();
            await FetchMusicAsync();
            if (_permissionStatus != PermissionStatus.Denied)
                SetupRecyclerView();
            SetUpServiceKillerIntent();
        }

        private void SetUpServiceKillerIntent()
        {
            _stopServiceIntent = new Intent(this, typeof(MusicPlayService));
           
        }

        protected override void OnDestroy()
        {
            MusicPlayerStaticCLass.ReleaseResource();
            StopService(_stopServiceIntent);
            FinishActivity(11);
            base.OnDestroy();
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return base.OnOptionsItemSelected(item);
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            new MenuInflater(this).Inflate(Resource.Menu.search_menu, menu);
            IMenuItem menuItem = menu.FindItem(Resource.Id.search);
            SearchView searchView = (SearchView)menuItem.ActionView;
            SearchSong(searchView);
            return base.OnCreateOptionsMenu(menu);
        }

        private void SearchSong(SearchView searchView)
        {
            searchView.SetOnQueryTextListener(this);
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
            SongData song = new SongData();
            if (_filteredList.Count > 0)
            {
                song = _filteredList[e.Position];
            }
            else
            {
                song = _songData[e.Position];
            }
            Intent intent = new Intent(this, typeof(SelectedMusicActivity));
            intent.PutExtra("path", song.Path);
            intent.PutExtra("name", song.Name);
            intent.PutExtra("position", e.Position);
            intent.PutExtra("duration", song.Duration);
            _filteredList.Clear();
            StartActivityForResult(intent, 11);
        }

        private async Task FetchMusicAsync()
        {
            var selection = Audio.Media.InterfaceConsts.IsMusic + " !=0";
            string[] projection =
            {
                Audio.Media.InterfaceConsts.Title,
                "_data",
                Audio.Media.InterfaceConsts.Duration,
                Audio.Media.InterfaceConsts.Id
            };
            _permissionStatus = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            if (_permissionStatus == PermissionStatus.Denied)
            {
                _permissionStatus = await Permissions.RequestAsync<Permissions.StorageRead>();
            }
            ContentResolver cr = this.ContentResolver;
            Uri uri;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
                uri = Audio.Media.ExternalContentUri;
            else
                uri = Audio.Media.GetContentUri(VolumeExternal);
            ICursor cur = cr.Query(uri, projection, selection, null, null);
            int count = 0;
            if (cur != null)
            {
                while (cur.MoveToNext())
                {
                    SongData song = new SongData();
                    song.Name = cur.GetString(cur.GetColumnIndex(Audio.Media.InterfaceConsts.Title));
                    song.Duration = cur.GetString(cur.GetColumnIndex(Audio.Media.InterfaceConsts.Duration));
                    song.Id = int.Parse(cur.GetString(cur.GetColumnIndex(Audio.Media.InterfaceConsts.Id)));
                    song.Path = cur.GetString(cur.GetColumnIndex("_data"));
                    // song.AlbumArtUri = cur.GetString(cur.GetColumnIndex(Audio.Media.InterfaceConsts.AlbumArt));
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
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public bool OnQueryTextChange(string newText)
        {
            _filteredList.Clear();
            _filteredList = _songData.Where(p => p.Name.ToLower().Contains(newText.ToLower())).ToList();
            _musicListAdapter.FilterSearched(_filteredList);
            return true;
        }
        public bool OnQueryTextSubmit(string newText)
        {
            return false;
        }
    }
}