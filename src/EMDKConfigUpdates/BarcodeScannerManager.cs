using Android.Content;
using Symbol.XamarinEMDK;
using Symbol.XamarinEMDK.Barcode;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace EMDKSampleApp
{
	public class BarcodeScannerManager : Java.Lang.Object, EMDKManager.IEMDKListener, IBarcodeScannerManager
	{
		private EMDKManager _emdkManager;
		private BarcodeManager _barcodeManager;
		private Scanner _scanner;
		public event EventHandler<Scanner.DataEventArgs> ScanReceived;
		public bool IsScannerEnabled { get; private set; }

		public BarcodeScannerManager(Context context)
		{
			InitializeEMDK(context);
		}

		void InitializeEMDK(Context context)
		{
			var results = EMDKManager.GetEMDKManager(context, this);
			if (results.StatusCode != EMDKResults.STATUS_CODE.Success)
				throw new InvalidOperationException("Unable to initialize EMDK Manager");
		}

		void InitializeBarcodeManager()
		{
			_barcodeManager = (BarcodeManager)_emdkManager?.GetInstance(EMDKManager.FEATURE_TYPE.Barcode);
			if (_barcodeManager == null)
				return;

			if (_barcodeManager.SupportedDevicesInfo?.Count > 0)
				_scanner = _barcodeManager.GetDevice(BarcodeManager.DeviceIdentifier.Default);
		}

		public void OnClosed()
		{
			if (_scanner != null)
			{
				_scanner.Data -= OnScanReceived;
				_scanner.Status -= OnStatusChanged;

				_scanner.Disable();
				_scanner.Release();
				_scanner = null;
			}

			if (_emdkManager != null)
			{
				_emdkManager.Release();
				_emdkManager = null;
			}
		}

		public void OnOpened(EMDKManager manager)
		{
			_emdkManager = manager;

			// We can only initialize the barcode manager once we retrieve
			// the EMDK Manager, which is invoked by via callback in the
			// EMDK library
			InitializeBarcodeManager();
		}

		public void Enable()
		{
			if (_scanner == null)
				return;

			_scanner.Data += OnScanReceived;
			_scanner.Status += OnStatusChanged;

			_scanner.Enable();
			_scanner.TriggerType = Scanner.TriggerTypes.Hard;

			IsScannerEnabled = true;
		}

		public void Disable()
		{
			if (_scanner == null)
				return;

			_scanner.Data -= OnScanReceived;
			_scanner.Status -= OnStatusChanged;

			_scanner.Disable();

			IsScannerEnabled = false;
		}

		private void OnStatusChanged(object sender, Scanner.StatusEventArgs args)
		{
			if (args?.P0?.State == StatusData.ScannerStates.Idle)
			{
				Task.Delay(100);
				_scanner.Read();
			}
		}

		private void OnScanReceived(object sender, Scanner.DataEventArgs args)
		{
			MainThread.BeginInvokeOnMainThread(() => ScanReceived?.Invoke(sender, args));
		}
	}
}
