using Plugin.Maui.OCR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace app_s8.Services
{
    public class OcrService
    {
        public async Task<double> ExtraerTotal(byte[] imageBytes)
        {
            try
            {
                var ocrResult = await OcrPlugin.Default.RecognizeTextAsync(imageBytes);

                if (ocrResult.Success)
                {
                    return BuscarTotal(ocrResult.AllText);
                }
                return 0;
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
                @"(?i)total\s+a\s+pagar[\s:]*(\d+[.,]\d+)",
                @"(?i)total[\s:]*\$?\s*(\d+[.,]\d+)",
                @"(?i)importe[\s:]*\$?\s*(\d+[.,]\d+)",
                @"(?i)monto[\s:]*\$?\s*(\d+[.,]\d+)",
                @"(\d+[.,]\d+)(?=\s*$)",
                @"(?i)a\s+pagar[\s:]*(\d+[.,]\d+)"
            };

            foreach (var patron in patrones)
            {
                var match = Regex.Match(texto, patron);
                if (match.Success)
                {
                    string valor = match.Groups[1].Value.Replace(',', '.');
                    if (double.TryParse(valor, out double total) && total > 0)
                        return total;
                }
            }

            var numerosEncontrados = new List<double>();
            var patronNumeros = @"(\d+[.,]\d+)";
            var matches = Regex.Matches(texto, patronNumeros);

            foreach (Match match in matches)
            {
                string valor = match.Groups[1].Value.Replace(',', '.');
                if (double.TryParse(valor, out double numero) && numero > 5)
                {
                    numerosEncontrados.Add(numero);
                }
            }

            return numerosEncontrados.Count > 0 ? numerosEncontrados.Max() : 0;
        }

    }
}
