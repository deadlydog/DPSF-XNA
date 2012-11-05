using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Microsoft.Xna.Framework;

namespace DPSF_Demo_for_Mono_for_Android
{
	[Activity(Label = "DPSF_Demo_for_Mono_for_Android", MainLauncher = true, Icon = "@drawable/icon")]
	public class Activity1 : AndroidGameActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			DPSF_Demo_Phone.Game1.Activity = this;
			var game = new DPSF_Demo_Phone.Game1();
			SetContentView(game.Window);
			game.Run();
		}
	}
}

