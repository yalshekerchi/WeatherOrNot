using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;

namespace WeatherOrNot
{
    [Activity(Label = "SecondScreen")]
    public class SecondScreen : Activity
    {
        private List<string> mItems;
        private ListView mListView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.second_layout);
            mListView = FindViewById<ListView>(Resource.Id.myListView);

            mItems = new List<string>();

            for (int i = 0; i < 7; i++)
            {
                if (i == 0)
                    mItems.Add(DateTime.Now.ToString());
                else
                    mItems.Add(DateTime.Now.AddDays(i).ToString());
            }

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mItems);
            mListView.Adapter = adapter;

            // Create your application here
        }
    }


}