using BarcodeScanning;

namespace ScannerApp;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
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

