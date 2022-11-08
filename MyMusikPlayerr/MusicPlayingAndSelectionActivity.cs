using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMusikPlayerr
{
    [Activity(Label = "MusicPlayingAndSelectionActivity")]
    public class MusicPlayingAndSelectionActivity : AppCompatActivity
    {
        private ImageView ImageView;
        private TextView TextView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
    }
}