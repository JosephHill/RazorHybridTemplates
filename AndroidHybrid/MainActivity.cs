using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Android.OS;

namespace AndroidHybrid
{
	[Activity (Label = "AndroidHybrid", MainLauncher = true)]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			var webView = FindViewById<WebView> (Resource.Id.LocalWebView);
			webView.Settings.JavaScriptEnabled = true;

			// Use subclassed WebViewClient to intercept hybrid native calls
			webView.SetWebViewClient (new HybridWebViewClient ());

			var template = new RazorView () { Model = "Text goes here" };
			var page = template.GenerateString ();

			webView.LoadDataWithBaseURL("file:///android_asset/", page, "text/html", "UTF-8", null);

		}

		private class HybridWebViewClient : WebViewClient {
			public override bool ShouldOverrideUrlLoading (WebView view, string url) {
				if (!url.StartsWith ("hybrid:")) 
					return false;

				var resources = url.Substring(7).Split('?');

				if (resources[0] == "UpdateLabel") {
					var values = System.Web.HttpUtility.ParseQueryString(resources[1]);

					var textvalue = values["textbox"];

					var prepended = string.Format ("C# says \"{0}\"", textvalue);

					var js = string.Format("SetLabelText('{0}');", prepended);

					view.LoadUrl ("javascript:" + js);
				}

				return true;
			}
		}
	}
}


