using BarcodeScanning;

namespace ScannerApp;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

#if ANDROID
		var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
		if (status != PermissionStatus.Granted)
			status = await Permissions.RequestAsync<Permissions.Camera>();

		if (status != PermissionStatus.Granted)
		{
			await DisplayAlert("Camera permission required", "Please allow camera access to scan barcodes.", "OK");
			return;
		}
#endif

		// Ensure camera/scanning are enabled after permissions are granted.
		try
		{
			Camera.CameraEnabled = true;
			Camera.PauseScanning = false;
		}
		catch
		{
			// Best-effort: keep page usable even if the control API differs by version.
		}
	}

	protected override void OnDisappearing()
	{
		try
		{
			Camera.PauseScanning = true;
			Camera.CameraEnabled = false;
		}
		catch
		{
		}

		base.OnDisappearing();
	}

	private void OnDetectionFinished(object sender, OnDetectionFinishedEventArg e)
	{
		var first = e.BarcodeResults?.FirstOrDefault();
		if (first is null)
			return;

		var value = first.DisplayValue ?? first.RawValue ?? string.Empty;
		if (string.IsNullOrWhiteSpace(value))
			return;

		MainThread.BeginInvokeOnMainThread(() =>
		{
			ResultLabel.Text = $"Detected: {value}";
		});
	}
}

