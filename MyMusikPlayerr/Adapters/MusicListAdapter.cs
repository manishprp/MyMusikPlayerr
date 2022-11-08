using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MyMusikPlayerr.Model;
using System;
using System.Collections.Generic;

namespace MyMusikPlayerr.Adapters
{
    public class MusicListAdapter : RecyclerView.Adapter
    {
        public event EventHandler<MusicListAdapterClickEventArgs> ItemClick;
        public event EventHandler<MusicListAdapterClickEventArgs> ItemLongClick;
        List<SongData> items;

        public MusicListAdapter(List<SongData> data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.musik_row_item;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);
            var vh = new MusicListAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as MusicListAdapterViewHolder;
            holder.textViewSongName.Text = item.Name;
        }

        public override int ItemCount => items.Count;

        void OnClick(MusicListAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(MusicListAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

        public void FilterSearched(List<SongData> list)
        {
            items = list;
            NotifyDataSetChanged();
        }
    }

    public class MusicListAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView textViewSongName { get; set; }


        public MusicListAdapterViewHolder(View itemView, Action<MusicListAdapterClickEventArgs> clickListener,
                            Action<MusicListAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            textViewSongName = itemView.FindViewById<TextView>(Resource.Id.textViewSongName);
            itemView.Click += (sender, e) => clickListener(new MusicListAdapterClickEventArgs { View = itemView, Position = LayoutPosition });
            itemView.LongClick += (sender, e) => longClickListener(new MusicListAdapterClickEventArgs { View = itemView, Position = LayoutPosition });
        }
    }

    public class MusicListAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }

    }
}