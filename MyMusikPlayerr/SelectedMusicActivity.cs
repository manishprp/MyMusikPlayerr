using Android.App;
using Android.Content;
using Android.OS;
using Android.Telephony;
using Android.Widget;
using AndroidX.AppCompat.App;
using Com.Airbnb.Lottie;
using MyMusikPlayerr.Model;
using MyMusikPlayerr.MusicHelperClass;
using System;
using System.Collections.Generic;
using Timer = System.Timers.Timer;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace MyMusikPlayerr
{
    [Activity(Label = "SelectedMusicActivity")]
    public class SelectedMusicActivity : AppCompatActivity, TelephonyCallback.ICallStateListener
    {
        private Toolbar _toolbar;
        private TextView _songNameTextView, _timeElapsedTextView, _songDurationTextView;
        private LottieAnimationView _musicImage;
        private SeekBar _durationSeekBar;
        private bool _isPlaying = true;
        private Timer _timer;
        private int _mSeconds = 0;
        private List<SongData> DataList = new List<SongData>();
        private int _position=-1;
        private string _songName, _path, _duration;
        private Button _playPauseButton, _reverseButton, _forwardButton;
      
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.selected_music);
            UiConnection();
            SetUpToolbar();
            ClickEvents();
            _songName = Intent.GetStringExtra("name");
            _path = Intent.GetStringExtra("path");
            _duration = Intent.GetStringExtra("duration");
            _position = Intent.GetIntExtra("position", -1);
            SetUpData();
            PlaySong();
            SeekBarSetUp();
            //TelephonyManager tmanager = (TelephonyManager)this.GetSystemService(TelephonyService);
            //tmanager.RegisterTelephonyCallback(this.MainExecutor, new PhoneStateListenerClass());
        }


        private void SetUpToolbar()
        {
            SetSupportActionBar(_toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Title = "Music Playing";
            _toolbar.NavigationClick += _toolbar_NavigationClick;
        }

        private void _toolbar_NavigationClick(object sender, Toolbar.NavigationClickEventArgs e)
        {
            OnBackPressed();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
        private void SeekBarSetUp()
        {
            _durationSeekBar.Max = int.Parse(_duration);
            _durationSeekBar.StartTrackingTouch += _durationSeekBar_StartTrackingTouch;
            _durationSeekBar.StopTrackingTouch += _durationSeekBar_StopTrackingTouch;
            SetUpTimerAndSeekBarMoving();
        }

        private void _durationSeekBar_StopTrackingTouch(object sender, SeekBar.StopTrackingTouchEventArgs e)
        {
            _timer.Enabled = true;
            MusicPlayerStaticCLass.SetSongOnProgressChange(e.SeekBar.Progress);
            _mSeconds = e.SeekBar.Progress;
        }

        private void _durationSeekBar_StartTrackingTouch(object sender, SeekBar.StartTrackingTouchEventArgs e)
        {
            _timer.Enabled = false;
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
            if(_mSeconds == (int.Parse(_duration)))
            {
                _timer.Stop();
            }
            RunOnUiThread(() =>
            {
                _durationSeekBar.Progress = MusicPlayerStaticCLass.GetCurrentPosition();
                _timeElapsedTextView.Text = getTime(_mSeconds);
                if(_timeElapsedTextView.Text == _songDurationTextView.Text)
                {
                    _musicImage.PauseAnimation();
                    LoadNextSong();
                }
            });
        }

        private void _durationSeekBar_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            MusicPlayerStaticCLass.SetSongOnProgressChange(e.Progress);
            _mSeconds = e.Progress;
        }
        private void ClickEvents()
        {
            _playPauseButton.Click += _playPauseButton_Click;
            _forwardButton.Click += _forwardButton_Click;
            _reverseButton.Click += _reverseButton_Click;
        }

        private void _reverseButton_Click(object sender, EventArgs e)
        {
            DataList = StaticDataClass.GetSongList();
            LoadPreviousSong(0, (DataList.Count) - 1);
        }

        private void LoadPreviousSong(int max, int min)
        {
            _timer.Close();
            MusicPlayerStaticCLass.StopSong();

            int position = _position;
            if (position==max)
            {
                position = min;
            }
            else
            {
                position--;
            }
            Intent intent = new Intent(this, typeof(SelectedMusicActivity));
            intent.PutExtra("path", DataList[position].Path);
            intent.PutExtra("name", DataList[position].Name);
            intent.PutExtra("duration", DataList[position].Duration);
            intent.PutExtra("position", position);
            intent.AddFlags(ActivityFlags.ClearTop);
            StartActivity(intent);
            this.Finish();
        }

        private void _forwardButton_Click(object sender, EventArgs e)
        {
             
            LoadNextSong();
        }

        private void LoadNextSong()
        {
            DataList = StaticDataClass.GetSongList();
            _timer.Close();
            MusicPlayerStaticCLass.StopSong();
            
            int position = _position;
            if (((DataList.Count) - 1 )> _position)
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
            StartActivity(intent);
            this.Finish();
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
                _playPauseButton.SetBackgroundResource(Resource.Drawable.play_buton);
                _musicImage.PauseAnimation();
            }
            else
            {
                MusicPlayerStaticCLass.ResumeSong();
                _playPauseButton.SetBackgroundResource(Resource.Drawable.pause_button);
                _musicImage.PlayAnimation();
            }
            _isPlaying = !_isPlaying;
        }

        private void PlaySong()
        {
            MusicPlayerStaticCLass.PlaySong(_path);
        }

        private void SetUpData()
        {
            //Glide.With(this).Load(Resource.Raw.musiclottiePlay).Into(_musicImage);
            _musicImage.SetAnimation(Resource.Raw.musiclottiePlay);
            _musicImage.PlayAnimation();
            _songNameTextView.Text = _songName;
            _songDurationTextView.Text = getTime(int.Parse(_duration));
        }
        private void UiConnection()
        {
            _toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            _songNameTextView = FindViewById<TextView>(Resource.Id.songNameTextView);
            _timeElapsedTextView = FindViewById<TextView>(Resource.Id.timeElapsedTextView);
            _timeElapsedTextView.Text = "0:00";
            _songDurationTextView = FindViewById<TextView>(Resource.Id.totalTimeTextView);
            _musicImage = FindViewById<LottieAnimationView>(Resource.Id.musicImage);
            _durationSeekBar = FindViewById<SeekBar>(Resource.Id.durationSeekBar);
            _playPauseButton = FindViewById<Button>(Resource.Id.playPauseButton);
            _reverseButton = FindViewById<Button>(Resource.Id.reverseButton);
            _forwardButton = FindViewById<Button>(Resource.Id.forwardButton);
        }

        public void OnCallStateChanged(int state)
        {
            if(state ==(int) CallState.Ringing)
            {
                Toast.MakeText(this, "Ringing",ToastLength.Short).Show();
            }
            if (state == (int)CallState.Idle)
            {
                Toast.MakeText(this, "No call", ToastLength.Short).Show();
            }
            if (state == (int)CallState.Offhook)
            {
                Toast.MakeText(this, "talking", ToastLength.Short).Show();
            }
        }

        //public void OnProgressChanged(SeekBar seekBar, int progress, bool fromUser)
        //{
        //    //_durationSeekBar.Progress = progress;
        //    //MusicPlayerStaticCLass.SetSongOnProgressChange(_durationSeekBar.Progress);
        //}

        //public void OnStartTrackingTouch(SeekBar seekBar)
        //{
        //    //_durationSeekBar.ProgressChanged += _durationSeekBar_ProgressChanged;
        //}

        //public void OnStopTrackingTouch(SeekBar seekBar)
        //{
        //    //_durationSeekBar.ProgressChanged -= _durationSeekBar_ProgressChanged;
        //}
    }
}