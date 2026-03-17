using BarcodeScanning;
using Microsoft.Maui.Graphics;

namespace ScannerApp;

public partial class ScanPage : ContentPage
{
	private readonly BarcodeDrawable _drawable = new();
	private readonly List<string> _qualities = [];
	private IDispatcherTimer? _statusTimer;

	public ScanPage()
	{
		InitializeComponent();

		BackButton.Text = "<";

		_qualities.AddRange(["Low", "Medium", "High", "Highest"]);
		Quality.ItemsSource = _qualities;
		Quality.SelectedIndex = 3; // Highest

		if (DeviceInfo.Platform != DevicePlatform.MacCatalyst)
			Quality.Title = "Quality";
	}

	protected override async void OnAppearing()
	{
		var hasPermission = await AskForRequiredPermissionAsync();
		base.OnAppearing();

		if (!hasPermission)
		{
			await DisplayAlert("Camera permission required", "Please allow camera access to scan barcodes.", "OK");
			StatusLabel.Text = "Camera permission: denied";
			return;
		}

		Graphics.Drawable = _drawable;

		// Give the page a moment to layout before enabling camera on some devices.
		await Task.Delay(250);

		Barcode.BarcodeSymbologies = BarcodeFormats.All;
		Barcode.CaptureQuality = CaptureQuality.Highest;
		Barcode.ForceInverted = false;
		Barcode.AimMode = false;
		Barcode.TapToFocusEnabled = true;

		Barcode.CameraEnabled = true;
		Barcode.PauseScanning = false;

		StartStatusTimer();
	}

	protected override void OnDisappearing()
	{
		StopStatusTimer();
		base.OnDisappearing();
		// Keep camera enabled if you want faster back/forward nav; otherwise disable here.
	}

	private void ContentPage_Unloaded(object sender, EventArgs e)
	{
		// Optional: Barcode.Handler?.DisconnectHandler();
	}

	private void CameraView_OnDetectionFinished(object sender, OnDetectionFinishedEventArg e)
	{
		_drawable.BarcodeResults = e.BarcodeResults;
		Graphics.Invalidate();

		var first = e.BarcodeResults?.FirstOrDefault();
		var value = first?.DisplayValue ?? first?.RawValue;

		MainThread.BeginInvokeOnMainThread(() =>
		{
			StatusLabel.Text = string.IsNullOrWhiteSpace(value)
				? $"No barcode (results: {e.BarcodeResults?.Count ?? 0})"
				: $"Detected: {value}";
		});
	}

	private async void BackButton_Clicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("..");
	}

	private void CameraButton_Clicked(object sender, EventArgs e)
	{
		Barcode.CameraFacing = Barcode.CameraFacing == CameraFacing.Back ? CameraFacing.Front : CameraFacing.Back;
	}

	private void TorchButton_Clicked(object sender, EventArgs e)
	{
		Barcode.TorchOn = !Barcode.TorchOn;
	}

	private void AimModeButton_Clicked(object sender, EventArgs e)
	{
		Barcode.AimMode = !Barcode.AimMode;
		StatusLabel.Text = $"AimMode={(Barcode.AimMode ? "ON" : "OFF")}";
	}

	private void VibrateButton_Clicked(object sender, EventArgs e)
	{
		Barcode.VibrationOnDetected = !Barcode.VibrationOnDetected;
	}

	private void PauseButton_Clicked(object sender, EventArgs e)
	{
		Barcode.PauseScanning = !Barcode.PauseScanning;
	}

	private void Quality_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (sender is not Picker picker)
			return;

		if (picker.SelectedIndex >= 0 && picker.SelectedIndex < _qualities.Count)
			Barcode.CaptureQuality = (CaptureQuality)picker.SelectedIndex;
	}

	private static async Task<bool> AskForRequiredPermissionAsync()
	{
#if ANDROID
		var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
		if (status != PermissionStatus.Granted)
			status = await Permissions.RequestAsync<Permissions.Camera>();

		return status == PermissionStatus.Granted;
#else
		return true;
#endif
	}

	private void StartStatusTimer()
	{
		_statusTimer ??= Dispatcher.CreateTimer();
		_statusTimer.Interval = TimeSpan.FromSeconds(2);
		_statusTimer.IsRepeating = true;
		_statusTimer.Tick -= StatusTimerOnTick;
		_statusTimer.Tick += StatusTimerOnTick;

		if (!_statusTimer.IsRunning)
			_statusTimer.Start();
	}

	private void StopStatusTimer()
	{
		try
		{
			_statusTimer?.Stop();
		}
		catch
		{
		}
	}

	private void StatusTimerOnTick(object? sender, EventArgs e)
	{
		StatusLabel.Text =
			$"CameraEnabled={Barcode.CameraEnabled}, PauseScanning={Barcode.PauseScanning}, AimMode={Barcode.AimMode}, Facing={Barcode.CameraFacing}, Torch={Barcode.TorchOn}";
	}

	private sealed class BarcodeDrawable : IDrawable
	{
		public IReadOnlySet<BarcodeResult>? BarcodeResults;

		public void Draw(ICanvas canvas, RectF dirtyRect)
		{
			if (BarcodeResults is not { Count: > 0 })
				return;

			canvas.StrokeSize = 15;
			canvas.StrokeColor = Colors.Red;
			var scale = 1 / canvas.DisplayScale;
			canvas.Scale(scale, scale);

			foreach (var barcode in BarcodeResults)
				canvas.DrawRectangle(barcode.PreviewBoundingBox);
		}
	}
}

