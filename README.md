RazorHybrid
===========
This project provides several simple-project templates for developers that want to build hybrid applications for iOS or Android using Xamarin and the Razor templating engine.  Mobile hybrid applications are typically native apps use a full-screen web browser to present the views in an app, and as a view engine for ASP.NET MVC, Razor is particularly well suited to rendering HTML for mobile hybrid applications written in C#.

This repository includes templates for new Android projects, as well new iPhone, iPad, and Universal (iPhone and iPad combined) projects.  No assumption has been made as to which subset of project types developers want to use as the basis for their own apps, and no code-sharing strategies have been employed to attempt to keep common files in sync between projects.  As such, a solution file has been committed for each project type.  Developers that use these templates can read [Xamarin’s Code Sharing Options guide](http://docs.xamarin.com/guides/cross-platform/application_fundamentals/building_cross_platform_applications/sharing_code_options/) for help in selecting an appropriate strategy for their app.

## Razor Template
All projects include a copy of RazorView.cshtml.  This is the primary Razor template that is used to create the HTML that is rendered. It is a very minimal template, but demonstrates a few key concepts:

### Static Content
This project includes a minimal style sheet intended to demonstrate how to include static content in your hybrid app.  Other static content, such as javascript files and images, can be included in the same folder as the .css file (Resources on iOS or Assets on Android), with the same Build Action (BundleResource on iOS or AndroidAsset on Android), and these resources will be accessible from a path starting at the root of your hybrid application. E.g.,
````
<link rel="stylesheet" href="style.css" />
````

### Calling C# from Javascript
Communication from a rendered webview calling back to C# is done by setting the URL for the webview, and then intercepting the request in C# to handle the native request without reloading the webview.  

An example can be seen in how RazorView’s button is handled.  The button has 
````html
<input type="button" name="UpdateLabel" value="Click" onclick="InvokeCSharpWithFormValues(this)" />
````

The InvokeCSharpWithFormValues javascript function reads all of the values from the HTML Form and sets the location.href for the webview:

````js
location.href = "hybrid:" + elm.name + "?" + qs;
````

This attempts to navigate the webview to a URL with a custom scheme that we’ve made up (“hybrid”):

````
hybrid:UpdateLabel?textbox=SomeValue&UpdateLabel=Click
````

When the native webview processes this navigation request, we have the opportunity to intercept it.  In iOS, this is done by handling the UIWebView’s `HandleShouldStartLoad event`.  In Android, we simply subclass the WebViewClient used in the form, and override `ShouldOverrideUrlLoading`.  

The internals of these two navigation interceptors is essentially the same.  

First, we check the URL that webview is attempting to load, and if it doesn’t start with our custom scheme (“hybrid:”), we allow the navigation to occur as normal.  

For our custom URL scheme, we treat everything in the URL between the scheme and the “?” as the method name to be handled (in this case, “UpdateLabel”).  Everything in the querystring will be treated as the parameters to the method call:

````csharp
var resources = url.Substring(scheme.Length).Split('?');
var method = resources [0];
var parameters = System.Web.HttpUtility.ParseQueryString(resources[1]);
````

UpdateLabel in this sample does a minimal amount of string manipulation on the “textbox” parameter (prepending “C# says’” to the string), and then calls back to the webview.

At the end of our URL handling, we abort the navigation so that the webview does not attempt to finish navigating to our custom URL.

### Calling Javascript from C#
Communication to a rendered HTML webview from C# is done by calling javascript in the webview.  On iOS, this is done by calling `EvaluateJavascript` on the UIWebView:  

````csharp
webView.EvaluateJavascript (js);
````

On Android, javascript can be invoked in the webview by loading the javascript as a URL using the “javascript:” URL scheme:

````csharp
webView.LoadUrl ("javascript:" + js);
````

### Native Layout
These templates do not make use of native controls on each platform; however, the webviews are hosted in native layout files that can be edited with the native designers for each platform.  To add native chrome or widgetry outside of the main webview on iOS, you can edit the MainStoryboard.storyboard in the iOS designer.  On Android, you can open  Resources/layout/Main.axml
