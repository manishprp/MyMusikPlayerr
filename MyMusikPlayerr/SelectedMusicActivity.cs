using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using Com.Airbnb.Lottie;
using MyMusikPlayerr.Model;
using MyMusikPlayerr.MusicHelperClass;
using MyMusikPlayerr.Receiver;
using System;
using System.Collections.Generic;
using Thread = System.Threading.Thread;
using Timer = System.Timers.Timer;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace MyMusikPlayerr
{
    [Activity(Label = "SelectedMusicActivity")]
    public class SelectedMusicActivity : AppCompatActivity, SeekBar.IOnSeekBarChangeListener 
    {
        private Toolbar _toolbar;
        private TextView _songNameTextView, _timeElapsedTextView, _songDurationTextView;
        private LottieAnimationView _musicImage;
        private SeekBar _durationSeekBar;
        private bool _isPlaying = true;
        private bool _nextPressed = false, _previousPressed = false;
        private Timer _timer;
        private Thread _uiThread;
        private int _mSeconds = 0;
        private List<SongData> DataList = new List<SongData>();
        private int _position = -1;
        private VolumeManagerReceiver volumeManagerReceiver;
        private string _songName, _path, _duration;
        private Button _playPauseButton, _reverseButton, _forwardButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.selected_music);
            UiConnection();
            SetUpToolbar();
            ClickEvents();
            GetIntentData();
            SetUpData();
            PlaySong();
            SeekBarSetUp();
        }

        private void RegisterMyBroadcastReceiver()
        {
            volumeManagerReceiver = new VolumeManagerReceiver();
            RegisterReceiver(volumeManagerReceiver, new IntentFilter(AudioManager.ActionAudioBecomingNoisy));
        }

        private void GetIntentData()
        {
            _songName = Intent.GetStringExtra("name");
            _path = Intent.GetStringExtra("path");
            _duration = Intent.GetStringExtra("duration");
            _position = Intent.GetIntExtra("position", -1);
        }
        private void SetUpToolbar()
        {
            SetSupportActionBar(_toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Title = "Playing Music...";
            _toolbar.NavigationClick += _toolbar_NavigationClick;
        }

        private void PlaySongOnService()
        {
            Intent intent = new Intent(MusicPlayService.ActionPlay, null, this, typeof(MusicPlayService));
            intent.PutExtra("position", _position);
            intent.PutExtra("playpausebool",true);
            StartService(intent);
        }

        private void _toolbar_NavigationClick(object sender, Toolbar.NavigationClickEventArgs e)
        {
            CleanUpResources();
            OnBackPressed();
        }

        public override void OnBackPressed()
        {
            CleanUpResources();
            base.OnBackPressed();
        }
        private void SeekBarSetUp()
        {
            _durationSeekBar.Max = int.Parse(_duration);
            _durationSeekBar.SetOnSeekBarChangeListener(this);
            _uiThread = new Thread(new System.Threading.ThreadStart(SetUpTimerAndSeekBarMoving));
            _uiThread.Start();
            //SetUpTimerAndSeekBarMoving();
        }
        private void SetUpTimerAndSeekBarMoving()
        {
            _timer = new Timer();
            _timer.Interval = 1;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }
        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _mSeconds++;
            RunOnUiThread(() =>
             {
                 _timeElapsedTextView.Text = getTime(MusicPlayerStaticCLass.GetCurrentPosition()).ToString();
                 _durationSeekBar.Progress = _mSeconds;
             });
        }
        private void ClickEvents()
        {
            _playPauseButton.Click += _playPauseButton_Click;
            _forwardButton.Click += _forwardButton_Click;
            _reverseButton.Click += _reverseButton_Click;
        }

        private void _reverseButton_Click(object sender, EventArgs e)
        {
            if (!_previousPressed)
            {
                _previousPressed = true;
                DataList = StaticDataClass.GetSongList();
                LoadPreviousSong();
            }
            else
            {
                return;
            }
        }
        private void LoadPreviousSong()
        {
            _timer.Close();
            int length = StaticDataClass.GetSongList().Count;
            MusicPlayerStaticCLass.UnSubscribCompletion();
            int position = _position;
            if (position == 0)
            {
                position = length - 1; ;
            }
            else
            {
                position--;
            }
            Intent intent = new Intent(this, typeof(SelectedMusicActivity));
            intent.PutExtra("path", StaticDataClass.GetSongList()[position].Path);
            intent.PutExtra("name", StaticDataClass.GetSongList()[position].Name);
            intent.PutExtra("duration", StaticDataClass.GetSongList()[position].Duration);
            intent.PutExtra("position", position);
            intent.AddFlags(ActivityFlags.ClearTop);
            CleanUpResources();
            StartActivity(intent);
            this.Finish();
        }
        

        private void _forwardButton_Click(object sender, EventArgs e)
        {
            if (!_nextPressed)
            {
                LoadNextSong();
                _nextPressed = true;
            }
            else
            {
                return;
            }
        }

        private void LoadNextSong()
        {

            _timer.Stop();
            DataList = StaticDataClass.GetSongList();
            MusicPlayerStaticCLass.UnSubscribCompletion();

            int position = _position;
            if (((DataList.Count) - 1) > _position)
            {
                position++;
            }
            else
            {
                position = 0;
            }
            Intent intent = new Intent(this, typeof(SelectedMusicActivity));
            intent.PutExtra("path", DataList[position].Path);
            intent.PutExtra("name", DataList[position].Name);
            intent.PutExtra("duration", DataList[position].Duration);
            intent.PutExtra("position", position);
            intent.AddFlags(ActivityFlags.ClearTop);
            CleanUpResources();
            StartActivity(intent);
            this.Finish();
        }
        private void CleanUpResources()
        {
            _timer.Enabled=false;
            _timer.Close(); 
            _timer.Dispose();
            _uiThread.Interrupt();
            MusicPlayerStaticCLass.nextSong -= MusicPlayerStaticCLass_nextSong;
            MusicPlayerStaticCLass.previousSong -= MusicPlayerStaticCLass_previousSong;
            MusicPlayerStaticCLass.Play -= MusicPlayerStaticCLass_Play;
            MusicPlayerStaticCLass.Pause -= MusicPlayerStaticCLass_Pause;
           _musicImage.PauseAnimation();
        }
        protected override void OnDestroy()
        {
            CleanUpResources();
            base.OnDestroy();
        }
        private void _playPauseButton_Click(object sender, EventArgs e)
        {
            PlayPauseMethod();
        }
        public string getTime(int duration)
        {
            int min = duration / 1000 / 60;
            int sec = duration / 1000 % 60;
            string time = min.ToString() + ":";
            if (sec < 10)
            {
                time += "0" + sec.ToString();
            }
            else
            {
                time += sec.ToString();
            }
            return time;
        }

        private void PlayPauseMethod()
        {
            
            if (_isPlaying)
            {
                MusicPlayerStaticCLass.PauseSong();
                CreateNotification.GetInstance(this, (NotificationManager)GetSystemService(NotificationService), _position, false).NewCreateNotification();
                _playPauseButton.SetBackgroundResource(Resource.Drawable.play_buton);
                _musicImage.PauseAnimation();
            }
            else
            {
                MusicPlayerStaticCLass.ResumeSong();
                CreateNotification.GetInstance(this, (NotificationManager)GetSystemService(NotificationService), _position, true).NewCreateNotification();
                _playPauseButton.SetBackgroundResource(Resource.Drawable.pause_button);
                _musicImage.PlayAnimation();
            }
            _isPlaying = !_isPlaying;
        }

        private void PlaySong()
        {
            PlaySongOnService();
            MusicPlayerStaticCLass.completed += MusicPlayerStaticCLass_completed;
            MusicPlayerStaticCLass.nextSong += MusicPlayerStaticCLass_nextSong;
            MusicPlayerStaticCLass.previousSong += MusicPlayerStaticCLass_previousSong;
            MusicPlayerStaticCLass.Play += MusicPlayerStaticCLass_Play;
            MusicPlayerStaticCLass.Pause += MusicPlayerStaticCLass_Pause;
        }

        private void MusicPlayerStaticCLass_Pause(object sender, EventArgs e)
        {
            PlayPauseMethod();
        }

        private void MusicPlayerStaticCLass_Play(object sender, EventArgs e)
        {
            PlayPauseMethod();
        }

        private void MusicPlayerStaticCLass_previousSong(object sender, EventArgs e)
        {
            LoadPreviousSong();
        }

        private void MusicPlayerStaticCLass_nextSong(object sender, EventArgs e)
        {
            LoadNextSong();
        }

        private void MusicPlayerStaticCLass_completed(object sender, EventArgs e)
        {
            LoadNextSong();
        }

        private void SetUpData()
        {
            _timeElapsedTextView.Text = "0:00";
            RunAnimation();
            _songNameTextView.Text = _songName;
            _songDurationTextView.Text = getTime(int.Parse(_duration));
        }
        private void RunAnimation()
        {
            _musicImage.SetAnimation(Resource.Raw.musiclottiePlay);
            _musicImage.PlayAnimation();
        }
        private void UiConnection()
        {
            _toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            _songNameTextView = FindViewById<TextView>(Resource.Id.songNameTextView);
            _timeElapsedTextView = FindViewById<TextView>(Resource.Id.timeElapsedTextView);
            _songDurationTextView = FindViewById<TextView>(Resource.Id.totalTimeTextView);
            _musicImage = FindViewById<LottieAnimationView>(Resource.Id.musicImage);
            _musicImage.EnableMergePathsForKitKatAndAbove(true);
            _musicImage.Hover += _musicImage_Hover;
            _durationSeekBar = FindViewById<SeekBar>(Resource.Id.durationSeekBar);
            _playPauseButton = FindViewById<Button>(Resource.Id.playPauseButton);
            _reverseButton = FindViewById<Button>(Resource.Id.reverseButton);
            _forwardButton = FindViewById<Button>(Resource.Id.forwardButton);
        }

        private void _musicImage_Hover(object sender, Android.Views.View.HoverEventArgs e)
        {
            Toast.MakeText(this,"Hovering",ToastLength.Short).Show();
        }

        public void OnProgressChanged(SeekBar seekBar, int progress, bool fromUser)
        {
            _mSeconds = seekBar.Progress;
            if (fromUser)
            {
                _durationSeekBar.Progress = progress;
            }
            return;
        }
        public void OnStartTrackingTouch(SeekBar seekBar)
        {
            _timer.Enabled = false;
        }
        public void OnStopTrackingTouch(SeekBar seekBar)
        {
            _timer.Enabled = true;
            if(MusicPlayerStaticCLass.SendObjectOfMediaPlayer()!=null)
            {
                MusicPlayerStaticCLass.SetSongOnProgressChange(seekBar.Progress);
            }
        }
    }
}