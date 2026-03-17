namespace ScannerApp;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private void OnBarcodesDetected(object sender, BarcodeScanning.Native.Maui.BarcodesDetectedEventArgs e)
	{
		var first = e.Results?.FirstOrDefault();
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

