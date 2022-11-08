using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MyMusikPlayerr.MusicHelperClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMusikPlayerr.Receiver
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    [IntentFilter(new[]{ AudioManager.ActionAudioBecomingNoisy})]
    public class VolumeManagerReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            context.StartService(new Intent(MusicPlayService.ActionPause, null, context,typeof(MusicPlayService)));
        }
    }
}