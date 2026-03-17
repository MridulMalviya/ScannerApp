using BarcodeScanning;

namespace ScannerApp;

public partial class MainPage : ContentPage
{
	private bool _vibrationEnabled = true;

	public MainPage()
	{
		InitializeComponent();

		Quality.ItemsSource = Enum.GetValues<CaptureQuality>().Select(q => q.ToString()).ToList();
		Quality.SelectedItem = CaptureQuality.High.ToString();
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
		Camera.VibrationOnDetected = _vibrationEnabled;
		Camera.CameraEnabled = true;
		Camera.PauseScanning = false;
	}

	protected override void OnDisappearing()
	{
		Camera.PauseScanning = true;
		Camera.CameraEnabled = false;

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

	private async void BackButton_Clicked(object sender, EventArgs e)
	{
		if (Navigation.NavigationStack.Count > 1)
			await Navigation.PopAsync();
	}

	private void CameraButton_Clicked(object sender, EventArgs e)
	{
		// Toggle camera on/off
		Camera.CameraEnabled = !Camera.CameraEnabled;
		if (Camera.CameraEnabled)
			Camera.PauseScanning = false;
	}

	private void TorchButton_Clicked(object sender, EventArgs e)
	{
		Camera.TorchOn = !Camera.TorchOn;
	}

	private void VibrateButton_Clicked(object sender, EventArgs e)
	{
		_vibrationEnabled = !_vibrationEnabled;
		Camera.VibrationOnDetected = _vibrationEnabled;
	}

	private void PauseButton_Clicked(object sender, EventArgs e)
	{
		Camera.PauseScanning = !Camera.PauseScanning;
	}

	private void Quality_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (Quality.SelectedItem is not string s)
			return;

		if (Enum.TryParse<CaptureQuality>(s, ignoreCase: true, out var q))
			Camera.CaptureQuality = q;
	}
}

