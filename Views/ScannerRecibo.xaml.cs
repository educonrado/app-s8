
using System.Diagnostics;
using System.Text.RegularExpressions;
using Tesseract;

namespace app_s8.Views;

public partial class ScannerRecibo : ContentPage
{
    private double totalDetectado = 0;
    public ScannerRecibo()
    {
        InitializeComponent();
    }

    private async void btnUsarTotal_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("Total Capturado", $"Se utilizará el total: ${totalDetectado:F2}", "OK");
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
            using (var fileStream = File.Create(tempFile))
            {
                await stream.CopyToAsync(fileStream);
            }

            string textoExtraido = await Task.Run(() =>
            {
                using (var engine = new TesseractEngine("./tessdata", "spa", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(tempFile))
                    {
                        using (var page = engine.Process(img))
                        {
                            return page.GetText();
                        }
                    }
                }
            });
            totalDetectado = EncontrarTotal(textoExtraido);
            if (totalDetectado > 0)
            {
                lblTotal.Text = $"${totalDetectado:F2}";
                btnUsarTotal.IsEnabled = true;
            }
            else
            {
                lblTotal.Text = "No se puede detectar el total";
                btnUsarTotal.IsEnabled = false;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error OCR", $"Error al procesar texto: {ex.Message}", "OK");
        }
    }

    private double EncontrarTotal(string textoExtraido)
    {
        var patrones = new List<string>
        {
            @"TOTAL[\s:]*\$?\s*(\d+[.,]\d+)",
            @"Total[\s:]*\$?\s*(\d+[.,]\d+)",
            @"IMPORTE[\s:]*\$?\s*(\d+[.,]\d+)",
            @"MONTO[\s:]*\$?\s*(\d+[.,]\d+)",
            @"A PAGAR[\s:]*\$?\s*(\d+[.,]\d+)"
        };

        foreach (var patron in patrones)
        {
            var match = Regex.Match(textoExtraido, patron);
            if (match.Success && match.Groups.Count > 1)
            {
                string valor = match.Groups[1].Value.Replace(',', '.');
                if (double.TryParse(valor, out double total))
                {
                    return total;
                }
            }
        }
        return 0;
    }
}