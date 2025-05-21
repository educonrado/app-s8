using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tesseract;

namespace app_s8.Services
{
    public class OcrService
    {
        public async Task<double> ExtraerTotal(string rutaImagen)
        {
            try
            {
                using (var engine = new TesseractEngine("./tessdata", "spa+eng", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(rutaImagen))
                    {
                        using (var page = engine.Process(img))
                        {
                            string texto = page.GetText();
                            return BuscarTotal(texto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error OCR: {ex.Message}");
            }
        }

        private double BuscarTotal(string texto)
        {
            var patrones = new[]
            {
            @"(?i)total[\s:]*\$?\s*(\d+[.,]\d+)",
            @"(?i)importe[\s:]*\$?\s*(\d+[.,]\d+)",
            @"(?i)monto[\s:]*\$?\s*(\d+[.,]\d+)"
        };

            foreach (var patron in patrones)
            {
                var match = Regex.Match(texto, patron);
                if (match.Success)
                {
                    string valor = match.Groups[1].Value.Replace(',', '.');
                    if (double.TryParse(valor, out double total))
                        return total;
                }
            }
            return 0;
        }

    }
}
