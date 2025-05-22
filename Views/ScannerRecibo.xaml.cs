
using System.Diagnostics;
using System.Text.RegularExpressions;
using app_s8.Services;

namespace app_s8.Views;

public partial class ScannerRecibo : ContentPage
{
    private double total = 0;
    public ScannerRecibo()
    {
        InitializeComponent();
    }

    private async void btnCapturarRecibo_Clicked(object sender, EventArgs e)
    {
        if (!MediaPicker.IsCaptureSupported)
        {
            await DisplayAlert("Error", "La captura de fotos no está soportada en este dispositivo", "OK");
            return;
        }

        try
        {
            var status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Permiso denegado", "La aplicación necesita acceso a la cámara", "OK");
                return;
            }

            var foto = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
            {
                Title = "Captura tu recibo"
            });

            if (foto != null)
            {
                var stream = await foto.OpenReadAsync();
                ImageRecibo.Source = ImageSource.FromStream(() => stream);
                brdImagen.IsVisible = true;
                await ProcesarReciboAsync(foto);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Se produjo un error: {ex.Message}", "OK");
        }
    }

    private async Task ProcesarReciboAsync(FileResult foto)
    {
        try
        {
            await DisplayAlert("Procesando", "Analizando recibo...", "OK");
            var tempFile = Path.Combine(FileSystem.CacheDirectory, foto.FileName);

            using (var stream = await foto.OpenReadAsync())
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();
            var ocr = new OcrService();

            total = await ocr.ExtraerTotal(imageBytes);

            if (total > 0)
            {
                lblTotal.Text = $"${total:F2}";
                btnRegistarGasto.IsEnabled = true;
                btnRegistrarIngreso.IsEnabled = true;
            }
            else
            {
                lblTotal.Text = "No se puede detectar el total";
                    btnRegistarGasto.IsEnabled = false;
                    btnRegistrarIngreso.IsEnabled = false;
                }

            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error OCR", $"Error al procesar texto: {ex.Message}", "OK");
        }
    }

    private void btnRegistarGasto_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Views.GastoPage(total));
    }

    private void btnRegistrarIngreso_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Views.IngresoPage(total));
    }
}