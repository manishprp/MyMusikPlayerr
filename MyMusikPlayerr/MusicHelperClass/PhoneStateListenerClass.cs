using Android.Telephony;

namespace MyMusikPlayerr.MusicHelperClass
{
    public class PhoneStateListenerClass : TelephonyCallback, TelephonyCallback.ICallStateListener
    {
        public void OnCallStateChanged(int state)
        {
           if( state ==(int)CallState.Ringing)
            {

            }
        }
    }
}