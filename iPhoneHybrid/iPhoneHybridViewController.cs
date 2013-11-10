using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace iPhoneHybrid
{
	public partial class iPhoneHybridViewController : UIViewController
	{
		public iPhoneHybridViewController (IntPtr handle) : base (handle)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Intercept URL loading to handle native calls from browser
			uiWebView.ShouldStartLoad += HandleShouldStartLoad;

			// Render the view from the type generated from RazorView.cshtml
			var template = new RazorView () { Model = "Text goes here" };
			var page = template.GenerateString ();

			// Load the rendered HTML into the view with a base URL 
			// that points to the root of the bundled Resources folder
			uiWebView.LoadHtmlString (page, NSBundle.MainBundle.BundleUrl);

			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}

		#endregion

		bool HandleShouldStartLoad (UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType) {

			// If the URL is not our own special scheme, just let the webView load the URL as usual
			if (request.Url.Scheme != "hybrid")
				return true;

			var resources = request.Url.ResourceSpecifier.Split('?');

			if (resources[0] == "UpdateLabel") {
				var values = System.Web.HttpUtility.ParseQueryString(resources[1]);

				var textvalue = values["textbox"];

				var prepended = string.Format ("C# says \"{0}\"", textvalue);

				var js = string.Format("SetLabelText('{0}');", prepended);

				webView.EvaluateJavascript (js);
			}

			return false;
		}

	}
}

