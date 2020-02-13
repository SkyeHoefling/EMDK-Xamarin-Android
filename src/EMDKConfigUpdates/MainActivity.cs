using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Symbol.XamarinEMDK.Barcode;

namespace EMDKSampleApp
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
	public class MainActivity : AppCompatActivity, View.IOnClickListener
	{
		IBarcodeScannerManager _scannerManager;
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.activity_main);

			_scannerManager = new BarcodeScannerManager(this);
			_scannerManager.ScanReceived += OnScanReceived;

			var toggleScannerButton = FindViewById<Button>(Resource.Id.toggle_scanner_button);
			toggleScannerButton.SetOnClickListener(this);


		}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		_scannerManager.ScanReceived -= OnScanReceived;
		_scannerManager.Dispose();
		_scannerManager = null;
	}

	void OnScanReceived(object sender, Scanner.DataEventArgs args)
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
			_scannerManager.Disable();
		else
			_scannerManager.Enable();
	}
}
}
