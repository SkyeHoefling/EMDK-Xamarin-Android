using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Symbol.XamarinEMDK.Barcode;

namespace EMDKConfigUpdates
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
	public class MainActivity : AppCompatActivity, View.IOnClickListener
	{
		private BarcodeScannerManager _scannerManager;
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			Xamarin.Essentials.Platform.Init(this, savedInstanceState);
			_scannerManager = new BarcodeScannerManager(this);
			_scannerManager.ScanReceived += OnScanReceived;

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.activity_main);

			var toggleScannerButton = FindViewById<Button>(Resource.Id.toggle_scanner_button);
			toggleScannerButton.SetOnClickListener(this);
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
		{
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			_scannerManager.ScanReceived -= OnScanReceived;
			_scannerManager.Dispose();
			_scannerManager = null;
		}

		private void OnScanReceived(object sender, Scanner.DataEventArgs args)
		{
			var scanDataCollection = args.P0;
			if (scanDataCollection?.Result == ScannerResults.Success)
			{
				var textView = FindViewById<TextView>(Resource.Id.last_scan_received);
				if (textView != null)
					textView.Text = scanDataCollection.GetScanData()[0].Data;
			}
		}

		public void OnClick(View v)
		{
			if (_scannerManager.IsScannerEnabled)
				_scannerManager.DisableScanner();
			else
				_scannerManager.EnableScanner();
		}
	}
}
