using BarcodeScanning;
using Microsoft.Maui.Graphics;

namespace ScannerApp;

public partial class ScanPage : ContentPage
{
	private readonly BarcodeDrawable _drawable = new();
	private readonly List<string> _qualities = [];

	public ScanPage()
	{
		InitializeComponent();

		BackButton.Text = "<";

		_qualities.AddRange(["Low", "Medium", "High", "Highest"]);
		Quality.ItemsSource = _qualities;

		if (DeviceInfo.Platform != DevicePlatform.MacCatalyst)
			Quality.Title = "Quality";
	}

	protected override async void OnAppearing()
	{
		await AskForRequiredPermissionAsync();
		base.OnAppearing();

		Barcode.CameraEnabled = true;
		Barcode.PauseScanning = false;
		Graphics.Drawable = _drawable;
	}

	protected override void OnDisappearing()
	{
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

		if (picker.SelectedIndex > -1 && picker.SelectedIndex < 5)
			Barcode.CaptureQuality = (CaptureQuality)picker.SelectedIndex;
	}

	private static async Task AskForRequiredPermissionAsync()
	{
#if ANDROID
		var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
		if (status != PermissionStatus.Granted)
			status = await Permissions.RequestAsync<Permissions.Camera>();
#endif
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

