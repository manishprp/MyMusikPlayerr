using Android.Service.Voice;
using MyMusikPlayerr.Model;
using System.Collections.Generic;

namespace MyMusikPlayerr.MusicHelperClass
{
    public static class StaticDataClass
    {
        public static List<SongData> mainSongList = new List<SongData>();

        public static void SetSongList(List<SongData> mainSongListIn)
        {
            mainSongList = mainSongListIn;
        }

        public static List<SongData> GetSongList ()
        {
            return mainSongList;
        }
    }
}